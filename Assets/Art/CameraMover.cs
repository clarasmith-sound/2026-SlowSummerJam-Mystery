using UnityEngine;
using PrimeTween;
using UnityEngine.Rendering.Universal;
using System.Runtime.CompilerServices;

public class CameraMover : MonoBehaviour
{
    private Transform mainCam;
    private Vector3 startingPos;
    [SerializeField] private Vector3 targetPos;

    [SerializeField] private float moveDuration;

    [SerializeField] private Transform reportCard;
    [SerializeField] private Vector3 reportStartPos;
    [SerializeField] private Vector3 reportCardMoveAmt;

    [SerializeField] private Transform Desk;
    [SerializeField] private Vector3 deskStartPos;
    [SerializeField] private Vector3 deskMoveAmt;

    [SerializeField] private SpriteRenderer backgroundBlur;
    [SerializeField] private Color blurIn;
    [SerializeField] private Color blurOut;

    private Color transparent;

    private void Start()
    {
        mainCam = Camera.main.transform;
        startingPos = mainCam.position;

        deskStartPos = Desk.position;
        reportStartPos = reportCard.position;
    }

    private void MoveToInspection()
    {
        Sequence.Create(cycles: 1)
            .Group(Tween.Position(mainCam, endValue: targetPos, duration: moveDuration, ease: Ease.InOutSine)) // move camera
            .Group(Tween.CameraOrthographicSize(Camera.main, endValue: 3.35f, duration: moveDuration))
            .Group(Tween.Position(reportCard, endValue: reportCard.position + reportCardMoveAmt, duration: moveDuration, ease: Ease.InOutSine)) // move in report card
            .Group(Tween.Position(Desk, endValue: Desk.position + deskMoveAmt, duration: moveDuration, ease: Ease.InOutSine)) // move out desk
            .Group(Tween.Color(backgroundBlur, endValue: blurIn, duration: moveDuration, ease: Ease.InOutSine)); // fade in background blur
    }

    private void MoveToDefault()
    {
        Sequence.Create(cycles: 1)
            .Group(Tween.Position(mainCam, endValue: startingPos, duration: moveDuration, ease: Ease.InOutSine)) // move camera
            .Group(Tween.CameraOrthographicSize(Camera.main, endValue: 5f, duration: moveDuration))
            .Group(Tween.Position(reportCard, endValue: reportStartPos, duration: moveDuration, ease: Ease.InOutSine)) // move out report card
            .Group(Tween.Position(Desk, endValue: deskStartPos, duration: moveDuration, ease: Ease.InOutSine)) // move in desk
            .Group(Tween.Color(backgroundBlur, endValue: blurOut, duration: moveDuration, ease: Ease.InOutSine)); // fade out background blur
    }
}
