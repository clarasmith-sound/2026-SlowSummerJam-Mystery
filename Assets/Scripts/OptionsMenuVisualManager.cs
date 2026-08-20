using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class OptionsMenuVisualManager : MonoBehaviour
{
    public UIDocument optionsMenuUIDoc;
    private VisualElement optionsUI;

    public void OnEnable()
    {
        optionsUI = optionsMenuUIDoc.rootVisualElement;
        optionsUI.Q<Button>("BackToStart").RegisterCallback<PointerEnterEvent>(OnButtonHover);
        optionsUI.Q<Button>("BackToGame").RegisterCallback<PointerEnterEvent>(OnButtonHover);
        optionsUI.Q<Button>("OpenOptionsMenu").RegisterCallback<PointerEnterEvent>(OnButtonHover);
        optionsUI.Q<Button>("BackToStart").clicked += BackToStartMenu;
        optionsUI.Q<Button>("OpenOptionsMenu").clicked += ShowOptionsMenu;
        optionsUI.Q<Button>("BackToGame").clicked += HideOptionsMenu;
    }

    public void OnDisable()
    {
        optionsUI.Q<Button>("BackToStart").UnregisterCallback<PointerEnterEvent>(OnButtonHover);
        optionsUI.Q<Button>("BackToGame").UnregisterCallback<PointerEnterEvent>(OnButtonHover);
        optionsUI.Q<Button>("OpenOptionsMenu").UnregisterCallback<PointerEnterEvent>(OnButtonHover);
        optionsUI.Q<Button>("BackToStart").clicked -= BackToStartMenu;
        optionsUI.Q<Button>("OpenOptionsMenu").clicked -= ShowOptionsMenu;
        optionsUI.Q<Button>("BackToGame").clicked -= HideOptionsMenu;
    }

    public void ShowOptionsMenu()
    {
        // TODO - SOUND : options menus opens 
        optionsUI.Q<VisualElement>("OptionsMenu").style.display = DisplayStyle.Flex;
    }

    public void BackToStartMenu()
    {
        SceneManager.LoadScene("Start", LoadSceneMode.Single);
    }

    public void HideOptionsMenu()
    {
        // TODO - SOUND : options menus closes 
        optionsUI.Q<VisualElement>("OptionsMenu").style.display = DisplayStyle.None;
    }

    public void OnButtonHover(PointerEnterEvent evt)
    {
        // TODO - SOUND : a button in the settings menu was hovered over
        return;
    }
}
