using UnityEngine;

public class StampController : MonoBehaviour
{
    public GameObject StampNotHeld;
    public GameObject StampHeld;

    public GameObject StampBase;
    private bool stampPickedUp = false;

    private void OnMouseEnter()
    {
        // TODO - SOUND :  The mouse is hovering over the stamp. If (!stampPickedUp), the stamp is not currently being held,
        // so the whole thing is outlined to indicate pickup. If (stampPickedUp), the stamp is already being held, 
        // and the base is outlined to indicate putting the stamp back. 
        if (!stampPickedUp)
        {
            StampNotHeld.GetComponent<SpriteRenderer>().material.SetFloat("_Toggle", 1.0f);
        }
        else if (stampPickedUp)
        {
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
        if (!stampPickedUp)
        {
            if (!GameManager.Instance.PrepareToStamp()) return;
            // TODO - SOUND :  The stamp was picked up
            StampNotHeld.SetActive(false);
            StampHeld.SetActive(true);
            stampPickedUp = true;
            StampBase.GetComponent<SpriteRenderer>().material.SetFloat("_Toggle", 1.0f);
        }
        else if (stampPickedUp)
        {
            // TODO - SOUND :  The stamp was put down
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
