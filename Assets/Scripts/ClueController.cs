using UnityEngine;
using FMODUnity;

public class ClueController : MonoBehaviour
{
    public SuspectController suspect;
    [HideInInspector] public int clueIndex;

    [Header("Audio")]
    [SerializeField] private string clueSelectSound = "event:/Objects/Clue_Select";


    private void OnMouseDown()
    {
        if (suspect.state != SuspectState.Inspection) return;
        // SOUND :  Using the monocle in inspect mode, a clue was clicked. 
        if (string.IsNullOrEmpty(clueSelectSound))
        {
            clueSelectSound = "event:/Objects/Clue_Select";
        }
        AudioManager.Instance.PlaySound2D(clueSelectSound);
        suspect.suspectData.clues[clueIndex].discovered = true;
        GameManager.Instance.RunDialogue(suspect.suspectData.clues[clueIndex].yarnDialogueNode);
        GameManager.Instance.CheckAllCluesDiscovered();
    }
}
