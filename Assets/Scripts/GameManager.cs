using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public CameraManager cameraManager;
    public UIDocument inspectionUIDoc;
    private VisualElement inspectionUI;
    private GameObject[] allSuspects;

    // TODO: Don't hard code the suspects (attach prefab GameObject to the scriptable object and instantiate from there)
    public SuspectController kid1; // dont do this
    public SuspectSO bradley;// dont do this
    public SuspectController kid2;// dont do this
    public SuspectSO maggie;// dont do this

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

    // TODO: split this out into a visual manager
    public void OnEnable()
    {
        inspectionUI = inspectionUIDoc.rootVisualElement;
        inspectionUI.Q<Button>("ExitInspection").clicked += EndInspection;
    }

    public void OnDisable()
    {
        inspectionUI.Q<Button>("ExitInspection").clicked -= EndInspection;
    }

    public void StartInspection(GameObject targetSuspect)
    {
        inspectionUI.Q<VisualElement>("Clues").Clear();
        cameraManager.MoveToInspection(targetSuspect);
        foreach (GameObject suspect in allSuspects)
            if (suspect != targetSuspect) suspect.GetComponent<SuspectController>().state = SuspectState.Blurred;

        SuspectSO suspectData = targetSuspect.GetComponent<SuspectController>().suspectData;
        foreach (Clue clue in suspectData.clues)
        {
            VisualTreeAsset clueAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/PermanentRecordClue.uxml");
            VisualElement clueUI = clueAsset.Instantiate();
            clueUI.dataSource = clue;
            inspectionUI.Q<VisualElement>("Clues").Add(clueUI);
        }
        inspectionUI.Q<VisualElement>("PermanentRecord").dataSource = suspectData;
        inspectionUI.Q<VisualElement>("PermanentRecord").RemoveFromClassList("hidden");
        inspectionUI.Q<Button>("ExitInspection").style.display = DisplayStyle.Flex;
    }

    public void EndInspection()
    {
        cameraManager.MoveToDefault();
        foreach (GameObject suspect in allSuspects)
            suspect.GetComponent<SuspectController>().RestoreToReady();
        inspectionUI.Q<VisualElement>("PermanentRecord").AddToClassList("hidden");
        inspectionUI.Q<Button>("ExitInspection").style.display = DisplayStyle.None;
    }
}
