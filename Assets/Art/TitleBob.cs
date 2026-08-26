using PrimeTween;
using UnityEngine;

public class TitleBob : MonoBehaviour
{
    void Start()
    {
        Move();
    }

    void Move()
    {
        float endTarget = gameObject.transform.position.y + 0.15f;

    Sequence.Create(cycles: 1, cycleMode: Sequence.SequenceCycleMode.Yoyo)
        .Group(Tween.PositionY(gameObject.transform, endValue: endTarget, duration: 1f, ease: Ease.InOutSine));
    }
}
