using UnityEngine;
using UnityEngine.InputSystem;

public class LookAtMousePos : MonoBehaviour
{
    private Vector2 mousePos;
    private Camera cam;

    Vector2 currentPos;
    Vector2 direction;
    float angle;

    public float rotationSpeed = 60;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        mousePos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        currentPos = new Vector2(transform.position.x, transform.position.y);
        direction = (mousePos - currentPos).normalized;
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), rotationSpeed * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(mousePos, 0.25f);
    }
}