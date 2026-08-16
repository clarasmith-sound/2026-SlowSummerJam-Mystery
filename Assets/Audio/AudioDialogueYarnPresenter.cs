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

    public override YarnTask OnDialogueStartedAsync() => YarnTask.CompletedTask;

    public override YarnTask OnDialogueCompleteAsync() => YarnTask.CompletedTask;

    public override async YarnTask RunLineAsync(LocalizedLine line, LineCancellationToken token)
    {
        string lineID = line.TextID;
        string character = line.CharacterName ?? "Unknown";

        try
        {
            FMOD.GUID eventGUID = RuntimeManager.PathToGUID(fmodEventPath);
            _currentVoiceInstance = RuntimeManager.CreateInstance(eventGUID);

            _currentVoiceInstance.setParameterByNameWithLabel("Character", character);
            
            GCHandle stringHandle = GCHandle.Alloc(lineID, GCHandleType.Pinned);
            _currentVoiceInstance.setUserData(GCHandle.ToIntPtr(stringHandle));

            _currentVoiceInstance.start();

            while(IsAudioPlaying(_currentVoiceInstance))
            {
                if(token.IsNextContentRequested)
                {
                    _currentVoiceInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                    break;
                }

                await YarnTask.Yield();
            }
            if(stringHandle.IsAllocated)
            {
                stringHandle.Free();
            }
        }
        catch(Exception ex)
        {
            Debug.LogError($"Error playing dialogue line '{lineID}' for character '{character}': {ex.Message}");
        }
        finally
        {
            _currentVoiceInstance.release();
        }
    }

    private bool IsAudioPlaying(EventInstance instance)
    {
        if (instance.isValid())
        {
            instance.getPlaybackState(out PLAYBACK_STATE state);
            return state == PLAYBACK_STATE.PLAYING;
        }
        return false;
    }

}
