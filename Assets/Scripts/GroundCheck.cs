using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public LayerMask groundLayerMask;
    public bool grounded { get; private set; } = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((groundLayerMask & (1 << collision.gameObject.layer)) != 0) // checks if collision layer is contained in mask
        {
            grounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((groundLayerMask & (1 << collision.gameObject.layer)) != 0) // checks if collision layer is contained in mask
        {
            grounded = false;
        }
    }
}
