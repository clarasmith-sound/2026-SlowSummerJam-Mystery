using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class PhoneAudioController : MonoBehaviour
{
    [Header("FMOD Settings")]
    [SerializeField] private EventReference phoneRingEvent;

    private EventInstance phoneRingEventInstance;

    void Start()
    {
        if (phoneRingEvent.IsNull) return;

        if (phoneRingEventInstance.isValid())
        {
            phoneRingEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            phoneRingEventInstance.release();
        }
    
        phoneRingEventInstance = RuntimeManager.CreateInstance(phoneRingEvent);
        
        RuntimeManager.AttachInstanceToGameObject(phoneRingEventInstance, gameObject);
    }
    private void OnEnable()
    {
        // Listen to the phone's event
        PhoneController.OnPhoneRingStateChanged += HandlePhoneRingStateChanged;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        PhoneController.OnPhoneRingStateChanged -= HandlePhoneRingStateChanged;
    }

    private void HandlePhoneRingStateChanged(bool isRinging)
    {
        if (isRinging)
        {
            phoneRingEventInstance.start();
            AudioManager.Instance.PlayLoopingSound(phoneRingEvent, out phoneRingEventInstance);
        }
        else
        {
            if (phoneRingEventInstance.isValid())
            {
                phoneRingEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                phoneRingEventInstance.release();
            }
        }
    }

}
