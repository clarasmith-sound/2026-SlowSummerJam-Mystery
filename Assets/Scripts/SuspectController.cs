using System.Reflection.Metadata.Ecma335;
using UnityEngine;
public enum SuspectState { Ready, Hover, Focus, Blurred };

public class SuspectController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public SuspectState state = SuspectState.Ready;
    public SuspectSO suspectData;
    public GameObject[] clueObjects; // TODO: get these automatically? Or generate?

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void HighlightKid(bool hover)
    {
        // Only valid if we're entering or leaving hover state
        if (hover && state != SuspectState.Ready || !hover && state != SuspectState.Hover) return;
        spriteRenderer.material.SetFloat("_Toggle", hover ? 1.0f : 0f);
        state = hover ? SuspectState.Hover : SuspectState.Ready;
    }

    private void OnMouseEnter()
    {
        HighlightKid(true);
    }

    private void OnMouseExit()
    {
        HighlightKid(false);
    }

    private void OnMouseDown()
    {
        if (state != SuspectState.Hover) return;
        spriteRenderer.material.SetFloat("_Toggle", 0f);
        state = SuspectState.Focus;
        GameManager.Instance.StartInspection(gameObject);
        foreach (GameObject clueObject in clueObjects)
            clueObject.SetActive(true);
    }

    public void RestoreToReady()
    {
        foreach (GameObject clueObject in clueObjects)
            clueObject.SetActive(false);
        state = SuspectState.Ready;
    }
}
