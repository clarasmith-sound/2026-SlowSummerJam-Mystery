using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioSceneManager : MonoBehaviour
{
    [Header("Ambience")]
    [SerializeField] private EventReference ambienceSound;

    private EventInstance ambienceEventInstance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (ambienceSound.IsNull) return;

        if (ambienceEventInstance.isValid())
        {
            ambienceEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            ambienceEventInstance.release();
        }
            
        ambienceEventInstance = RuntimeManager.CreateInstance(ambienceSound);
        ambienceEventInstance.start();

        RuntimeManager.AttachInstanceToGameObject(ambienceEventInstance, gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        if (ambienceEventInstance.isValid())
        {
            ambienceEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            ambienceEventInstance.release();
        }
    }
}
