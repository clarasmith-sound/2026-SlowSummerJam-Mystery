using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public enum AudioUIButtonType { Press, Hover, StartGame }

public enum AudioOptionSliders { MainVolume, MusicVolume, SFXVolume, DialogueVolume, AmbientVolume }

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("UI Sounds")]
    public EventReference uiButtonPress;
    public EventReference uiButtonHover;
    public EventReference uiButtonStartGame;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float mainVolume = 1.0f;
    [Range(0f, 1f)] public float musicVolume = 1.0f;
    [Range(0f, 1f)] public float sfxVolume = 1.0f;
    [Range(0f, 1f)] public float dialogueVolume = 1.0f;
    [Range(0f, 1f)] public float ambientVolume = 1.0f;

    private Bus mainBus;
    private Bus musicBus;
    private Bus sfxBus;
    private Bus dialogueBus;
    private Bus ambientBus;

    private EventInstance musicEventInstance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeOnStartup()
    {
        GameObject audioManagerObject = new GameObject("AudioManager");
        Instance = audioManagerObject.AddComponent<AudioManager>();
        DontDestroyOnLoad(audioManagerObject);
    }

    private void Awake()
    {
        mainBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
        dialogueBus = RuntimeManager.GetBus("bus:/Dialogue");
        ambientBus = RuntimeManager.GetBus("bus:/Ambience");
    }

    private void Update()
    {
        if (mainBus.isValid()) mainBus.setVolume(mainVolume);
        if (musicBus.isValid()) musicBus.setVolume(musicVolume);
        if (sfxBus.isValid()) sfxBus.setVolume(sfxVolume);
        if (dialogueBus.isValid()) dialogueBus.setVolume(dialogueVolume);
        if (ambientBus.isValid()) ambientBus.setVolume(ambientVolume);
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

    public void PlayUISound(AudioUIButtonType buttonType)
    {
        switch (buttonType)
        {
            case AudioUIButtonType.Press:
                PlaySound2D(uiButtonPress);
                break;
            case AudioUIButtonType.Hover:
                PlaySound2D(uiButtonHover);
                break;
            case AudioUIButtonType.StartGame:
                PlaySound2D(uiButtonStartGame);
                break;
            default:
                Debug.LogWarning("Unknown UI button type.");
                break;
        }
    }

    public void PlayLoopingSound(EventReference eventReference, out EventInstance eventInstance)
    {
        eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstance.start();
    }

    public void PlayMusic(EventReference eventReference)
    {
        if (eventReference.IsNull) return;

        if (musicEventInstance.isValid())
        {
            musicEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            musicEventInstance.release();
        }

        musicEventInstance = RuntimeManager.CreateInstance(eventReference);
        musicEventInstance.start();
    }

    public void StopMusic()
    {
        if (musicEventInstance.isValid())
        {
            musicEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            musicEventInstance.release();
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
        float clampedValue = Mathf.Clamp01(value); // Ensure the value is between 0 and 1
        switch (sliderType)
        {
            case AudioOptionSliders.MainVolume:
                mainVolume = clampedValue;
                break;
            case AudioOptionSliders.MusicVolume:
                musicVolume = clampedValue;
                break;
            case AudioOptionSliders.SFXVolume:
                sfxVolume = clampedValue;
                break;
            case AudioOptionSliders.DialogueVolume:
                dialogueVolume = clampedValue;
                break;
            case AudioOptionSliders.AmbientVolume:
                ambientVolume = clampedValue;
                break;
            default:
                Debug.LogWarning("Unknown audio option slider type.");
                break;
        }
    }
}
