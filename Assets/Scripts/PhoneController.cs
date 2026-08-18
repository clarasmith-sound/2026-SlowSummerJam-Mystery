using PrimeTween;
using UnityEngine;

public class PhoneController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private bool isPhoneRinging = false;
    private Sequence ringingAnimation;

    public void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseEnter()
    {
        if (!isPhoneRinging) return;
        else
        {
            spriteRenderer.material.SetFloat("_Toggle", 1.0f);
        }
    }

    private void OnMouseExit()
    {
        spriteRenderer.material.SetFloat("_Toggle", 0f);
    }

    private void OnMouseDown()
    {
        if (!isPhoneRinging) return;
        spriteRenderer.material.SetFloat("_Toggle", 0f);
        ringingAnimation.Stop();
        isPhoneRinging = false;
        GameManager.Instance.PhonePickedUp();
    }

    public void StartPhoneRinging()
    {
        isPhoneRinging = true;
        // TODO: phone ringing sound effect
        ringingAnimation = Sequence.Create(cycles: -1, Sequence.SequenceCycleMode.Yoyo)
            .Group(Tween.ShakeScale(gameObject.transform, strength: new Vector3(.1f, .1f, .1f), duration: 0.3f));
    }
}
