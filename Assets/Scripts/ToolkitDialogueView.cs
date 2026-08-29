using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Yarn.Unity;
using PrimeTween;
using FMODUnity;


public class ToolkitDialogueView : DialoguePresenterBase
{
    // Reference - https://gist.github.com/hiptopjones/4c8fbe3a23520a5dabfe37f3672bc28d

    [SerializeField] private DialogueRunner runner;
    [SerializeField] private UIDocument UIDocument;
    private VisualElement dialogueRootEl;
    private Button continueButton;
    private bool _waitingForNextLine;

    [Header("Audio")]
    [SerializeField] private EventReference buttonHoverSound;
    [SerializeField] private EventReference continueButtonSound;
    [SerializeField] private EventReference dialogueOpenSound;
    [SerializeField] private EventReference dialogueCloseSound;
    [SerializeField] private EventReference typeWriterSound;

    [Header("Inspection Visual Manager")]
    [SerializeField] private InspectionVisualManager inspectionVisualManager;


    public void OnEnable()
    {
        dialogueRootEl = UIDocument.rootVisualElement.Q<VisualElement>("DialogueRoot");
        dialogueRootEl.style.display = DisplayStyle.None;
        continueButton = dialogueRootEl.Q<Button>("Continue");
        continueButton.clicked += OnContinueClicked;
        continueButton.RegisterCallback<PointerEnterEvent>(OnButtonHover);

    }

    public void OnDisabled()
    {
        dialogueRootEl.Q<Button>("Continue").clicked -= OnContinueClicked;
        continueButton.UnregisterCallback<PointerEnterEvent>(OnButtonHover);
    }

    public void OnButtonHover(PointerEnterEvent evt)
    {
        // SOUND :  The "next/continue" button in a dialogue box was hovered over
        AudioManager.Instance.PlaySound2D(buttonHoverSound);
    }

    private void OnContinueClicked()
    {
        AudioManager.Instance.PlaySound2D(continueButtonSound);
        if (_waitingForNextLine)
        {
            runner.RequestNextLine();
        }
        else
        {
            runner.RequestHurryUpLine();
        }
    }

    public override YarnTask OnDialogueStartedAsync()
    {
        // SOUND :  Dialogue box pops up (note overlap - this happens when a clue is clicked)
        AudioManager.Instance.PlaySound2D(dialogueOpenSound);

        //Disable all other selectables using the options bool
        if(GameManager.Instance != null) GameManager.Instance.optionsMenuOpen = true;
        if(inspectionVisualManager != null) inspectionVisualManager.HidePermanentRecord();

        dialogueRootEl.style.display = DisplayStyle.Flex;

        Sequence.Create(cycles: 1)
           .Group(dialogueRootEl.VisualElementShakeScale(new ShakeSettings(strength: new Vector3(.1f, .1f, .1f), duration: 0.3f, frequency: 3)))
           .Group(dialogueRootEl.VisualElementPunchRotation(new ShakeSettings(strength: new Vector3(0f, 0f, -5f), duration: 0.25f, frequency: 5)));
        Sequence.Create(cycles: -1, Sequence.SequenceCycleMode.Yoyo)
            .Group(continueButton.VisualElementTranslate(endValue: new Vector2(10f, 0f), new TweenSettings(duration: .35f))); //gameObject.transform, endValue: endTarget, duration: .35f));
        return YarnTask.CompletedTask;
    }

    public override YarnTask OnDialogueCompleteAsync()
    {
        AudioManager.Instance.PlaySound2D(dialogueCloseSound);
        dialogueRootEl.style.display = DisplayStyle.None;

        if(GameManager.Instance != null) GameManager.Instance.optionsMenuOpen = false;
        if(inspectionVisualManager != null) inspectionVisualManager.ShowPermanentRecord();
        return YarnTask.CompletedTask;
    }

    public override async YarnTask RunLineAsync(LocalizedLine line, LineCancellationToken token)
    {
        var speechSpeaker = dialogueRootEl.Q<Label>("speech-speaker");
        if (speechSpeaker == null) Debug.LogError("speech-speaker Label not found!");
        speechSpeaker.text = line.CharacterName;

        var speechText = dialogueRootEl.Q<Label>("speech-text");
        if (speechText == null) Debug.LogError("speech-text Label not found!");
        Debug.Log($"Line text received: '{line.TextWithoutCharacterName.Text}' (length: {line.TextWithoutCharacterName.Text.Length})");
        await RunTypewriterEffect(speechText, line.TextWithoutCharacterName.Text, token.HurryUpToken);
        //speechText.text = line.TextWithoutCharacterName.Text;

        _waitingForNextLine = true;

        await YarnTask.WaitUntilCanceled(token.NextContentToken).SuppressCancellationThrow();

        _waitingForNextLine = false;
    }

    private async Task RunTypewriterEffect(Label speechText, string text, CancellationToken token)
    {
        var count = text.Length;

        for (int i = 0; i < count; i++)
        {
            speechText.text = text[..(i + 1)];

            if (!char.IsWhiteSpace(text[i]))
            {
                // SOUND :  A character in a dialogue line is displayed (not for whitespace)
                AudioManager.Instance.PlaySound2D(typeWriterSound);
            }

            float delay = char.IsPunctuation(text[i]) ? 0.1f : 0.01f;
            await Task.Delay(TimeSpan.FromSeconds(delay));

            if (token.IsCancellationRequested)
            {
                speechText.text = text;
                break;
            }
        }
    }
}
