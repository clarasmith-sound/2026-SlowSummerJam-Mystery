using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class RestartVisualManager : MonoBehaviour
{
    public UIDocument restartUIDoc;
    private VisualElement restartUI;

    public void OnEnable()
    {
        restartUI = restartUIDoc.rootVisualElement;
        restartUI.Q<Button>("RestartGame").clicked += RestartClicked;
    }

    public void OnDisable()
    {
        restartUI.Q<Button>("RestartGame").clicked -= RestartClicked;
    }

    public void ShowRestart()
    {
        VisualElement container = restartUI.Q<VisualElement>("RestartMenu");
        if (container != null)
        {
            container.style.display = DisplayStyle.Flex;
        }
    }

    public void HideRestart()
    {
        VisualElement container = restartUI.Q<VisualElement>("RestartMenu");
        if (container != null)
        {
            container.style.display = DisplayStyle.None;
        }
    }

    public void RestartClicked()
    {
        HideRestart();
        SceneManager.LoadScene("Start", LoadSceneMode.Single);
    }
}
