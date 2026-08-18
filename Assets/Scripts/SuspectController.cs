using System.Collections.Generic;
using UnityEngine;
public enum SuspectState { Ready, Hover, Inspection, Blurred, Judged };

public class SuspectController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public SuspectState state = SuspectState.Ready;
    public SuspectSO suspectOrigin; // Unmodified scriptable object, for comparisons
    [HideInInspector] public SuspectSO suspectData;
    private readonly List<GameObject> clueObjects = new();
    public GameObject expelledStamp;

    void Start()
    {
        suspectData = Instantiate(suspectOrigin);
        spriteRenderer = GetComponent<SpriteRenderer>();
        expelledStamp.SetActive(false);
        int currClueIndex = 0; // This requires the order of the Clue game objects to be in the same order as they're defined
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Clue"))
            {
                clueObjects.Add(child.gameObject);
                child.gameObject.SetActive(false);
                child.GetComponent<ClueController>().clueIndex = currClueIndex;
                currClueIndex++;
            }
        }
    }

    public void HighlightKid()
    {
        // Only valid if we're Ready or ReadyToStamp
        if (state != SuspectState.Ready) return;
        spriteRenderer.material.SetFloat("_Toggle", 1.0f);
        state = SuspectState.Hover;
    }

    public void UnhighlightKid()
    {
        // Only valid if the kid is already being hovered 
        if (state != SuspectState.Hover) return;
        spriteRenderer.material.SetFloat("_Toggle", 0f);
        state = SuspectState.Ready;
    }

    private void OnMouseEnter()
    {
        HighlightKid();
        if (state == SuspectState.Ready && GameManager.Instance != null && !GameManager.Instance.stampBeingHeld)
            GameManager.Instance.PlaySuspectHoverSound();
    }

    private void OnMouseExit()
    {
        UnhighlightKid();
    }

    private void OnMouseDown()
    {
        if (state != SuspectState.Hover) return;
        spriteRenderer.material.SetFloat("_Toggle", 0f);
        if (!GameManager.Instance.stampBeingHeld)
        {
            state = SuspectState.Inspection;
            GameManager.Instance.StartInspection(gameObject);
            foreach (GameObject clueObject in clueObjects)
                clueObject.SetActive(true);
        }
        else
        {
            expelledStamp.SetActive(true);
            GameManager.Instance.StampedGuilty(this);
        }
    }

    private void HideAllClues()
    {
        foreach (GameObject clueObject in clueObjects)
            clueObject.SetActive(false);
    }

    public void RestoreToReady()
    {
        HideAllClues();
        state = SuspectState.Ready;
    }
}
