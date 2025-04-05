using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    InputAction movementInput;
    InputAction jumpInput;

    bool jumpPressed = false;
    bool grounded = false;

    [SerializeField] LayerMask groundingLayerMask;

    float horizontalMovement;

    Rigidbody2D rb;
    Collider2D col;

    [SerializeField] float speed = 10;
    [SerializeField] float acceleration = 20;
    [SerializeField] float deceleration = 40;
    [SerializeField] float jumpHeight = 10;
    float gravity;

    SpriteRenderer rbSprite;

    public GameObject headLight;

    void Start()
    {
        movementInput = InputSystem.actions.FindAction("Move");
        jumpInput = InputSystem.actions.FindAction("Jump");
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        rbSprite = GetComponent<SpriteRenderer>();
        gravity = Physics.gravity.y;
    }


    void Update()
    {
        horizontalMovement = movementInput.ReadValue<Vector2>().x;
        if (horizontalMovement > 0)
        {
            rbSprite.flipX = false;
            headLight.transform.localPosition = new Vector3(0.25f, 0.33f, 0);
        }
        else if (horizontalMovement < 0)
        {
            rbSprite.flipX = true;
            headLight.transform.localPosition = new Vector3(-0.25f, 0.33f, 0);
        }

        if (jumpInput.WasPressedThisFrame()) // Make buffer, sets to false after a time
        {
            jumpPressed = true;
        }
    }

    private void FixedUpdate()
    {
        grounded = col.IsTouchingLayers(groundingLayerMask);



        if (horizontalMovement != 0)
        {
            rb.linearVelocityX = Mathf.MoveTowards(rb.linearVelocityX, speed * horizontalMovement, acceleration * Time.deltaTime);
        }
        else
        {
            rb.linearVelocityX = Mathf.MoveTowards(rb.linearVelocityX, 0, deceleration * Time.deltaTime);
        }

        if (grounded && jumpPressed)
        {
            jumpPressed = false;
            Jump(jumpHeight);
            // Need extra downforce when release jump
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {

    }

    void Jump(float height)
    {
        float jumpForce = (Mathf.Sqrt(height * gravity * -2)) * rb.mass;
        Debug.Log("JUMP FORCE: " +  jumpForce);
        rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
    }
}
