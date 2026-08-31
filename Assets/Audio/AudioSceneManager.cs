using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;

public class AudioSceneManager : MonoBehaviour
{
    [Header("Ambience")]
    [SerializeField] private EventReference ambienceSound;
    [SerializeField] private EventReference musicEvent;

    private EventInstance ambienceEventInstance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(WaitForBanksThenPlay());
    }

    private IEnumerator WaitForBanksThenPlay()
    {
        while (!RuntimeManager.HaveAllBanksLoaded)
        {
            yield return null;
        }

        if(!musicEvent.IsNull)
        {
            PlayMusic();
        }
        else if (!ambienceSound.IsNull)
        {
            PlayAmbience();
        }
    }

    private void PlayAmbience()
    {
        if (ambienceEventInstance.isValid())
        {
            ambienceEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            ambienceEventInstance.release();
        }
            
        ambienceEventInstance = RuntimeManager.CreateInstance(ambienceSound);
        ambienceEventInstance.start();

        RuntimeManager.AttachInstanceToGameObject(ambienceEventInstance, gameObject);
    }

    private void PlayMusic()
    {
        AudioManager.Instance.PlayMusic(musicEvent);
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
