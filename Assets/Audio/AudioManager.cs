using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Dynamic;

public enum AudioUIButtonType { Press, Hover, StartGame }

public enum AudioOptionSliders { MainVolume, MusicVolume, SFXVolume, DialogueVolume, AmbientVolume }

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float mainVolume = 1.0f;
    [Range(0f, 1f)] public float musicVolume = 1.0f;
    [Range(0f, 1f)] public float sfxVolume = 1.0f;
    [Range(0f, 1f)] public float dialogueVolume = 1.0f;
    [Range(0f, 1f)] public float ambientVolume = 1.0f;

    [Header("Slider Bounds")]
    public float SliderLowValue = -60f;
    public float SliderHighValue = 10f;
    public float SliderDefaultValue = 0f;

    private Bus mainBus;
    private Bus musicBus;
    private Bus sfxBus;
    private Bus dialogueBus;
    private Bus ambientBus;

    private EventInstance musicEventInstance;
    private EventReference currentMusicReference;

    private const string MAIN_VOL_KEY = "Audio_MainVolume";
    private const string MUSIC_VOL_KEY = "Audio_MusicVolume";
    private const string SFX_VOL_KEY = "Audio_SFXVolume";
    private const string DIALOGUE_VOL_KEY = "Audio_DialogueVolume";
    private const string AMBIENT_VOL_KEY = "Audio_AmbientVolume";
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeOnStartup()
    {
        GameObject audioManagerObject = new GameObject("AudioManager");
        Instance = audioManagerObject.AddComponent<AudioManager>();
        DontDestroyOnLoad(audioManagerObject);
    }

    private void Awake()
    {

        StartCoroutine(WaitForBanksThenSetup());
    }

    private System.Collections.IEnumerator WaitForBanksThenSetup()
    {
        while (!FMODUnity.RuntimeManager.HaveAllBanksLoaded)
            yield return null;

        mainBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
        dialogueBus = RuntimeManager.GetBus("bus:/Dialogue");
        ambientBus = RuntimeManager.GetBus("bus:/Ambience");

        mainVolume = PlayerPrefs.GetFloat(MAIN_VOL_KEY, mainVolume);
        musicVolume = PlayerPrefs.GetFloat(MUSIC_VOL_KEY, musicVolume);
        sfxVolume = PlayerPrefs.GetFloat(SFX_VOL_KEY, sfxVolume);
        dialogueVolume = PlayerPrefs.GetFloat(DIALOGUE_VOL_KEY, dialogueVolume);
        ambientVolume = PlayerPrefs.GetFloat(AMBIENT_VOL_KEY, ambientVolume);

        SetBusVolume(mainBus, mainVolume);
        SetBusVolume(musicBus, musicVolume);
        SetBusVolume(sfxBus, sfxVolume);
        SetBusVolume(dialogueBus, dialogueVolume);
        SetBusVolume(ambientBus, ambientVolume);
    }

    public void PlaySound2D(EventReference eventReference)
    {
        if (eventReference.IsNull)
        {
            Debug.LogWarning("EventReference is null. Cannot play sound.");
            return;
        }
        RuntimeManager.PlayOneShot(eventReference);
    }

    public void PlaySound2D(string eventPath)
    {
        if (string.IsNullOrEmpty(eventPath))
        {
            Debug.LogWarning("Event path is null or empty. Cannot play sound.");
            return;
        }
        EventReference eventReference = RuntimeManager.PathToEventReference(eventPath);
        if (eventReference.IsNull)
        {
            Debug.LogWarning($"EventReference for path '{eventPath}' is null. Cannot play sound.");
            return;
        }
        PlaySound2D(eventReference);
    }


    public void PlayLoopingSound(EventReference eventReference, out EventInstance eventInstance)
    {
        eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstance.start();
    }

    public void PlayMusic(EventReference eventReference)
    {
        if (eventReference.IsNull) return;

        StopMusic();

        musicEventInstance = RuntimeManager.CreateInstance(eventReference);
        musicEventInstance.start();
        currentMusicReference = eventReference;
    }

    public void StopMusic()
    {
        if (musicEventInstance.isValid())
        {
            EventInstance oldInstance = musicEventInstance;
            oldInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            oldInstance.release();
            musicEventInstance = default;
            currentMusicReference = default;
        }
    }

    public void SetInstanceParameter(EventInstance instance, string parameterName, float value)
    {
        if (instance.isValid())
        {
            instance.setParameterByName(parameterName, value);
        }
        else
        {
            Debug.LogWarning($"Cannot set parameter '{parameterName}'. The sound instance is not valid or has stopped.");
        }
    }

    public void SetGlobalParameter(string parameterName, float value)
    {
        RuntimeManager.StudioSystem.setParameterByName(parameterName, value);
    }

    public void UpdateAudioOptionsSlider(AudioOptionSliders sliderType, float value)
    {
        float clampedValue = Mathf.Clamp(value, -60.0f, 10.0f); // Ensure the value is strictly between 0 and 1

        switch (sliderType)
        {
            case AudioOptionSliders.MainVolume:
                mainVolume = clampedValue;
                SetBusVolume(mainBus, mainVolume);
                PlayerPrefs.SetFloat(MAIN_VOL_KEY, mainVolume);
                break;
            case AudioOptionSliders.MusicVolume:
                musicVolume = clampedValue;
                SetBusVolume(musicBus, musicVolume);
                PlayerPrefs.SetFloat(MUSIC_VOL_KEY, musicVolume);
                break;
            case AudioOptionSliders.SFXVolume:
                sfxVolume = clampedValue;
                SetBusVolume(sfxBus, sfxVolume);
                PlayerPrefs.SetFloat(SFX_VOL_KEY, sfxVolume);
                break;
            case AudioOptionSliders.DialogueVolume:
                dialogueVolume = clampedValue;
                SetBusVolume(dialogueBus, dialogueVolume);
                PlayerPrefs.SetFloat(DIALOGUE_VOL_KEY, dialogueVolume);
                break;
            case AudioOptionSliders.AmbientVolume:
                ambientVolume = clampedValue;
                SetBusVolume(ambientBus, ambientVolume);
                PlayerPrefs.SetFloat(AMBIENT_VOL_KEY, ambientVolume);
                break;
            default:
                Debug.LogWarning("Unknown audio option slider type.");
                break;
        }
    }

    private void SetBusVolume(Bus targetBus, float inDb)
    {
        if (!targetBus.isValid()) return;

        float linear = Mathf.Pow(10,(inDb/20));

        targetBus.setVolume(linear);
    }

    public void SaveVolumeSettingsToDisk()
    {
        PlayerPrefs.Save();
    }
    public float GetVolume(AudioOptionSliders sliderType)
    {
        switch (sliderType)
        {
            case AudioOptionSliders.MainVolume: return mainVolume;
            case AudioOptionSliders.MusicVolume: return musicVolume;
            case AudioOptionSliders.SFXVolume: return sfxVolume;
            case AudioOptionSliders.DialogueVolume: return dialogueVolume;
            case AudioOptionSliders.AmbientVolume: return ambientVolume;
            default: return 0f;
        }
    }

    public void SetVolume(AudioOptionSliders sliderType, float dbValue)
    {
        UpdateAudioOptionsSlider(sliderType, dbValue);
    }
}
