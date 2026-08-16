using UnityEngine;
using FMODUnity;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public CameraManager cameraManager;
    private GameObject[] allSuspects;
    [SerializeField] private InspectionVisualManager inspectionVisualManager;

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
        // TODO: initialize from case
        kid1.suspectData = Instantiate(bradley);
        kid2.suspectData = Instantiate(maggie);
        FindAllSuspectsInScene();
    }

    private void FindAllSuspectsInScene()
    {
        allSuspects = GameObject.FindGameObjectsWithTag("Suspect");
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
        cameraManager.MoveToDefault();
        foreach (GameObject suspect in allSuspects)
            suspect.GetComponent<SuspectController>().RestoreToReady();
    }

    public void PlaySuspectHoverSound()
    {
        AudioManager.Instance.PlaySound2D(suspectHoverSound);
    }
}
