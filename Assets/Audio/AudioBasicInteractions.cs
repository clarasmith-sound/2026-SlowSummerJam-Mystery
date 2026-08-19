using UnityEngine;
using FMODUnity;

public class AudioBasicInteractions : MonoBehaviour
{
    [Header("Sounds")]
    [SerializeField] private EventReference clickSound;
    [SerializeField] private EventReference hoverSound;

    private void OnMouseDown()
    {
        AudioManager.Instance.PlaySound2D(clickSound);
    }

    private void OnMouseEnter()
    {
        AudioManager.Instance.PlaySound2D(hoverSound);
    }
}
