using UnityEngine;
using UnityEngine.InputSystem;

public class MonocleController : MonoBehaviour
{
    InputAction pointAction;
    Rigidbody rb;

    void Start()
    {
        pointAction = InputSystem.actions.FindAction("Point");
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(pointAction.ReadValue<Vector2>());
        mousePosition.z = transform.position.z;
        if (rb.position.x - mousePosition.x > 1 || rb.position.y - mousePosition.y > 1)
        {
            // Jump without physics
            transform.position = mousePosition;
            return;
        }
        Vector3 smoothedPosition = Vector3.Lerp(rb.position, mousePosition, 10f * Time.deltaTime);
        rb.MovePosition(smoothedPosition);
    }
}
