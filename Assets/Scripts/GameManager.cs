using UnityEngine;
using FMODUnity;
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

    [Header("Audio")]
    [SerializeField] private EventReference suspectHoverSound;

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
        inspectionVisualManager.StartInspection(targetSuspect);
        cameraManager.MoveToInspection(targetSuspect);
        foreach (GameObject suspect in allSuspects)
            if (suspect != targetSuspect) suspect.GetComponent<SuspectController>().state = SuspectState.Blurred;
        monocleController.gameObject.SetActive(true);
    }

    public void EndInspection()
    {
        _ = dialogueRunner.Stop();
        cameraManager.MoveToDefault();
        foreach (GameObject suspect in allSuspects)
            suspect.GetComponent<SuspectController>().RestoreToReady();
        monocleController.gameObject.SetActive(false);
    }

    public void PlaySuspectHoverSound()
    {
        AudioManager.Instance.PlaySound2D(suspectHoverSound);
    }

    public void RunDialogue(string startNode)
    {
        // TODO: I would expect clicking where I clicked again progresses the dialogue, but it restarts in (in the case of multi-line dialogue options)
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
