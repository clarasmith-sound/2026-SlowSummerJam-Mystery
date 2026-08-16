using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

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
}
