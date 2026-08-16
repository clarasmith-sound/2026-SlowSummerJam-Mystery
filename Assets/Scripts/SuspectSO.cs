using UnityEngine;

[CreateAssetMenu(fileName = "SuspectSO", menuName = "Scriptable Objects/SuspectSO")]
public class SuspectSO : ScriptableObject
{
    public string suspectName;
    [SerializeField] public Clue[] clues;
}
