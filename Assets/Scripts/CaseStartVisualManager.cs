using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using FMODUnity;

public class CaseStartVisualManager : MonoBehaviour
{
    public UIDocument caseStartUIDoc;
    private VisualElement caseStartUI;

    [Header("Audio")]
    [SerializeField] private EventReference caseOpenSound;
    [SerializeField] private EventReference caseStartSound;
    [SerializeField] private EventReference caseHoverSound;

    public void OnEnable()
    {
        caseStartUI = caseStartUIDoc.rootVisualElement;
        caseStartUI.Q<Button>("StartCase").RegisterCallback<PointerEnterEvent>(OnButtonHover);
    }

    public void OnDisable()
    {
        caseStartUI.Q<Button>("StartCase").UnregisterCallback<PointerEnterEvent>(OnButtonHover);
    }

    public Task DisplayStartCase(CaseSO caseToStart)
    {
        // SOUND :  This is where the initial case popup opens. It will open at the start of each case. 
        AudioManager.Instance.PlaySound2D(caseOpenSound);
        var tcs = new TaskCompletionSource<bool>();
        caseStartUI.dataSource = caseToStart;
        caseStartUI.Q<VisualElement>("Overlay").style.display = DisplayStyle.Flex;
        caseStartUI.Q<Button>("StartCase").clicked += ClickedStart;
        void ClickedStart()
        {
            // SOUND :  The "Start Case" button is clicked and the popup goes away.  
            AudioManager.Instance.PlaySound2D(caseStartSound);
            caseStartUI.Q<Button>("StartCase").clicked -= ClickedStart;
            caseStartUI.Q<VisualElement>("Overlay").style.display = DisplayStyle.None;
            tcs.TrySetResult(true); // Wait until button is clicked
        }
        return tcs.Task;
    }

    public void OnButtonHover(PointerEnterEvent evt)
    {
        // SOUND :  The "start case" button was hovered over
        AudioManager.Instance.PlaySound2D(caseHoverSound);
        return;
    }
}
