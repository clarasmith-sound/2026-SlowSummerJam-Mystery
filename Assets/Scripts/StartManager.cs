using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    public UIDocument startUIDoc;
    private VisualElement startUI;

    public void OnEnable()
    {
        startUI = startUIDoc.rootVisualElement;
        startUI.Q<Button>("StartGame").RegisterCallback<PointerEnterEvent>(OnButtonHover);
        startUI.Q<Button>("StartGame").clicked += StartGame;
    }

    public void OnDisable()
    {
        startUI.Q<Button>("StartGame").UnregisterCallback<PointerEnterEvent>(OnButtonHover);
        startUI.Q<Button>("StartGame").clicked -= StartGame;
    }

    public void OnButtonHover(PointerEnterEvent evt)
    {
        // TODO - SOUND : a button in the start menu was hovered over
        return;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Office", LoadSceneMode.Single);
    }
}
