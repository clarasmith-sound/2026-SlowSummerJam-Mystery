using PrimeTween;
using UnityEngine;
using System;
using FMODUnity;
using FMOD.Studio;

public class PhoneController : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private EventReference phoneRingEvent;
    private EventInstance phoneRingEventInstance;
    private SpriteRenderer spriteRenderer;
    private Sequence ringingAnimation;

    public void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (phoneRingEvent.IsNull) return;

        if (phoneRingEventInstance.isValid())
        {
            phoneRingEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            phoneRingEventInstance.release();
        }

        phoneRingEventInstance = RuntimeManager.CreateInstance(phoneRingEvent);

        RuntimeManager.AttachInstanceToGameObject(phoneRingEventInstance, gameObject);
    }

    private void OnMouseEnter()
    {
        if (GameManager.Instance != null && GameManager.Instance.optionsMenuOpen == true) return;
        spriteRenderer.material.SetFloat("_Toggle", 1.0f);
    }

    private void OnMouseExit()
    {
        spriteRenderer.material.SetFloat("_Toggle", 0f);
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance != null && GameManager.Instance.optionsMenuOpen == true) return;
        spriteRenderer.material.SetFloat("_Toggle", 0f);
        ringingAnimation.Stop();

        GameManager.Instance.PhonePickedUp();

        if (phoneRingEventInstance.isValid())
        {
            phoneRingEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            phoneRingEventInstance.release();
        }
    }

    public void StartPhoneRinging()
    {
        phoneRingEventInstance.start();
        AudioManager.Instance.PlayLoopingSound(phoneRingEvent, out phoneRingEventInstance);

        ringingAnimation = Sequence.Create(cycles: -1, Sequence.SequenceCycleMode.Yoyo)
            .Group(Tween.ShakeScale(gameObject.transform, strength: new Vector3(.1f, .1f, .1f), duration: 0.3f));
    }
}
