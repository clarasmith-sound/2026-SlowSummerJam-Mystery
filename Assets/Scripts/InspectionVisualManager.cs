using UnityEngine;
using UnityEngine.UIElements;
using FMODUnity;

public class InspectionVisualManager : MonoBehaviour
{

    public UIDocument inspectionUIDoc;
    private VisualElement inspectionUI;

    [Header("Audio")]
    [SerializeField] private EventReference buttonHoverSound;

    [Header("Clue Row Template")]
    [SerializeField] private VisualTreeAsset clueRowTemplate;
    private bool inInspection = false;

    public void OnEnable()
    {
        inspectionUI = inspectionUIDoc.rootVisualElement;
        inspectionUI.Q<Button>("ExitInspection").clicked += CallEndInspection;
        inspectionUI.Q<Button>("ExitInspection").RegisterCallback<PointerEnterEvent>(OnButtonHover);
    }

    public void OnDisable()
    {
        inspectionUI.Q<Button>("ExitInspection").clicked -= CallEndInspection;
        inspectionUI.Q<Button>("ExitInspection").UnregisterCallback<PointerEnterEvent>(OnButtonHover);
    }

    public void StartInspection(GameObject targetSuspect)
    {
        inInspection = true;
        if (GameManager.Instance != null) GameManager.Instance.optionsMenuOpen = true;

        inspectionUI.Q<VisualElement>("Clues").Clear();
        SuspectSO suspectData = targetSuspect.GetComponent<SuspectController>().suspectData;
        foreach (Clue clue in suspectData.clues)
        {
            VisualTreeAsset clueAsset = clueRowTemplate;
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
        inInspection = false;
        if (GameManager.Instance != null) GameManager.Instance.optionsMenuOpen = false;
        inspectionUI.Q<VisualElement>("PermanentRecord").AddToClassList("hidden");
        inspectionUI.Q<Button>("ExitInspection").style.display = DisplayStyle.None;
    }

    public void CallEndInspection()
    {
        GameManager.Instance.EndInspection();
        EndInspection();
    }

    public void OnButtonHover(PointerEnterEvent evt)
    {
        // SOUND :  The "back to default view" button was hovered over
        // Note, the place I noted for the click of this button is actually in GameManager
        // in EndInspection (since it also causes a camera zoom and the permanent record to slide off right)
        AudioManager.Instance.PlaySound2D(buttonHoverSound);
    }

    public void HidePermanentRecord()
    {
        inspectionUI.Q<VisualElement>("PermanentRecord").AddToClassList("hidden");
    }

    public void ShowPermanentRecord()
    {
        if (!inInspection) return;
        if (GameManager.Instance != null) GameManager.Instance.optionsMenuOpen = true;
        inspectionUI.Q<VisualElement>("PermanentRecord").RemoveFromClassList("hidden");
    }
}
