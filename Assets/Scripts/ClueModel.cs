using System;
using Unity.Properties;

[Serializable]
public class Clue
{
    public string textDescription;
    public string yarnDialogueNode;
    public bool discovered = false;
    [CreateProperty]
    public string GetClueDescription => discovered ? textDescription : "???";
}