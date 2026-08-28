using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using FMODUnity;
using System;
using System.Collections.Generic;

public class StartManager : MonoBehaviour
{
    [Serializable]
    public struct CreditEntry
    {
        public string jobTitle;
        public string personName;
    }

    [Header("Credits Configuration Data")]
    [SerializeField] private VisualTreeAsset creditRowTemplate;

    private CreditEntry[] gameCredits = new CreditEntry[]
    {
        new CreditEntry { jobTitle = "Art", personName = "Ashleypox" },
        new CreditEntry { jobTitle = "Game Designer & UI/UX", personName = "Cat Preimesberger" },
        new CreditEntry { jobTitle = "Tech Audio & UI Programming", personName = "Clara Smith" },
        new CreditEntry { jobTitle = "Programming & Writing", personName = "IfThenCreate" },
        new CreditEntry { jobTitle = "Voice Acting, Vocal Restoration, Sound Design", personName = "James Bartlett" },
        new CreditEntry { jobTitle = "Sound Design, Composition, Voice Acting, Audio Implementation", personName = "Jamie Billings" },
        new CreditEntry { jobTitle = "Project Management & Writing", personName = "Nathania Wong" },
        new CreditEntry { jobTitle = "Art Support", personName = "Widelczyna" }
    };

    public UIDocument startUIDoc;
    private VisualElement startUI;

    [Header("Audio Properties")]
    [SerializeField] private EventReference buttonHoverSound;
    [SerializeField] private EventReference startGameSound;
    [SerializeField] private EventReference genericClickSound;

    private AudioChannelUI[] audioChannels = AudioSliderBinder.DefaultChannels;
    private Dictionary<int, EventCallback<ChangeEvent<float>>> valueCallbacks = new();
    private Dictionary<int, EventCallback<PointerDownEvent>> clickCallbacks = new();

    //Scene containers
    private VisualElement mainMenuContainer;
    private VisualElement optionsMenuContainer;
    private VisualElement creditsMenuContainer;
    private VisualElement creditsListContainer;

    //cached buttons
    private Button startGameButton;
    private Button optionsButton;
    private Button creditsButton;
    private Button optionsBackToStartButton;
    private Button creditsBackButton;


    public void OnEnable()
    {
        startUI = startUIDoc.rootVisualElement;

        mainMenuContainer = startUI.Q<VisualElement>("MainMenuContainer");
        optionsMenuContainer = startUI.Q<VisualElement>("OptionsMenuContainer");
        creditsMenuContainer = startUI.Q<VisualElement>("CreditsMenuContainer");
        creditsListContainer = startUI.Q<VisualElement>("CreditsList");

        startGameButton = startUI.Q<Button>("StartGame");
        optionsButton = startUI.Q<Button>("Options");
        creditsButton = startUI.Q<Button>("Credits");

        if (optionsMenuContainer != null)
        {
            optionsBackToStartButton = optionsMenuContainer.Q<Button>("BackToStart");
        }

        if (creditsMenuContainer != null)
        {
            creditsBackButton = creditsMenuContainer.Q<Button>("OptionsBackBtn");
        }

        
        // Setup Button Listeners
        if (startGameButton != null)
        {
            startGameButton.RegisterCallback<PointerEnterEvent>(OnButtonHover);
            startGameButton.clicked += StartGame;
        }

        if (optionsButton != null)
        {
            optionsButton.RegisterCallback<PointerEnterEvent>(OnButtonHover);
            optionsButton.clicked += OpenOptions;
        }

        if (creditsButton != null)
        {
            creditsButton.RegisterCallback<PointerEnterEvent>(OnButtonHover);
            creditsButton.clicked += OpenCredits;
        }

        if (optionsBackToStartButton != null)
        {
            optionsBackToStartButton.RegisterCallback<PointerEnterEvent>(OnButtonHover);
            optionsBackToStartButton.clicked += CloseSubMenus;
        }

        if (creditsBackButton != null)
        {
            creditsBackButton.RegisterCallback<PointerEnterEvent>(OnButtonHover);
            creditsBackButton.clicked += CloseSubMenus;
        }

         audioChannels = AudioSliderBinder.Bind(
            startUI, audioChannels,
            genericClickSound, 
            valueCallbacks, clickCallbacks);


        PopulateCreditsScreen();
    }


    public void OnDisable()
    {
         AudioSliderBinder.Unbind(audioChannels, valueCallbacks, clickCallbacks);

        if (startGameButton != null)
        {
            startGameButton.UnregisterCallback<PointerEnterEvent>(OnButtonHover);
            startGameButton.clicked -= StartGame;
        }

        if (optionsButton != null)
        {
            optionsButton.UnregisterCallback<PointerEnterEvent>(OnButtonHover);
            optionsButton.clicked -= OpenOptions;
        }

        if (creditsButton != null)
        {
            creditsButton.UnregisterCallback<PointerEnterEvent>(OnButtonHover);
            creditsButton.clicked -= OpenCredits;
        }

        if (optionsBackToStartButton != null)
        {
            optionsBackToStartButton.UnregisterCallback<PointerEnterEvent>(OnButtonHover);
            optionsBackToStartButton.clicked -= CloseSubMenus;
        }

        if (creditsBackButton != null)
        {
            creditsBackButton.UnregisterCallback<PointerEnterEvent>(OnButtonHover);
            creditsBackButton.clicked -= CloseSubMenus;
        }
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

    public void OpenOptions()
    {
        AudioManager.Instance.PlaySound2D(genericClickSound);
        if (mainMenuContainer != null) mainMenuContainer.style.display = DisplayStyle.None;
        if (optionsMenuContainer != null) optionsMenuContainer.style.display = DisplayStyle.Flex;
        if (creditsMenuContainer != null) creditsMenuContainer.style.display = DisplayStyle.None;
    }

    public void OpenCredits()
    {
        AudioManager.Instance.PlaySound2D(genericClickSound);
        if (mainMenuContainer != null) mainMenuContainer.style.display = DisplayStyle.None;
        if (optionsMenuContainer != null) optionsMenuContainer.style.display = DisplayStyle.None;
        if (creditsMenuContainer != null) creditsMenuContainer.style.display = DisplayStyle.Flex;
    }

    public void CloseSubMenus()
    {
        AudioManager.Instance.PlaySound2D(genericClickSound);
        if (mainMenuContainer != null) mainMenuContainer.style.display = DisplayStyle.Flex;
        if (optionsMenuContainer != null) optionsMenuContainer.style.display = DisplayStyle.None;
        if (creditsMenuContainer != null) creditsMenuContainer.style.display = DisplayStyle.None;
    }


    private void PopulateCreditsScreen()
    {
        if (creditsListContainer == null || creditRowTemplate == null) return;

        creditsListContainer.Clear();

        for (int i = 0; i < gameCredits.Length; i++)
        {
            VisualElement rowInstance = creditRowTemplate.Instantiate();

            Label jobLabel = rowInstance.Q<Label>("JobTitle");
            Label nameLabel = rowInstance.Q<Label>("CreditName");

            if (jobLabel != null) jobLabel.text = gameCredits[i].jobTitle;
            if (nameLabel != null) nameLabel.text = gameCredits[i].personName;

            creditsListContainer.Add(rowInstance);
        }
    }

}

