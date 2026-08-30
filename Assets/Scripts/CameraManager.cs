using UnityEngine;
using PrimeTween;
public class CameraManager : MonoBehaviour
{
    private Camera mainCam;
    private Vector3 startingPos;
    [SerializeField] private float moveDuration = 2f;

    [SerializeField] private Transform Desk;
    private Vector3 deskStartPos;
    [SerializeField] private Vector3 deskMoveAmt = new Vector3(0, -0.15f, 0);

    [SerializeField] private Transform Phone;
    private Vector3 phoneStartPos;
    [SerializeField] private Vector3 phoneMoveAmt = new Vector3(0, -0.6f, 0);
    [SerializeField] private SpriteRenderer backgroundBlur;
    [SerializeField] private SpriteRenderer backgroundBlurFix1;
    [SerializeField] private SpriteRenderer backgroundBlurFix2;
    private Color blurIn = new Color(1f, 1f, 1f, 1f);
    private Color blurOut = new Color(1f, 1f, 1f, 0f);

    private void Start()
    {
        mainCam = Camera.main;
        startingPos = mainCam.transform.position;
        deskStartPos = Desk.position;
        phoneStartPos = Phone.position;
    }

    public void MoveToInspection(GameObject targetSuspect)
    {
        Vector3 cameraTarget = new Vector3(targetSuspect.transform.position.x + 0.75f, 0f, -10f);
        Sequence.Create(cycles: 1)
            .Group(Tween.Position(mainCam.transform, endValue: cameraTarget, duration: moveDuration, ease: Ease.InOutSine)) // move camera
            .Group(Tween.CameraOrthographicSize(mainCam, endValue: 4.15f, duration: moveDuration))
            .Group(Tween.Position(Desk, endValue: deskStartPos + deskMoveAmt, duration: moveDuration, ease: Ease.InOutSine)) // move out desk
            .Group(Tween.Position(Phone, endValue: phoneStartPos + phoneMoveAmt, duration: moveDuration, ease: Ease.InOutSine)) // move out phone
            .Group(Tween.Color(backgroundBlur, endValue: blurIn, duration: moveDuration, ease: Ease.InOutSine)) // fade in background blur
            .Group(Tween.Color(backgroundBlurFix1, endValue: blurIn, duration: moveDuration, ease: Ease.InOutSine)) // fade in background blur fix 1
            .Group(Tween.Color(backgroundBlurFix2, endValue: blurIn, duration: moveDuration, ease: Ease.InOutSine)); // fade in background blur fix 2

    }

    public void MoveToDefault()
    {
        Sequence.Create(cycles: 1)
            .Group(Tween.Position(mainCam.transform, endValue: startingPos, duration: moveDuration, ease: Ease.InOutSine)) // move camera
            .Group(Tween.CameraOrthographicSize(Camera.main, endValue: 5f, duration: moveDuration))
            .Group(Tween.Position(Desk, endValue: deskStartPos, duration: moveDuration, ease: Ease.InOutSine)) // move in desk
            .Group(Tween.Position(Phone, endValue: phoneStartPos, duration: moveDuration, ease: Ease.InOutSine)) // move in phone
            .Group(Tween.Color(backgroundBlur, endValue: blurOut, duration: moveDuration, ease: Ease.InOutSine)) // fade out background blur
            .Group(Tween.Color(backgroundBlurFix1, endValue: blurOut, duration: moveDuration, ease: Ease.InOutSine)) // fade out background blur fix 1
            .Group(Tween.Color(backgroundBlurFix2, endValue: blurOut, duration: moveDuration, ease: Ease.InOutSine)); // fade out background blur fix 2
    }
}
