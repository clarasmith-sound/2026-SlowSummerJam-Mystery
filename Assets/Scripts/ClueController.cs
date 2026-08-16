using UnityEngine;

public class ClueController : MonoBehaviour
{
    public SuspectController suspect;
    public int clueIndex; // TODO: replace with something less brittle?

    private void OnMouseDown()
    {
        if (suspect.state != SuspectState.Focus) return;
        suspect.suspectData.clues[clueIndex].discovered = true;
    }
}
