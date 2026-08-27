using UnityEngine;
using Yarn.Unity;
using FMODUnity;
using System.Runtime.InteropServices;
using System.Threading;
using FMOD.Studio;
using Unity.VisualScripting;
using Yarn.Unity.UnityLocalization;
using FMODUnityResonance;

public class AudioDialogueManager : DialoguePresenterBase
{
    [SerializeField] private EventReference voiceOverEvent;

    private EventInstance currentInstance;
    private GCHandle currentStringHandle;
    private bool isSoundPlaying;

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

        if(!string.IsNullOrEmpty(audioKey))
        {
            StopCurrentSound();
            PlayProgrammerSound(audioKey);
        }

        while(isSoundPlaying && !token.NextContentToken.IsCancellationRequested)
        {
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
        if(isSoundPlaying)
        {
            currentInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            isSoundPlaying = false;

            if (currentStringHandle.IsAllocated)
            {
                currentStringHandle.Free();
            }
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
            GCHandle stringHandle = GCHandle.FromIntPtr(userDataPtr);
            string audioKey = stringHandle.Target as string;

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
            }
        }
        else if (type == EVENT_CALLBACK_TYPE.DESTROY_PROGRAMMER_SOUND)
        {
            System.IntPtr userDataPtr;
            instance.getUserData(out userDataPtr);
            if(userDataPtr != System.IntPtr.Zero)
            {
                GCHandle stringHandle = GCHandle.FromIntPtr(userDataPtr);
                if (stringHandle.IsAllocated)
                {
                    stringHandle.Free();
                }
            }
        }

        return FMOD.RESULT.OK;
    }
}
