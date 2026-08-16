using PrimeTween;
using UnityEngine;

public class ArrowBob : MonoBehaviour
{
    void Start()
    {
        Move();
    }

    void Move()
    {
        float endTarget = gameObject.transform.position.x + 0.25f;

        Sequence.Create(cycles: -1, cycleMode: CycleMode.Yoyo)
            .Group(Tween.PositionX(gameObject.transform, endValue: endTarget, duration: .35f));
    }
}
