using UnityEngine;
using UnityEngine.UI;
public class VolumeSlider : MonoBehaviour
{
    private enum VolumeType
    {
        Main,
        Music,
        SFX,
        Dialogue,
        Ambient
    }

    [Header("Type")]
    [SerializeField] private VolumeType volumeType;

    private Slider slider;

    private void Awake()
    {
        slider = this.GetComponentInChildren<Slider>();
    }

    private void Update()
    {
        switch (volumeType)
        {
            case VolumeType.Main:
                slider.value = AudioManager.Instance.mainVolume;
                break;
            case VolumeType.Music:
                slider.value = AudioManager.Instance.musicVolume;
                break;
            case VolumeType.SFX:
                slider.value = AudioManager.Instance.sfxVolume;
                break;
            case VolumeType.Dialogue:
                slider.value = AudioManager.Instance.dialogueVolume;
                break;
            case VolumeType.Ambient:
                slider.value = AudioManager.Instance.ambientVolume;
                break;
        }
    }

    public void OnSliderValueChanged()
    {
        switch (volumeType)
        {
            case VolumeType.Main:
                AudioManager.Instance.mainVolume = slider.value;
                break;
            case VolumeType.Music:
                AudioManager.Instance.musicVolume = slider.value;
                break;
            case VolumeType.SFX:
                AudioManager.Instance.sfxVolume = slider.value;
                break;
            case VolumeType.Dialogue:
                AudioManager.Instance.dialogueVolume = slider.value;
                break;
            case VolumeType.Ambient:
                AudioManager.Instance.ambientVolume = slider.value;
                break;
        }
    }
}
