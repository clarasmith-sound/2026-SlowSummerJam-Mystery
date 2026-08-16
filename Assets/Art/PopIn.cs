using PrimeTween;
using UnityEngine;

public class PopIn : MonoBehaviour
{
    void Start()
    {
        Move();
    }

    void Move()
    {
        Sequence.Create(cycles: 1)
            .Group(Tween.ShakeScale(gameObject.transform, strength: new Vector3(.1f, .1f, .1f), duration: 0.3f, frequency: 3))
            .Group(Tween.PunchLocalRotation(gameObject.transform, strength: new Vector3(0f, 0f, -5f), duration: 0.25f, frequency: 5));
    }
}
