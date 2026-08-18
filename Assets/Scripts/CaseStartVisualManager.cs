using UnityEngine;
using UnityEngine.UIElements;

public class CaseStartVisualManager : MonoBehaviour
{

    public UIDocument caseStartUIDoc;
    private VisualElement caseStartUI;

    public void OnEnable()
    {
        caseStartUI = caseStartUIDoc.rootVisualElement;
        caseStartUI.Q<Button>("StartCase").clicked += ClickedStart;
    }

    public void OnDisable()
    {
        caseStartUI.Q<Button>("StartCase").clicked -= ClickedStart;
    }

    public void ClickedStart()
    {
        caseStartUI.Q<VisualElement>("Overlay").style.display = DisplayStyle.None;
    }

    public void DisplayStartCase(CaseSO caseToStart)
    {
        caseStartUI.dataSource = caseToStart;
        caseStartUI.Q<VisualElement>("Overlay").style.display = DisplayStyle.Flex;
    }

}
