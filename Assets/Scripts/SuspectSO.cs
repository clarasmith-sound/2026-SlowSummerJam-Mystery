using UnityEngine;

[CreateAssetMenu(fileName = "SuspectSO", menuName = "Scriptable Objects/SuspectSO")]
public class SuspectSO : ScriptableObject
{
    public string suspectName;
    public GameObject prefabSuspect;
    [SerializeField] public Clue[] clues;
}
