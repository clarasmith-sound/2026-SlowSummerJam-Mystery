using UnityEngine;
using UnityEngine.InputSystem;

public class ZoomDetail : MonoBehaviour
{
    InputAction pointAction;
    public SpriteRenderer suspectDefault;

    public float multiplier;

    void Start()
    {
        pointAction = InputSystem.actions.FindAction("Point");
    }

    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(pointAction.ReadValue<Vector2>());
        Vector3 glassOffsetFromBackground = mousePosition - suspectDefault.transform.position;

        float targetX = -glassOffsetFromBackground.x;
        float targetY = Mathf.Min(0f, -glassOffsetFromBackground.y * multiplier);

        transform.localPosition = new Vector3(targetX, targetY, transform.localPosition.z);
    }
}
