using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using FMODUnity;
using System;

public class OptionsMenuVisualManager : MonoBehaviour
{
    public UIDocument optionsMenuUIDoc;
    private VisualElement optionsUI;

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
    
    [Header("Audio Properties")]
    [SerializeField] private EventReference buttonHoverSound;
    [SerializeField] private EventReference buttonSelectSound;
    [SerializeField] private EventReference buttonBackSound;

    [Header("Decibel Slider Bounds")]
    [SerializeField] private float sliderLowValue = -60f;
    [SerializeField] private float sliderHighValue = 10f;

    private Button backToStartBtn;
    private Button backToGameBtn;
    private Button openOptionsMenuBtn;

    private AudioChannelUI[] audioChannels = new AudioChannelUI[]
    {
        new AudioChannelUI { channelName = "Master",    sliderUxmlName = "SliderMain",     labelUxmlName = "MainValueLabel",     sliderType = AudioOptionSliders.MainVolume },
        new AudioChannelUI { channelName = "Music",     sliderUxmlName = "SliderMusic",    labelUxmlName = "MusicValueLabel",    sliderType = AudioOptionSliders.MusicVolume },
        new AudioChannelUI { channelName = "SFX",       sliderUxmlName = "SliderSFX",      labelUxmlName = "SFXValueLabel",      sliderType = AudioOptionSliders.SFXVolume },
        new AudioChannelUI { channelName = "Ambience",  sliderUxmlName = "SliderAmbience", labelUxmlName = "AmbienceValueLabel", sliderType = AudioOptionSliders.AmbientVolume },
        new AudioChannelUI { channelName = "Dialogue",  sliderUxmlName = "SliderDialogue", labelUxmlName = "DialogueValueLabel", sliderType = AudioOptionSliders.DialogueVolume }
    };

    public void OnEnable()
    {
        if (optionsMenuUIDoc == null) return;
        optionsUI = optionsMenuUIDoc.rootVisualElement;

        VisualElement optionsContainer = optionsUI.Q<VisualElement>("OptionsMenuContainer");
        if (optionsContainer != null)
        {
            optionsContainer.pickingMode = PickingMode.Position;
        }

        // Query Button References Safely
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

        for(int i=0; i < audioChannels.Length; i++)
        {
            audioChannels[i].slider = optionsUI.Q<Slider>(audioChannels[i].sliderUxmlName);
            audioChannels[i].label = optionsUI.Q<Label>(audioChannels[i].labelUxmlName);

            if(audioChannels[i].slider != null)
            {
                audioChannels[i].slider.lowValue = sliderLowValue;
                audioChannels[i].slider.highValue = sliderHighValue;
                int cachedIndex = i; 

                float savedLinearValue = GetSavedLinearVolumeFromManager(audioChannels[cachedIndex].sliderType);
                float savedDbValue = LerpUnclamped(sliderLowValue, sliderHighValue, savedLinearValue);

                audioChannels[i].slider.SetValueWithoutNotify(savedDbValue);
                audioChannels[i].slider.RegisterValueChangedCallback(evt => OnVolumeSliderChanged(cachedIndex, evt.newValue));
            
                UpdateVolumeLabel(audioChannels[cachedIndex], audioChannels[cachedIndex].slider.value);
            }
        }
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

        for (int i = 0; i < audioChannels.Length; i++)
        {
            if (audioChannels[i].slider != null)
            {
                int cachedIndex = i;
                audioChannels[i].slider.UnregisterValueChangedCallback(evt => OnVolumeSliderChanged(cachedIndex, evt.newValue));
            }
        }
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
        AudioManager.Instance.PlaySound2D(buttonBackSound);
        AudioManager.Instance.SaveVolumeSettingsToDisk();
        GameManager.Instance.optionsMenuOpen = false;

        VisualElement container = optionsUI.Q<VisualElement>("OptionsMenuContainer");
        if (container != null){
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
        if (container != null){
             container.style.display = DisplayStyle.None;
             container.pickingMode = PickingMode.Ignore;
        }
    }

    public void OnButtonHover(PointerEnterEvent evt)
    {
        AudioManager.Instance.PlaySound2D(buttonHoverSound);
    }

    private void OnVolumeSliderChanged(int channelIndex, float dbValue)
    {
        AudioChannelUI activeChannel = audioChannels[channelIndex];
        UpdateVolumeLabel(activeChannel, dbValue);

        float normalizedValue = InverseLerpUnclamped(sliderLowValue, sliderHighValue, dbValue);
        AudioManager.Instance.UpdateAudioOptionsSlider(activeChannel.sliderType, normalizedValue);
    }

    private float InverseLerpUnclamped(float low, float high, float value)
    {
        if (Mathf.Approximately(low, high)) return 0f;
        return (value - low) / (high - low);
    }

    private void UpdateVolumeLabel(AudioChannelUI channel, float dbValue)
    {
        if(channel.label == null) return;

        if(dbValue <= (sliderLowValue + 0.5f))
        {
            channel.label.text = "MUTED";
        }
        else
        {
            string sign = dbValue > 0 ? "+" : "";
            channel.label.text = $"{sign}{dbValue:F1} dB";
        }
    }

    private float GetSavedLinearVolumeFromManager(AudioOptionSliders sliderType)
    {
        switch (sliderType)
        {
            case AudioOptionSliders.MainVolume:     return PlayerPrefs.GetFloat("MAIN_VOL_KEY", 0.8f); 
            case AudioOptionSliders.MusicVolume:    return PlayerPrefs.GetFloat("MUSIC_VOL_KEY", 0.8f);
            case AudioOptionSliders.SFXVolume:      return PlayerPrefs.GetFloat("SFX_VOL_KEY", 0.8f);
            case AudioOptionSliders.DialogueVolume: return PlayerPrefs.GetFloat("DIALOGUE_VOL_KEY", 1.0f);
            case AudioOptionSliders.AmbientVolume:  return PlayerPrefs.GetFloat("AMBIENT_VOL_KEY", 0.5f);
            default:                                return 1.0f;
        }
    }

    private float LerpUnclamped(float low, float high, float interpolationFactor)
    {
        return low + (high - low) * interpolationFactor;
    }
}
