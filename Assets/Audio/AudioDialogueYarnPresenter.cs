using UnityEngine;
using Yarn.Unity;
using FMODUnity;
using FMOD.Studio;
using System.Runtime.InteropServices;
using System;

public class AudioDialogueYarnPresenter : DialoguePresenterBase
{
    [Header("FMOD Settings")]
    [SerializeField] private string fmodEventPath = "event:/VoiceOver/DialogueLines";

    private EventInstance _currentVoiceInstance;

    public override async YarnTask RunLineAsync(LocalizedLine line, LineCancellationToken token)
    {
        string lineID = line.TextID;

        try
        {
            _currentVoiceInstance = RuntimeManager.CreateInstance(fmodEventPath);

             GCHandle stringHandle = GCHandle.Alloc(lineID, GCHandleType.Pinned);
            _currentVoiceInstance.setUserData(GCHandle.ToIntPtr(stringHandle));
        }
    }

    public override async YarnTask OnDialogueStartedAsync()
    {
        // Implementation for when dialogue starts
    }

    public override async YarnTask OnDialogueCompleteAsync()
    {
        // Implementation for when dialogue completes
    }



}
