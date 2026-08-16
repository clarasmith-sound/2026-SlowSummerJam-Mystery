using UnityEngine;

[CreateAssetMenu(fileName = "CaseSO", menuName = "Scriptable Objects/CaseSO")]
public class CaseSO : ScriptableObject
{
    public string caseName;
    public string caseDescription;
    [SerializeField] public SuspectSO[] suspects;
}
