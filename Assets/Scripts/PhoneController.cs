using PrimeTween;
using UnityEngine;
using System;

public class PhoneController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Sequence ringingAnimation;

    // Keep only this one property to manage the state everywhere
    public bool IsRinging { get; private set; } = false;
    public static event Action<bool> OnPhoneRingStateChanged;

    public void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseEnter()
    {
        if(GameManager.Instance != null && GameManager.Instance.optionsMenuOpen == true) return;
        if (!IsRinging) return;
        spriteRenderer.material.SetFloat("_Toggle", 1.0f);
    }

    private void OnMouseExit()
    {
        spriteRenderer.material.SetFloat("_Toggle", 0f);
    }

    private void OnMouseDown()
    {
        if(GameManager.Instance != null && GameManager.Instance.optionsMenuOpen == true) return;
        if (!IsRinging) return;

        spriteRenderer.material.SetFloat("_Toggle", 0f);
        ringingAnimation.Stop();

        IsRinging = false;
        OnPhoneRingStateChanged?.Invoke(false);

        GameManager.Instance.PhonePickedUp();
    }

    public void StartPhoneRinging()
    {
        if (IsRinging) return;

        IsRinging = true;
        OnPhoneRingStateChanged?.Invoke(true);

        ringingAnimation = Sequence.Create(cycles: -1, Sequence.SequenceCycleMode.Yoyo)
            .Group(Tween.ShakeScale(gameObject.transform, strength: new Vector3(.1f, .1f, .1f), duration: 0.3f));
    }
}
