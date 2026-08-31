using UnityEngine;
using FMODUnity;

public class StampController : MonoBehaviour
{
    public GameObject StampNotHeld;
    public GameObject StampHeld;

    public GameObject StampBase;
    private bool stampPickedUp = false;

    [Header("Audio")]
    [SerializeField] private EventReference stampHoverSound;
    [SerializeField] private EventReference stampPickupSound;
    [SerializeField] private EventReference stampPutDownSound;

    private void OnMouseEnter()
    {
        if(GameManager.Instance != null && GameManager.Instance.optionsMenuOpen == true || GameManager.Instance.inspectionInProgress == true || GameManager.Instance.currentCaseIndex == -1 ) return;
        // SOUND :  The mouse is hovering over the stamp. If (!stampPickedUp), the stamp is not currently being held,
        // so the whole thing is outlined to indicate pickup. If (stampPickedUp), the stamp is already being held, 
        // and the base is outlined to indicate putting the stamp back. 
        if (!stampPickedUp)
        {
            AudioManager.Instance.PlaySound2D(stampHoverSound);
            StampNotHeld.GetComponent<SpriteRenderer>().material.SetFloat("_Toggle", 1.0f);
        }
        else if (stampPickedUp)
        {
            AudioManager.Instance.PlaySound2D(stampHoverSound);
            StampBase.GetComponent<SpriteRenderer>().material.SetFloat("_Toggle", 1.0f);
        }
    }

    private void OnMouseExit()
    {
        if (!stampPickedUp)
        {
            StampNotHeld.GetComponent<SpriteRenderer>().material.SetFloat("_Toggle", 0f);
        }
        else if (stampPickedUp)
        {
            StampBase.GetComponent<SpriteRenderer>().material.SetFloat("_Toggle", 0f);
        }
    }

    private void OnMouseDown()
    {
        if(GameManager.Instance != null && GameManager.Instance.optionsMenuOpen == true || GameManager.Instance.inspectionInProgress == true|| GameManager.Instance.currentCaseIndex == -1) return;
        if (!stampPickedUp)
        {
            if (!GameManager.Instance.PrepareToStamp()) return;
            // SOUND :  The stamp was picked up
            AudioManager.Instance.PlaySound2D(stampPickupSound);
            StampNotHeld.SetActive(false);
            StampHeld.SetActive(true);
            stampPickedUp = true;
            StampBase.GetComponent<SpriteRenderer>().material.SetFloat("_Toggle", 1.0f);
        }
        else if (stampPickedUp)
        {
            // SOUND :  The stamp was put down
            AudioManager.Instance.PlaySound2D(stampPutDownSound);
            GameManager.Instance.PutDownStamp();
            StampNotHeld.GetComponent<SpriteRenderer>().material.SetFloat("_Toggle", 1f);
            StampBase.GetComponent<SpriteRenderer>().material.SetFloat("_Toggle", 0f);
        }
    }

    public void PutDownStamp()
    {
        StampNotHeld.SetActive(true);
        StampHeld.SetActive(false);
        stampPickedUp = false;
        StampNotHeld.GetComponent<SpriteRenderer>().material.SetFloat("_Toggle", 0f);
        StampBase.GetComponent<SpriteRenderer>().material.SetFloat("_Toggle", 0f);
    }
}
