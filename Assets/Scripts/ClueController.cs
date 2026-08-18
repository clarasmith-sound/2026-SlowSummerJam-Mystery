using UnityEngine;

public class ClueController : MonoBehaviour
{
    public SuspectController suspect;
    [HideInInspector] public int clueIndex;

    private void OnMouseDown()
    {
        if (suspect.state != SuspectState.Inspection) return;
        suspect.suspectData.clues[clueIndex].discovered = true;
        GameManager.Instance.RunDialogue(suspect.suspectData.clues[clueIndex].yarnDialogueNode);
        GameManager.Instance.CheckAllCluesDiscovered();
    }
}
