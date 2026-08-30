using UnityEngine;
using Yarn.Unity;
using FMODUnity;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using PrimeTween;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public CameraManager cameraManager;
    private GameObject[] allSuspects;
    [SerializeField] private InspectionVisualManager inspectionVisualManager;
    [SerializeField] private CaseStartVisualManager caseStartVisualManager;
    [SerializeField] private RestartVisualManager restartVisualManager;
    [SerializeField] private CaseFileVisualManager caseFileVisualManager;
    public DialogueRunner dialogueRunner;
    public bool undiscoveredClues = true;
    public bool stampBeingHeld = false;
    [HideInInspector] public StampController stampController;
    [HideInInspector] public PhoneController phoneController;
    [HideInInspector] public MonocleController monocleController;
    [HideInInspector] public FolderController folderController;
    public GameObject porkpiePrefab;
    public GameObject confettiObject;
    public GameObject failstampPrefab;
    public GameObject optionsMenuGO;
    public Clue[] bonusClues;

    public CaseSO[] allCases;
    public int currentCaseIndex = -1;

    public bool optionsMenuOpen = false; // Any menu open

    [Header("Audio")]
    [SerializeField] private EventReference suspectEnterRoomSound;
    [SerializeField] private EventReference startInspectionSound;
    [SerializeField] private EventReference endInspectionSound;
    [SerializeField] private EventReference moreToDiscoverSound;
    [SerializeField] private EventReference guiltyStampSound;
    [SerializeField] private EventReference playerExpelledSound;
    [SerializeField] private EventReference playerWinSound;
    [SerializeField] private EventReference folderSlideSound; // I'm reusing the dialogue open sound, replace if wanted! - Reagan

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
        FindControllersInScene();
        FindAllSuspectsInScene();
        SceneManager.sceneLoaded += OnSceneLoaded;
        _ = DelayAndRing();
    }

    public async Task StartCase(CaseSO caseToStart)
    {
        caseFileVisualManager.CloseCaseFile();
        await Awaitable.WaitForSecondsAsync(0.05f); // Hacky bug fix - let dialogue complete before opening the next UI so optionsMenuOpen doesn't break!
        await caseStartVisualManager.DisplayStartCase(caseToStart);
        foreach (SuspectSO suspect in caseToStart.suspects)
        {
            GameObject suspectGO = Instantiate(suspect.prefabSuspect);
            suspectGO.name = suspect.name;
        }
        // SOUND :  Instantiating a prefab suspect is the suspects "entering" the room. Currently they just
        // pop into existence, but they could fade in, or slide in, etc.
        AudioManager.Instance.PlaySound2D(suspectEnterRoomSound);
        FindAllSuspectsInScene();
        CheckAllCluesDiscovered();
    }

    private void FindAllSuspectsInScene()
    {
        allSuspects = GameObject.FindGameObjectsWithTag("Suspect");
    }

    private void FindControllersInScene()
    {
        stampController = FindAnyObjectByType<StampController>();
        phoneController = FindAnyObjectByType<PhoneController>();
        monocleController = FindAnyObjectByType<MonocleController>(FindObjectsInactive.Include);
        folderController = FindAnyObjectByType<FolderController>(FindObjectsInactive.Include);
    }

    public void StartInspection(GameObject targetSuspect)
    {
        caseFileVisualManager.CloseCaseFile();
        AudioManager.Instance.PlaySound2D(startInspectionSound);
        inspectionVisualManager.StartInspection(targetSuspect);
        cameraManager.MoveToInspection(targetSuspect);
        foreach (GameObject suspect in allSuspects)
            if (suspect != targetSuspect) suspect.GetComponent<SuspectController>().FadeOut();
        monocleController.gameObject.SetActive(true);
    }

    public void EndInspection()
    {
        _ = dialogueRunner.Stop();
        // SOUND :  We're exiting inspect mode and going back to the regular view.
        // This is the "back to default view" button being clicked in inspect mode
        // (the "permanent record" slides back out, and camera zooms out, and the monocle disappears)
        AudioManager.Instance.PlaySound2D(endInspectionSound);
        cameraManager.MoveToDefault();
        foreach (GameObject suspect in allSuspects)
            suspect.GetComponent<SuspectController>().RestoreToReady();
        monocleController.gameObject.SetActive(false);
    }

    public void RunDialogue(string startNode)
    {
        _ = dialogueRunner.StartDialogue(startNode);
    }

    public async YarnTask ClickClue(Clue clue, SuspectController suspect)
    {
        clue.discovered = true;
        CheckAllCluesDiscovered();
        // Hide monocle while clue dialogue plays
        monocleController.gameObject.SetActive(false);
        suspect.HideAllClues();

        //Hide Permanent Record

        await dialogueRunner.StartDialogue(clue.yarnDialogueNode);
        await dialogueRunner.DialogueTask;

        // Resume
        suspect.ShowAllClues();
        suspect.PlayAnimation(suspect.freezeAnimationName);
        monocleController.gameObject.SetActive(true);
    }

    public void CheckAllCluesDiscovered()
    {
        bool allCluesDiscovered = true;
        foreach (GameObject suspect in allSuspects)
        {
            SuspectController suspectController = suspect.GetComponent<SuspectController>();
            if (!suspectController.suspectData) allCluesDiscovered = false; // Not set yet
            else
            {
                foreach (Clue clue in suspectController.suspectData.clues)
                {
                    if (!clue.discovered) allCluesDiscovered = false;
                }
            }
        }
        undiscoveredClues = !allCluesDiscovered;
    }

    // Returns true if all clues have been discovered, and false otherwise 
    public bool PrepareToStamp()
    {
        caseFileVisualManager.CloseCaseFile();
        if (undiscoveredClues)
        {
            _ = dialogueRunner.StartDialogue("UndiscoveredClues");
            // SOUND :  The player tried to pick up the stamp, but there were undiscovered clues, so
            // the dialogue with Principal Judge thinking "there's more to discover here..." plays. The dialogue might have sound, 
            // but if it doesn't, a sound to indicate "denial" here may be helpful since the stamp won't get picked up 
            AudioManager.Instance.PlaySound2D(moreToDiscoverSound);
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

    public async void StampedGuilty(SuspectController accusedSuspect)
    {
        // SOUND :  A suspect was stamped as the guilty one. 
        AudioManager.Instance.PlaySound2D(guiltyStampSound);
        foreach (GameObject suspect in allSuspects)
            suspect.GetComponent<SuspectController>().state = SuspectState.Judged;
        PutDownStamp();
        if (allCases[currentCaseIndex].guiltySuspect == accusedSuspect.suspectOrigin)
        {// Guilty party was accused 
            await accusedSuspect.WaitThenFade();
            _ = dialogueRunner.StartDialogue(allCases[currentCaseIndex].yarnSuccessNode);
        }
        else
        {
            foreach (GameObject suspect in allSuspects)
                _ = suspect.GetComponent<SuspectController>().WaitThenFade();
            phoneController.StartPhoneRinging();
        }
    }

    public void PhonePickedUp()
    {
        caseFileVisualManager.CloseCaseFile();
        if (currentCaseIndex >= 0)
            _ = dialogueRunner.StartDialogue(allCases[currentCaseIndex].yarnFailureNode);
        else
            _ = dialogueRunner.StartDialogue("TutorialCall");
    }

    [YarnCommand("next_case")]
    public void StartNextCase()
    {
        // Remove current suspects
        foreach (GameObject suspect in allSuspects)
            Destroy(suspect);
        // SOUND :  As the success or failure dialogue finishes, the current case gets cleared.
        // Currently, the suspects just disappear, but they could fade out/slide out/etc. 
        //AudioManager.Instance.PlaySound2D(guiltyStampSound);
        optionsMenuOpen = true;
        if (currentCaseIndex < (allCases.Length - 1))
        {
            currentCaseIndex++;
            _ = StartCase(allCases[currentCaseIndex]);
        }
        else
        {
            Debug.Log("ERROR: There are no more cases to play.");
        }
    }

    private async Task FadeOutAllSuspects()
    {
        foreach (GameObject suspect in allSuspects)
            suspect.GetComponent<SuspectController>().FadeOut();
        await Awaitable.WaitForSecondsAsync(1.0f); // wait for fading to finish 
        foreach (GameObject suspect in allSuspects)
            Destroy(suspect);
    }

    [YarnCommand("game_end_success")]
    public async void GameEndSuccess()
    {
        await FadeOutAllSuspects();
        Instantiate(porkpiePrefab);
        Instantiate(confettiObject);
        await dialogueRunner.StartDialogue("PorkPieSuccess");
        await dialogueRunner.DialogueTask;
        AudioManager.Instance.PlaySound2D(playerWinSound);
        restartVisualManager.ShowRestart();
    }

    [YarnCommand("game_end_failure")]
    public async void GameEndFailure()
    {
        await FadeOutAllSuspects();
        await dialogueRunner.StartDialogue("PrincipalJudgeFailure");
        await dialogueRunner.DialogueTask;
        Instantiate(failstampPrefab);
        AudioManager.Instance.PlaySound2D(playerExpelledSound);
        AudioManager.Instance.PlayExpelledMusic();
        await Awaitable.WaitForSecondsAsync(3.0f);
        restartVisualManager.ShowRestart();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Start") // If we've gone back to the start menu, consider it a restart
        {
            currentCaseIndex = -1;
            optionsMenuGO.SetActive(false);
        }
        if (scene.name == "Office") // Restarting game after coming from start
        {
            optionsMenuGO.SetActive(true);
            FindControllersInScene();
            FindAllSuspectsInScene();
            _ = DelayAndRing();
        }
    }

    public async Task DelayAndRing()
    {
        await Awaitable.WaitForSecondsAsync(3.0f);
        phoneController.StartPhoneRinging();
    }

    public void OpenCaseFile()
    {
        caseFileVisualManager.OpenCaseFile(bonusClues);
    }

    [YarnCommand("unlock_casefile")]
    public void UnlockCaseFile()
    {
        _ = UnlockCaseFileAsync();
    }

    private async Task UnlockCaseFileAsync()
    {
        Vector3 targetPos = folderController.gameObject.transform.position;
        GameObject folderGO = folderController.gameObject;
        folderGO.transform.position = new Vector3(targetPos.x, targetPos.y - 5f, targetPos.z);
        folderController.gameObject.SetActive(true);
        await Tween.PositionY(folderGO.transform, endValue: targetPos.y, duration: 0.5f);
        AudioManager.Instance.PlaySound2D(folderSlideSound);
        StartNextCase();
    }

    [YarnCommand("discover_bonusclue")]
    public void DiscoverBonusClue(int clueIndex)
    {
        bonusClues[clueIndex].discovered = true;
    }
}
