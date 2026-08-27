using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using FMODUnity;
using System;

public class StartManager : MonoBehaviour
{
    [Serializable]
    public struct CreditEntry
    {
        public string jobTitle;
        public string personName;
    }

    [Serializable]
    public struct AudioChannelUI
    {
        public string channelName;
        public string sliderUxmlName;
        public string labelUxmlName;
        public AudioOptionSliders sliderType;

        internal Slider slider;
        internal Label label;
    }

    [Header("Credits Configuration Data")]
    [SerializeField] private VisualTreeAsset creditRowTemplate;

    private CreditEntry[] gameCredits = new CreditEntry[]
    {
        new CreditEntry { jobTitle = "Art", personName = "Ashleypox" },
        new CreditEntry { jobTitle = "Game Designer & UI/UX", personName = "Cat Preimesberger" },
        new CreditEntry { jobTitle = "Tech Audio & UI Programming", personName = "Clara Smith" },
        new CreditEntry { jobTitle = "Programming & Writing", personName = "IfThenCreate" },
        new CreditEntry { jobTitle = "Sound Design, Composition, Voice Acting, Audio Implementation", personName = "Jamie Billings" },
        new CreditEntry { jobTitle = "Project Management & Writing", personName = "Nathania Wong" },
        new CreditEntry { jobTitle = "Art Support", personName = "Widelczyna" }
    };

    public UIDocument startUIDoc;
    private VisualElement startUI;

    [Header("Audio Properties")]
    [SerializeField] private EventReference buttonHoverSound;
    [SerializeField] private EventReference startGameSound;
    [SerializeField] private EventReference genericClickSound;

    [Header("Decibel Slider Bounds")]
    [SerializeField] private float sliderLowValue = -60f;
    [SerializeField] private float sliderHighValue = 10f;

    private AudioChannelUI[] audioChannels = new AudioChannelUI[]
    {
        new AudioChannelUI { channelName = "Master",    sliderUxmlName = "SliderMain",     labelUxmlName = "MainValueLabel",     sliderType = AudioOptionSliders.MainVolume },
        new AudioChannelUI { channelName = "Music",     sliderUxmlName = "SliderMusic",    labelUxmlName = "MusicValueLabel",    sliderType = AudioOptionSliders.MusicVolume },
        new AudioChannelUI { channelName = "SFX",       sliderUxmlName = "SliderSFX",      labelUxmlName = "SFXValueLabel",      sliderType = AudioOptionSliders.SFXVolume },
        new AudioChannelUI { channelName = "Ambience",  sliderUxmlName = "SliderAmbience", labelUxmlName = "AmbienceValueLabel", sliderType = AudioOptionSliders.AmbientVolume },
        new AudioChannelUI { channelName = "Dialogue",  sliderUxmlName = "SliderDialogue", labelUxmlName = "DialogueValueLabel", sliderType = AudioOptionSliders.DialogueVolume }
    };


    //Scene containers
    private VisualElement mainMenuContainer;
    private VisualElement optionsMenuContainer;
    private VisualElement creditsMenuContainer;
    private VisualElement creditsListContainer;

    //cached buttons
    private Button startGameButton;
    private Button optionsButton;
    private Button creditsButton;
    private Button optionsBackToStartButton;
    private Button creditsBackButton;


    public void OnEnable()
    {
        startUI = startUIDoc.rootVisualElement;

        // Find containers
        mainMenuContainer = startUI.Q<VisualElement>("MainMenuContainer");
        optionsMenuContainer = startUI.Q<VisualElement>("OptionsMenuContainer");
        creditsMenuContainer = startUI.Q<VisualElement>("CreditsMenuContainer");
        creditsListContainer = startUI.Q<VisualElement>("CreditsList");

        // Find buttons
        startGameButton = startUI.Q<Button>("StartGame");
        optionsButton = startUI.Q<Button>("Options");
        creditsButton = startUI.Q<Button>("Credits");

        if (optionsMenuContainer != null)
        {
            optionsBackToStartButton = optionsMenuContainer.Q<Button>("BackToStart");
        }

        if (creditsMenuContainer != null)
        {
            creditsBackButton = creditsMenuContainer.Q<Button>("OptionsBackBtn");
        }

        // Setup Audio Sliders Loop
        for (int i = 0; i < audioChannels.Length; i++)
        {
            audioChannels[i].slider = startUI.Q<Slider>(audioChannels[i].sliderUxmlName);
            audioChannels[i].label = startUI.Q<Label>(audioChannels[i].labelUxmlName);

            if (audioChannels[i].slider != null)
            {
                audioChannels[i].slider.lowValue = sliderLowValue;
                audioChannels[i].slider.highValue = sliderHighValue;
                int cachedIndex = i;

                float savedVolume = GetSavedVolumeFromManager(audioChannels[cachedIndex].sliderType);

                audioChannels[i].slider.SetValueWithoutNotify(savedVolume);
                audioChannels[i].slider.RegisterValueChangedCallback(evt => OnVolumeSliderChanged(cachedIndex, evt.newValue));

                UpdateVolumeLabel(audioChannels[cachedIndex], audioChannels[cachedIndex].slider.value);
            }
        }

        // Setup Button Listeners
        if (startGameButton != null)
        {
            startGameButton.RegisterCallback<PointerEnterEvent>(OnButtonHover);
            startGameButton.clicked += StartGame;
        }

        if (optionsButton != null)
        {
            optionsButton.RegisterCallback<PointerEnterEvent>(OnButtonHover);
            optionsButton.clicked += OpenOptions;
        }

        if (creditsButton != null)
        {
            creditsButton.RegisterCallback<PointerEnterEvent>(OnButtonHover);
            creditsButton.clicked += OpenCredits;
        }

        if (optionsBackToStartButton != null)
        {
            optionsBackToStartButton.RegisterCallback<PointerEnterEvent>(OnButtonHover);
            optionsBackToStartButton.clicked += CloseSubMenus;
        }

        if (creditsBackButton != null)
        {
            creditsBackButton.RegisterCallback<PointerEnterEvent>(OnButtonHover);
            creditsBackButton.clicked += CloseSubMenus;
        }

        PopulateCreditsScreen();
    }


    public void OnDisable()
    {
        for (int i = 0; i < audioChannels.Length; i++)
        {
            if (audioChannels[i].slider != null)
            {
                int cachedIndex = i;
                audioChannels[i].slider.UnregisterValueChangedCallback(evt => OnVolumeSliderChanged(cachedIndex, evt.newValue));
            }
        }

        if (startGameButton != null)
        {
            startGameButton.UnregisterCallback<PointerEnterEvent>(OnButtonHover);
            startGameButton.clicked -= StartGame;
        }

        if (optionsButton != null)
        {
            optionsButton.UnregisterCallback<PointerEnterEvent>(OnButtonHover);
            optionsButton.clicked -= OpenOptions;
        }

        if (creditsButton != null)
        {
            creditsButton.UnregisterCallback<PointerEnterEvent>(OnButtonHover);
            creditsButton.clicked -= OpenCredits;
        }

        if (optionsBackToStartButton != null)
        {
            optionsBackToStartButton.UnregisterCallback<PointerEnterEvent>(OnButtonHover);
            optionsBackToStartButton.clicked -= CloseSubMenus;
        }

        if (creditsBackButton != null)
        {
            creditsBackButton.UnregisterCallback<PointerEnterEvent>(OnButtonHover);
            creditsBackButton.clicked -= CloseSubMenus;
        }
    }

    public void OnButtonHover(PointerEnterEvent evt)
    {
        // SOUND : a button in the start menu was hovered over
        AudioManager.Instance.PlaySound2D(buttonHoverSound);
    }

    public void StartGame()
    {
        // SOUND : the start game button was clicked
        AudioManager.Instance.PlaySound2D(startGameSound);
        SceneManager.LoadScene("Office", LoadSceneMode.Single);
    }

    public void OpenOptions()
    {
        AudioManager.Instance.PlaySound2D(genericClickSound);
        if (mainMenuContainer != null) mainMenuContainer.style.display = DisplayStyle.None;
        if (optionsMenuContainer != null) optionsMenuContainer.style.display = DisplayStyle.Flex;
        if (creditsMenuContainer != null) creditsMenuContainer.style.display = DisplayStyle.None;
    }

    public void OpenCredits()
    {
        AudioManager.Instance.PlaySound2D(genericClickSound);
        if (mainMenuContainer != null) mainMenuContainer.style.display = DisplayStyle.None;
        if (optionsMenuContainer != null) optionsMenuContainer.style.display = DisplayStyle.None;
        if (creditsMenuContainer != null) creditsMenuContainer.style.display = DisplayStyle.Flex;
    }

    public void CloseSubMenus()
    {
        AudioManager.Instance.PlaySound2D(genericClickSound);
        if (mainMenuContainer != null) mainMenuContainer.style.display = DisplayStyle.Flex;
        if (optionsMenuContainer != null) optionsMenuContainer.style.display = DisplayStyle.None;
        if (creditsMenuContainer != null) creditsMenuContainer.style.display = DisplayStyle.None;
    }

    private void OnVolumeSliderChanged(int channelIndex, float dbValue)
    {
        AudioChannelUI activeChannel = audioChannels[channelIndex];
        UpdateVolumeLabel(activeChannel, dbValue);

        AudioManager.Instance.UpdateAudioOptionsSlider(activeChannel.sliderType, dbValue);
    }

    private void UpdateVolumeLabel(AudioChannelUI channel, float dbValue)
    {
        if (channel.label == null) return;

        if (dbValue <= (sliderLowValue + 0.5f))
        {
            channel.label.text = "MUTED";
        }
        else
        {
            string sign = dbValue > 0 ? "+" : "";
            channel.label.text = $"{sign}{dbValue:F1} dB";
        }
    }

    private float GetSavedVolumeFromManager(AudioOptionSliders sliderType)
    {
        switch (sliderType)
        {
            case AudioOptionSliders.MainVolume: return PlayerPrefs.GetFloat("MAIN_VOL_KEY", 0f);
            case AudioOptionSliders.MusicVolume: return PlayerPrefs.GetFloat("MUSIC_VOL_KEY", 0f);
            case AudioOptionSliders.SFXVolume: return PlayerPrefs.GetFloat("SFX_VOL_KEY", 0f);
            case AudioOptionSliders.DialogueVolume: return PlayerPrefs.GetFloat("DIALOGUE_VOL_KEY", 0f);
            case AudioOptionSliders.AmbientVolume: return PlayerPrefs.GetFloat("AMBIENT_VOL_KEY", 0f);
            default: return 0f;
        }
    }

    private void PopulateCreditsScreen()
    {
        if (creditsListContainer == null || creditRowTemplate == null) return;

        creditsListContainer.Clear();

        for (int i = 0; i < gameCredits.Length; i++)
        {
            VisualElement rowInstance = creditRowTemplate.Instantiate();

            Label jobLabel = rowInstance.Q<Label>("JobTitle");
            Label nameLabel = rowInstance.Q<Label>("CreditName");

            if (jobLabel != null) jobLabel.text = gameCredits[i].jobTitle;
            if (nameLabel != null) nameLabel.text = gameCredits[i].personName;

            creditsListContainer.Add(rowInstance);
        }
    }

}

