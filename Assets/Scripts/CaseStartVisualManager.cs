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
    }

    public Task DisplayStartCase(CaseSO caseToStart)
    {
        var tcs = new TaskCompletionSource<bool>();
        caseStartUI.dataSource = caseToStart;
        caseStartUI.Q<VisualElement>("Overlay").style.display = DisplayStyle.Flex;
        caseStartUI.Q<Button>("StartCase").clicked += ClickedStart;
        void ClickedStart()
        {
            caseStartUI.Q<Button>("StartCase").clicked -= ClickedStart;
            caseStartUI.Q<VisualElement>("Overlay").style.display = DisplayStyle.None;
            tcs.TrySetResult(true); // Wait until button is clicked
        }
        return tcs.Task;
    }
}
