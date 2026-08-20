using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using FMODUnity;

public class StartManager : MonoBehaviour
{
    public UIDocument startUIDoc;
    private VisualElement startUI;

    [Header("Audio")]
    [SerializeField] private EventReference buttonHoverSound;
    [SerializeField] private EventReference startGameSound;

    public void OnEnable()
    {
        startUI = startUIDoc.rootVisualElement;
        startUI.Q<Button>("StartGame").RegisterCallback<PointerEnterEvent>(OnButtonHover);
        startUI.Q<Button>("StartGame").clicked += StartGame;
    }

    public void OnDisable()
    {
        startUI.Q<Button>("StartGame").UnregisterCallback<PointerEnterEvent>(OnButtonHover);
        startUI.Q<Button>("StartGame").clicked -= StartGame;
    }

    public void OnButtonHover(PointerEnterEvent evt)
    {
        // SOUND : a button in the start menu was hovered over
        AudioManager.Instance.PlaySound2D(buttonHoverSound);
    }

    public void StartGame()
    {
        // SOUND : the start game button was clicked
        AudioManager.Instance.PlaySound2D(startGameSound);
        SceneManager.LoadScene("Office", LoadSceneMode.Single);
    }
}
