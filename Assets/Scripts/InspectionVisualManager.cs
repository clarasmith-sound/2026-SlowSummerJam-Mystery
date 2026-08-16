using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class InspectionVisualManager : MonoBehaviour
{

    public UIDocument inspectionUIDoc;
    private VisualElement inspectionUI;

    public void OnEnable()
    {
        inspectionUI = inspectionUIDoc.rootVisualElement;
        inspectionUI.Q<Button>("ExitInspection").clicked += CallEndInspection;
    }

    public void OnDisable()
    {
        inspectionUI.Q<Button>("ExitInspection").clicked -= CallEndInspection;
    }

    public void StartInspection(GameObject targetSuspect)
    {
        inspectionUI.Q<VisualElement>("Clues").Clear();
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
        inspectionUI.Q<VisualElement>("PermanentRecord").AddToClassList("hidden");
        inspectionUI.Q<Button>("ExitInspection").style.display = DisplayStyle.None;
    }

    public void CallEndInspection()
    {
        GameManager.Instance.EndInspection();
        EndInspection();
    }
}
