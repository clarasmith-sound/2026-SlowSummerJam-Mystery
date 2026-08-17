using PrimeTween;
using UnityEngine;
using UnityEngine.UIElements;

public static class TweenExt
{
    public static Tween VisualElementShakeScale(this VisualElement target, ShakeSettings settings)
        => Tween.ShakeCustom(target, new Vector3(0f, 0f, 0f), settings, (target, val) => target.style.scale = new Scale(new Vector3(1f + val.x, 1f + val.y, 1f + val.z)));

    public static Tween VisualElementPunchRotation(this VisualElement target, ShakeSettings settings)
        => Tween.PunchCustom(target, new Vector3(0f, 0f, 0f), settings, (target, val) => target.style.rotate = new Rotate(Quaternion.Euler(val.x, val.y, val.z)));

    public static Tween VisualElementTranslate(this VisualElement target, Vector2 endValue, TweenSettings settings)
        => Tween.Custom(target, new TweenSettings<Vector2>(new Vector2(0f, 0f), endValue, settings), (target, val) => target.style.translate = new StyleTranslate(new Translate(val.x, val.y)));
}
