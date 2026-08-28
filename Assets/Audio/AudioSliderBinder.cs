using UnityEngine;
using UnityEngine.UIElements;
using FMODUnity;
using System;
using System.Collections.Generic;

[Serializable]
public struct AudioChannelUI
{
    public string channelName;
    public string sliderUxmlName;
    public string labelUxmlName;
    public AudioOptionSliders sliderType;

    internal Slider slider;
    internal Label label;
}

public static class AudioSliderBinder
{
    public static readonly AudioChannelUI[] DefaultChannels = new AudioChannelUI[]
    {
        new AudioChannelUI { channelName = "Master",    sliderUxmlName = "SliderMain",     labelUxmlName = "db-value", sliderType = AudioOptionSliders.MainVolume },
        new AudioChannelUI { channelName = "Music",     sliderUxmlName = "SliderMusic",    labelUxmlName = "db-value", sliderType = AudioOptionSliders.MusicVolume },
        new AudioChannelUI { channelName = "SFX",       sliderUxmlName = "SliderSFX",      labelUxmlName = "db-value", sliderType = AudioOptionSliders.SFXVolume },
        new AudioChannelUI { channelName = "Ambience",  sliderUxmlName = "SliderAmbience", labelUxmlName = "db-value", sliderType = AudioOptionSliders.AmbientVolume },
        new AudioChannelUI { channelName = "Dialogue",  sliderUxmlName = "SliderDialogue", labelUxmlName = "db-value", sliderType = AudioOptionSliders.DialogueVolume }
    };

        public static AudioChannelUI[] Bind(
        VisualElement root,
        AudioChannelUI[] channels,
        EventReference clickSound,
        Dictionary<int, EventCallback<ChangeEvent<float>>> valueCallbacks,
        Dictionary<int, EventCallback<PointerDownEvent>> clickCallbacks)
    {
        float lowValue = AudioManager.Instance.SliderLowValue;
        float highValue = AudioManager.Instance.SliderHighValue;
        float defaultValue = AudioManager.Instance.SliderDefaultValue;

        for (int i = 0; i < channels.Length; i++)
        {
            VisualElement rowContainer = root.Q<VisualElement>(channels[i].sliderUxmlName);

            if (rowContainer != null)
            {
                channels[i].slider = rowContainer.Q<Slider>();
                channels[i].label = rowContainer.Q<Label>(className: channels[i].labelUxmlName);
            }

            if (channels[i].slider == null) continue;

            channels[i].slider.lowValue = lowValue;
            channels[i].slider.highValue = highValue;

            int cachedIndex = i;

            EventCallback<ChangeEvent<float>> valueCallback = evt =>
                OnVolumeSliderChanged(channels, cachedIndex, evt.newValue);

            EventCallback<PointerDownEvent> clickCallback = evt =>
                OnSliderClicked(channels, cachedIndex, evt, defaultValue, clickSound);

            valueCallbacks[cachedIndex] = valueCallback;
            clickCallbacks[cachedIndex] = clickCallback;

            float savedVolume = AudioManager.Instance.GetVolume(channels[cachedIndex].sliderType);
            channels[cachedIndex].slider.SetValueWithoutNotify(savedVolume);
            UpdateVolumeLabel(channels[cachedIndex], savedVolume, lowValue);

            channels[cachedIndex].slider.RegisterValueChangedCallback(valueCallback);
            channels[cachedIndex].slider.RegisterCallback(clickCallback, TrickleDown.TrickleDown);
        }

        return channels;
    }

    public static void Unbind(
        AudioChannelUI[] channels,
        Dictionary<int, EventCallback<ChangeEvent<float>>> valueCallbacks,
        Dictionary<int, EventCallback<PointerDownEvent>> clickCallbacks)
    {
        for (int i = 0; i < channels.Length; i++)
        {
            if (channels[i].slider == null) continue;

            if (valueCallbacks.TryGetValue(i, out var valueCallback))
                channels[i].slider.UnregisterValueChangedCallback(valueCallback);

            if (clickCallbacks.TryGetValue(i, out var clickCallback))
                channels[i].slider.UnregisterCallback(clickCallback);
        }

        valueCallbacks.Clear();
        clickCallbacks.Clear();
    }

    private static void OnVolumeSliderChanged(AudioChannelUI[] channels, int channelIndex, float dbValue)
    {
        UpdateVolumeLabel(channels[channelIndex], dbValue, AudioManager.Instance.SliderLowValue);
        AudioManager.Instance.SetVolume(channels[channelIndex].sliderType, dbValue);
    }

    private static void OnSliderClicked(AudioChannelUI[] channels, int channelIndex, PointerDownEvent evt, float defaultValue, EventReference clickSound)
    {
        if (evt.clickCount != 2) return;

        Slider activeSlider = channels[channelIndex].slider;
        if (activeSlider == null) return;

        activeSlider.value = defaultValue;
        AudioManager.Instance.PlaySound2D(clickSound);
        evt.StopPropagation();
    }

    private static void UpdateVolumeLabel(AudioChannelUI channel, float dbValue, float lowValue)
    {
        if (channel.label == null) return;

        if (dbValue <= (lowValue + 0.5f))
        {
            channel.label.text = "MUTED";
        }
        else
        {
            string sign = dbValue > 0 ? "+" : "";
            channel.label.text = $"{sign}{dbValue:F1} dB";
        }
    }
}