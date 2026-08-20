using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using FMODUnity;

public class OptionsMenuVisualManager : MonoBehaviour
{
    public UIDocument optionsMenuUIDoc;
    private VisualElement optionsUI;

    [Header("Audio")]
    [SerializeField] private EventReference buttonHoverSound;
    [SerializeField] private EventReference buttonSelectSound;
    [SerializeField] private EventReference buttonBackSound;

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
        // SOUND : options menus opens 
        AudioManager.Instance.PlaySound2D(buttonSelectSound);
        optionsUI.Q<VisualElement>("OptionsMenu").style.display = DisplayStyle.Flex;
    }

    public void BackToStartMenu()
    {
        // SOUND : options menus closes and goes back to start menu
        AudioManager.Instance.PlaySound2D(buttonBackSound);
        AudioManager.Instance.SaveVolumeSettingsToDisk();
        SceneManager.LoadScene("Start", LoadSceneMode.Single);
    }

    public void HideOptionsMenu()
    {
        // SOUND : options menus closes 
        AudioManager.Instance.PlaySound2D(buttonBackSound);
        AudioManager.Instance.SaveVolumeSettingsToDisk();
        optionsUI.Q<VisualElement>("OptionsMenu").style.display = DisplayStyle.None;
    }

    public void OnButtonHover(PointerEnterEvent evt)
    {
        // SOUND : a button in the settings menu was hovered over
        AudioManager.Instance.PlaySound2D(buttonHoverSound);
    }
}
