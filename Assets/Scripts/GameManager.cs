using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using Yarn.Unity;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public CameraManager cameraManager;
    private GameObject[] allSuspects;
    [SerializeField] private InspectionVisualManager inspectionVisualManager;
    [SerializeField] private CaseStartVisualManager caseStartVisualManager;
    public DialogueRunner dialogueRunner;
    public bool undiscoveredClues = true;
    public bool stampBeingHeld = false;
    [HideInInspector] public StampController stampController;
    [HideInInspector] public PhoneController phoneController;
    [HideInInspector] public MonocleController monocleController;
    public CaseSO[] allCases;
    public int currentCaseIndex = 0;


    private void Awake()
    {
        // Check if an instance already exists in the scene
        if (Instance != null && Instance != this)
        {
            // If yes, destroy the duplicate object
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        StartCase(allCases[currentCaseIndex]);
        FindControllersInScene();
    }

    public async void StartCase(CaseSO caseToStart)
    {
        await caseStartVisualManager.DisplayStartCase(caseToStart);
        foreach (SuspectSO suspect in caseToStart.suspects)
            Instantiate(suspect.prefabSuspect);
        // TODO - SOUND :  Instantiating a prefab suspect is the suspects "entering" the room. Currently they just
        // pop into existence, but they could fade in, or slide in, etc.
        FindAllSuspectsInScene();
    }

    private void FindAllSuspectsInScene()
    {
        allSuspects = GameObject.FindGameObjectsWithTag("Suspect");
    }

    private void FindControllersInScene()
    {
        // This is only necessary if GameManager persists from the title screen. If it only exists in this scene, we can just assign the GameObject
        stampController = FindAnyObjectByType<StampController>();
        phoneController = FindAnyObjectByType<PhoneController>();
        monocleController = FindAnyObjectByType<MonocleController>(FindObjectsInactive.Include);
    }

    public void StartInspection(GameObject targetSuspect)
    {
        // TODO - SOUND :  A suspect was clicked, and we're entering inspect mode.
        // (the click also triggers the "permanent record" to slide out, the camera to zoom in, and the monocle to be picked up)
        inspectionVisualManager.StartInspection(targetSuspect);
        cameraManager.MoveToInspection(targetSuspect);
        foreach (GameObject suspect in allSuspects)
            if (suspect != targetSuspect) suspect.GetComponent<SuspectController>().state = SuspectState.Blurred;
        monocleController.gameObject.SetActive(true);
    }

    public void EndInspection()
    {
        _ = dialogueRunner.Stop();
        // TODO - SOUND :  We're exiting inspect mode and going back to the regular view.
        // This is the "back to default view" button being clicked in inspect mode
        // (the "permanent record" slides back out, and camera zooms out, and the monocle disappears)
        cameraManager.MoveToDefault();
        foreach (GameObject suspect in allSuspects)
            suspect.GetComponent<SuspectController>().RestoreToReady();
        monocleController.gameObject.SetActive(false);
    }

    public void RunDialogue(string startNode)
    {
        // TODO - GAME DESIGN: I would expect clicking where I clicked again progresses the dialogue, but it restarts
        // Evaluate this UX, maybe once a node starts, you enter a state where clicks continue instead of restarting it
        _ = dialogueRunner.StartDialogue(startNode);
    }

    public void CheckAllCluesDiscovered()
    {
        bool allCluesDiscovered = true;
        foreach (GameObject suspect in allSuspects)
        {
            foreach (Clue clue in suspect.GetComponent<SuspectController>().suspectData.clues)
            {
                if (!clue.discovered) allCluesDiscovered = false;
            }
        }
        undiscoveredClues = !allCluesDiscovered;
    }

    // Returns true if all clues have been discovered, and false otherwise 
    public bool PrepareToStamp()
    {
        if (undiscoveredClues)
        {
            _ = dialogueRunner.StartDialogue("UndiscoveredClues");
            // TODO - SOUND :  The player tried to pick up the stamp, but there were undiscovered clues, so
            // the dialogue with Principal Judge thinking "there's more to discover here..." plays. The dialogue might have sound, 
            // but if it doesn't, a sound to indicate "denial" here may be helpful since the stamp won't get picked up 
            return false;
        }
        else
        {
            stampBeingHeld = true;
            return true;
        }
    }

    public void PutDownStamp()
    {
        stampBeingHeld = false;
        stampController.PutDownStamp();
    }

    public void StampedGuilty(SuspectController accusedSuspect)
    {
        // TODO - SOUND :  A suspect was stamped as the guilty one. 
        foreach (GameObject suspect in allSuspects)
            suspect.GetComponent<SuspectController>().state = SuspectState.Judged;
        PutDownStamp();
        if (allCases[currentCaseIndex].guiltySuspect == accusedSuspect.suspectOrigin) // Guilty party was accused
            _ = dialogueRunner.StartDialogue(allCases[currentCaseIndex].yarnSuccessNode);
        else
            phoneController.StartPhoneRinging();
    }

    public void PhonePickedUp()
    {
        _ = dialogueRunner.StartDialogue(allCases[currentCaseIndex].yarnFailureNode);
    }

    [YarnCommand("next_case")]
    public void StartNextCase()
    {
        // Remove current suspects
        foreach (GameObject suspect in allSuspects)
            Destroy(suspect);
        // TODO - SOUND :  As the success or failure dialogue finishes, the current case gets cleared.
        // Currently, the suspects just disappear, but they could fade out/slide out/etc. 
        if (currentCaseIndex < (allCases.Length - 1))
        {
            currentCaseIndex++;
            StartCase(allCases[currentCaseIndex]);
        }
        else
        {
            // TODO: Game over
            Debug.Log("There are no more cases");
        }
    }
}
