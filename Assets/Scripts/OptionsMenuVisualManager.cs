using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using FMODUnity;
using System;
using System.Collections.Generic;

public class OptionsMenuVisualManager : MonoBehaviour
{
    public UIDocument optionsMenuUIDoc;
    private VisualElement optionsUI;

    [Header("Audio Properties")]
    [SerializeField] private EventReference buttonHoverSound;
    [SerializeField] private EventReference buttonSelectSound;
    [SerializeField] private EventReference buttonBackSound;

    private Button backToStartBtn;
    private Button backToGameBtn;
    private Button openOptionsMenuBtn;

    private AudioChannelUI[] audioChannels = AudioSliderBinder.DefaultChannels;
    private Dictionary<int, EventCallback<ChangeEvent<float>>> valueCallbacks = new();
    private Dictionary<int, EventCallback<PointerDownEvent>> clickCallbacks = new();

    public void OnEnable()
    {
        if (optionsMenuUIDoc == null) return;
        optionsUI = optionsMenuUIDoc.rootVisualElement;

        VisualElement optionsContainer = optionsUI.Q<VisualElement>("OptionsMenuContainer");
        if (optionsContainer != null)
        {
            optionsContainer.pickingMode = PickingMode.Position;
        }

        backToStartBtn = optionsUI.Q<Button>("BackToStart");
        backToGameBtn = optionsUI.Q<Button>("BackToGame");
        openOptionsMenuBtn = optionsUI.Q<Button>("OpenOptionsMenu");

        if (backToStartBtn != null)
        {
            backToStartBtn.RegisterCallback<PointerEnterEvent>(OnButtonHover);
            backToStartBtn.clicked += BackToStartMenu;
        }

        if (backToGameBtn != null)
        {
            backToGameBtn.RegisterCallback<PointerEnterEvent>(OnButtonHover);
            backToGameBtn.clicked += HideOptionsMenu;
        }

        if (openOptionsMenuBtn != null)
        {
            openOptionsMenuBtn.RegisterCallback<PointerEnterEvent>(OnButtonHover);
            openOptionsMenuBtn.clicked += ShowOptionsMenu;
        }

        audioChannels = AudioSliderBinder.Bind(
            optionsUI, audioChannels,
            buttonSelectSound, valueCallbacks, clickCallbacks);

    }

    public void OnDisable()
    {
        if (backToStartBtn != null)
        {
            backToStartBtn.UnregisterCallback<PointerEnterEvent>(OnButtonHover);
            backToStartBtn.clicked -= BackToStartMenu;
        }

        if (backToGameBtn != null)
        {
            backToGameBtn.UnregisterCallback<PointerEnterEvent>(OnButtonHover);
            backToGameBtn.clicked -= HideOptionsMenu;
        }

        if (openOptionsMenuBtn != null)
        {
            openOptionsMenuBtn.UnregisterCallback<PointerEnterEvent>(OnButtonHover);
            openOptionsMenuBtn.clicked -= ShowOptionsMenu;
        }

        AudioSliderBinder.Unbind(audioChannels, valueCallbacks, clickCallbacks);
    }

    public void ShowOptionsMenu()
    {
        AudioManager.Instance.PlaySound2D(buttonSelectSound);
        GameManager.Instance.optionsMenuOpen = true;
        VisualElement container = optionsUI.Q<VisualElement>("OptionsMenuContainer");
        if (container != null)
        {
            container.style.display = DisplayStyle.Flex;
            container.pickingMode = PickingMode.Position;
        }
    }

    public void BackToStartMenu()
    {
        GameManager.Instance.PrepareForBackToStart();
        AudioManager.Instance.PlaySound2D(buttonBackSound);
        AudioManager.Instance.SaveVolumeSettingsToDisk();
        GameManager.Instance.optionsMenuOpen = false;

        VisualElement container = optionsUI.Q<VisualElement>("OptionsMenuContainer");
        if (container != null)
        {
            container.style.display = DisplayStyle.None;
            container.pickingMode = PickingMode.Ignore;
        }
        SceneManager.LoadScene("Start", LoadSceneMode.Single);
    }

    public void HideOptionsMenu()
    {
        AudioManager.Instance.PlaySound2D(buttonBackSound);
        AudioManager.Instance.SaveVolumeSettingsToDisk();
        GameManager.Instance.optionsMenuOpen = false;
        VisualElement container = optionsUI.Q<VisualElement>("OptionsMenuContainer");
        if (container != null)
        {
            container.style.display = DisplayStyle.None;
            container.pickingMode = PickingMode.Ignore;
        }
    }

    public void OnButtonHover(PointerEnterEvent evt)
    {
        AudioManager.Instance.PlaySound2D(buttonHoverSound);
    }

}