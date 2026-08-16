using UnityEngine;
using PrimeTween;
public class CameraManager : MonoBehaviour
{
    private Camera mainCam;
    private Vector3 startingPos;
    [SerializeField] private float moveDuration = 2f;

    [SerializeField] private Transform Desk;
    private Vector3 deskStartPos;
    [SerializeField] private Vector3 deskMoveAmt = new Vector3(0, -0.35f, 0);

    [SerializeField] private SpriteRenderer backgroundBlur;
    private Color blurIn = new Color(1f, 1f, 1f, 1f);
    private Color blurOut = new Color(1f, 1f, 1f, 0f);

    private void Start()
    {
        mainCam = Camera.main;
        startingPos = mainCam.transform.position;
        deskStartPos = Desk.position;
    }

    public void MoveToInspection(GameObject targetSuspect)
    {
        Vector3 cameraTarget = new Vector3(targetSuspect.transform.position.x + 0.75f, -0.75f, -10f);
        Sequence.Create(cycles: 1)
            .Group(Tween.Position(mainCam.transform, endValue: cameraTarget, duration: moveDuration, ease: Ease.InOutSine)) // move camera
            .Group(Tween.CameraOrthographicSize(mainCam, endValue: 3.35f, duration: moveDuration))
            .Group(Tween.Position(Desk, endValue: Desk.position + deskMoveAmt, duration: moveDuration, ease: Ease.InOutSine)) // move out desk
            .Group(Tween.Color(backgroundBlur, endValue: blurIn, duration: moveDuration, ease: Ease.InOutSine)); // fade in background blur
    }

    public void MoveToDefault()
    {
        Sequence.Create(cycles: 1)
            .Group(Tween.Position(mainCam.transform, endValue: startingPos, duration: moveDuration, ease: Ease.InOutSine)) // move camera
            .Group(Tween.CameraOrthographicSize(Camera.main, endValue: 5f, duration: moveDuration))
            .Group(Tween.Position(Desk, endValue: deskStartPos, duration: moveDuration, ease: Ease.InOutSine)) // move in desk
            .Group(Tween.Color(backgroundBlur, endValue: blurOut, duration: moveDuration, ease: Ease.InOutSine)); // fade out background blur
    }
}
