using UnityEngine;
using FMODUnity;
using Yarn.Unity;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public CameraManager cameraManager;
    private GameObject[] allSuspects;
    [SerializeField] private InspectionVisualManager inspectionVisualManager;
    public DialogueRunner dialogueRunner;
    public bool undiscoveredClues = true;
    public bool stampBeingHeld = false;
    public StampController stampController;

    // TODO: Don't hard code the suspects (attach prefab GameObject to the scriptable object and instantiate from there)
    public SuspectController kid1; // dont do this
    public SuspectSO bradley;// dont do this
    public SuspectController kid2;// dont do this
    public SuspectSO maggie;// dont do this

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
        // TODO: initialize dynamically from case
        kid1.suspectData = Instantiate(bradley);
        kid2.suspectData = Instantiate(maggie);
        FindAllSuspectsInScene();
        FindJudgementStamp();
    }

    private void FindAllSuspectsInScene()
    {
        allSuspects = GameObject.FindGameObjectsWithTag("Suspect");
    }

    private void FindJudgementStamp()
    {
        stampController = GameObject.FindGameObjectWithTag("JudgementStamp").GetComponent<StampController>();
    }

    public void StartInspection(GameObject targetSuspect)
    {
        inspectionVisualManager.StartInspection(targetSuspect);
        cameraManager.MoveToInspection(targetSuspect);
        foreach (GameObject suspect in allSuspects)
            if (suspect != targetSuspect) suspect.GetComponent<SuspectController>().state = SuspectState.Blurred;
    }

    public void EndInspection()
    {
        _ = dialogueRunner.Stop();
        cameraManager.MoveToDefault();
        foreach (GameObject suspect in allSuspects)
            suspect.GetComponent<SuspectController>().RestoreToReady();
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

    public void StampedGuilty(GameObject suspectGO)
    {
        foreach (GameObject suspect in allSuspects)
            suspect.GetComponent<SuspectController>().state = SuspectState.Judged;
        PutDownStamp();
        // TODO: actually pick a guilty party and include a guilty result as well
        // TODO: pick dialogue dynamically from the relevant case
        _ = dialogueRunner.StartDialogue("DemoCaseSuccess");
    }
}
