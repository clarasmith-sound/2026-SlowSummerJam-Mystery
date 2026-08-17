using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Yarn.Unity;
using PrimeTween;

public class ToolkitDialogueView : DialoguePresenterBase
{
    // Reference - https://gist.github.com/hiptopjones/4c8fbe3a23520a5dabfe37f3672bc28d

    [SerializeField] private DialogueRunner runner;
    [SerializeField] private UIDocument UIDocument;
    private VisualElement dialogueRootEl;
    private Button continueButton;
    private bool _waitingForNextLine;

    public void OnEnable()
    {
        dialogueRootEl = UIDocument.rootVisualElement.Q<VisualElement>("DialogueRoot");
        dialogueRootEl.style.display = DisplayStyle.None;
        continueButton = dialogueRootEl.Q<Button>("Continue");
        continueButton.clicked += OnContinueClicked;
    }

    public void OnDisabled()
    {
        dialogueRootEl.Q<Button>("Continue").clicked -= OnContinueClicked;
    }

    private void OnContinueClicked()
    {
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
        dialogueRootEl.style.display = DisplayStyle.None;
        return YarnTask.CompletedTask;
    }

    public override async YarnTask RunLineAsync(LocalizedLine line, LineCancellationToken token)
    {
        var speechSpeaker = dialogueRootEl.Q<Label>("speech-speaker");
        speechSpeaker.text = line.CharacterName;

        var speechText = dialogueRootEl.Q<Label>("speech-text");
        await RunTypewriterEffect(speechText, line.TextWithoutCharacterName.Text, token.HurryUpToken);

        _waitingForNextLine = true;

        await YarnTask.WaitUntilCanceled(token.NextContentToken).SuppressCancellationThrow();

        _waitingForNextLine = false;
    }

    private async Task RunTypewriterEffect(Label speechText, string text, CancellationToken token)
    {
        var count = text.Length;

        for (int i = 0; i < count; i++)
        {
            var output = text;

            if (i < count - 1)
            {
                output = text[..i] + "<alpha=#00>" + text[i..];
            }

            speechText.text = output;

            await Task.Delay(TimeSpan.FromSeconds(0.01));

            if (token.IsCancellationRequested)
            {
                // Requested to hurry up
                speechText.text = text;
                break;
            }
        }
    }
}
