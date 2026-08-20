using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class CaseStartVisualManager : MonoBehaviour
{
    public UIDocument caseStartUIDoc;
    private VisualElement caseStartUI;

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
        // TODO - SOUND :  This is where the initial case popup opens. It will open at the start of each case. 
        var tcs = new TaskCompletionSource<bool>();
        caseStartUI.dataSource = caseToStart;
        caseStartUI.Q<VisualElement>("Overlay").style.display = DisplayStyle.Flex;
        caseStartUI.Q<Button>("StartCase").clicked += ClickedStart;
        void ClickedStart()
        {
            // TODO - SOUND :  The "Start Case" button is clicked and the popup goes away.  
            caseStartUI.Q<Button>("StartCase").clicked -= ClickedStart;
            caseStartUI.Q<VisualElement>("Overlay").style.display = DisplayStyle.None;
            tcs.TrySetResult(true); // Wait until button is clicked
        }
        return tcs.Task;
    }

    public void OnButtonHover(PointerEnterEvent evt)
    {
        // TODO - SOUND :  The "start case" button was hovered over
        return;
    }
}
