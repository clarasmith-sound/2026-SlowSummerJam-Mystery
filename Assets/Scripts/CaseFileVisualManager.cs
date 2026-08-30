using PrimeTween;
using UnityEngine;
using UnityEngine.UIElements;
using FMODUnity;

public class CaseFileVisualManager : MonoBehaviour
{
    public UIDocument caseFileUIDoc;
    private VisualElement caseFileUI;

    [Header("Audio")]
    [SerializeField] private EventReference buttonHoverSound;
    [SerializeField] private EventReference folderOpenSound;
    [SerializeField] private EventReference folderCloseSound;

    public void OnEnable()
    {
        caseFileUI = caseFileUIDoc.rootVisualElement;
        caseFileUI.Q<Button>("ContinueButton").clicked += CloseCaseFile;
        caseFileUI.Q<Button>("ContinueButton").RegisterCallback<PointerEnterEvent>(OnButtonHover);
    }

    public void OnDisable()
    {
        caseFileUI.Q<Button>("ContinueButton").clicked -= CloseCaseFile;
        caseFileUI.Q<Button>("ContinueButton").UnregisterCallback<PointerEnterEvent>(OnButtonHover);
    }

    public void OpenCaseFile(Clue[] clues)
    {
        if (GameManager.Instance != null) GameManager.Instance.optionsMenuOpen = true;
        caseFileUI.Q<VisualElement>("Clue0").dataSource = clues[0];
        caseFileUI.Q<VisualElement>("Clue1").dataSource = clues[1];
        caseFileUI.Q<VisualElement>("Clue2").dataSource = clues[2];
        caseFileUI.Q<VisualElement>("CaseFileMenu").style.display = DisplayStyle.Flex;
        Sequence.Create(cycles: 1)
           .Group(caseFileUI.VisualElementShakeScale(new ShakeSettings(strength: new Vector3(.1f, .1f, .1f), duration: 0.3f, frequency: 3)))
           .Group(caseFileUI.VisualElementPunchRotation(new ShakeSettings(strength: new Vector3(0f, 0f, -5f), duration: 0.25f, frequency: 5)));
        AudioManager.Instance.PlaySound2D(folderOpenSound);
    }

    public void CloseCaseFile()
    {
        if (GameManager.Instance != null) GameManager.Instance.optionsMenuOpen = false;
        caseFileUI.Q<VisualElement>("CaseFileMenu").style.display = DisplayStyle.None;
        AudioManager.Instance.PlaySound2D(folderCloseSound);
    }

    public void OnButtonHover(PointerEnterEvent evt)
    {
        AudioManager.Instance.PlaySound2D(buttonHoverSound);
        return;
    }
}
