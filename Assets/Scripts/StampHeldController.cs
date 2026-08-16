using UnityEngine;
using UnityEngine.InputSystem;

public class StampHeldController : MonoBehaviour
{
    InputAction pointAction;

    void Start()
    {
        pointAction = InputSystem.actions.FindAction("Point");
    }

    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(pointAction.ReadValue<Vector2>());
        mousePosition.z = transform.position.z;
        transform.position = mousePosition;
    }
}
