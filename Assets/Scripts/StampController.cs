using UnityEngine;

public class StampController : MonoBehaviour
{
    public GameObject StampNotHeld;
    public GameObject StampHeld;

    public GameObject StampBase;
    private bool stampPickedUp = false;

    private void OnMouseEnter()
    {
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
            StampNotHeld.SetActive(false);
            StampHeld.SetActive(true);
            stampPickedUp = true;
            StampBase.GetComponent<SpriteRenderer>().material.SetFloat("_Toggle", 1.0f);
        }
        else if (stampPickedUp)
        {
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
