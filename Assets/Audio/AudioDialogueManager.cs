using UnityEngine;
using Yarn.Unity;
using FMODUnity;
using System.Runtime.InteropServices;
using FMOD.Studio;

public class AudioDialogueManager : DialoguePresenterBase
{
    [SerializeField] private EventReference voiceOverEvent;

    private EventInstance currentInstance;
    private GCHandle currentStringHandle;
    private bool isSoundPlaying;

    void Start()
    {
        if (!RuntimeManager.HasBankLoaded("Dialogue_EN"))
        {
            RuntimeManager.LoadBank("Dialogue_EN");
            Debug.Log("[Audio Dialogue] Explicitly loaded Dialogue_EN bank file.");
        }
    }


    public override async YarnTask RunLineAsync(LocalizedLine localisedLine, LineCancellationToken token)
    {
        string audioKey = localisedLine.TextID;

        foreach (var tag in localisedLine.Metadata)
        {
            if (tag.StartsWith("line:"))
            {
                audioKey = tag.Replace("line:", "").Trim();
                break;
            }
        }
        
        if (audioKey.StartsWith("line:"))
        {
            audioKey = audioKey.Substring("line:".Length);
        }

        if(!string.IsNullOrEmpty(audioKey))
        {
            StopCurrentSound();
            PlayProgrammerSound(audioKey);
        }

        while(isSoundPlaying && !token.NextContentToken.IsCancellationRequested)
        {
            if (currentInstance.isValid())
            {
                currentInstance.getPlaybackState(out PLAYBACK_STATE state);
                if (state == PLAYBACK_STATE.STOPPED)
                {
                    isSoundPlaying = false;
                }
            }
            else
            {
                isSoundPlaying = false;
            }

            await YarnTask.Yield();
        }

        StopCurrentSound();
    }

    public override YarnTask OnDialogueStartedAsync()
    {
        return YarnTask.CompletedTask;
    }

    public override YarnTask OnDialogueCompleteAsync()
    {
        StopCurrentSound();
        return YarnTask.CompletedTask;
    }

    private void PlayProgrammerSound(string audioKey)
    {
        if(voiceOverEvent.IsNull) return;

        currentInstance = RuntimeManager.CreateInstance(voiceOverEvent);

        currentStringHandle = GCHandle.Alloc(audioKey, GCHandleType.Pinned);
        currentInstance.setUserData(GCHandle.ToIntPtr(currentStringHandle));

        currentInstance.setCallback(DialogueCallback);
        currentInstance.start();
        currentInstance.release();
        isSoundPlaying = true;
    }

    private void StopCurrentSound()
    {
        if (currentInstance.isValid())
        {
            currentInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            currentInstance.clearHandle();
        }

        isSoundPlaying = false;

        if (currentStringHandle.IsAllocated)
        {
            currentStringHandle.Free();
        }
    }

    [AOT.MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
    private static FMOD.RESULT DialogueCallback(EVENT_CALLBACK_TYPE type, System.IntPtr instancePtr, System.IntPtr parameterPtr)
    {
        EventInstance instance = new EventInstance(instancePtr);
        
        if(type == EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND)
        {
            var parameter = (PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(PROGRAMMER_SOUND_PROPERTIES));

            System.IntPtr userDataPtr;
            instance.getUserData(out userDataPtr);
            
            if (userDataPtr == System.IntPtr.Zero) return FMOD.RESULT.OK;

            GCHandle stringHandle = GCHandle.FromIntPtr(userDataPtr);
            string audioKey = stringHandle.Target as string;

            //Debug.Log($"[Dialogue Audio] Looking up key: '{audioKey}'");

            SOUND_INFO soundInfo;
            var infoResult = RuntimeManager.StudioSystem.getSoundInfo(audioKey, out soundInfo);

            if(infoResult == FMOD.RESULT.OK)
            {
                FMOD.Sound dialogueSound;
                var soundResult = RuntimeManager.CoreSystem.createSound(
                    soundInfo.name_or_data,
                    soundInfo.mode | FMOD.MODE.LOOP_OFF,
                    ref soundInfo.exinfo,
                    out dialogueSound
                );

                if(soundResult == FMOD.RESULT.OK)
                {
                    parameter.sound = dialogueSound.handle;
                    parameter.subsoundIndex = soundInfo.subsoundindex;
                    Marshal.StructureToPtr(parameter, parameterPtr, false);
                }
                // else
                // {
                //     UnityEngine.Debug.LogWarning($"[Dialogue Audio] createSound failed: {soundResult} for key '{audioKey}'");
                // }
            }
            // else
            // {
            //     UnityEngine.Debug.LogWarning($"[Dialogue Audio] getSoundInfo failed: {infoResult} for key '{audioKey}'");
            // }
        }
        else if (type == EVENT_CALLBACK_TYPE.DESTROY_PROGRAMMER_SOUND)
        {
            var parameter = (PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(PROGRAMMER_SOUND_PROPERTIES));
            FMOD.Sound dialogueSound = new FMOD.Sound(parameter.sound);
            dialogueSound.release();
        }

        return FMOD.RESULT.OK;
    }
}
