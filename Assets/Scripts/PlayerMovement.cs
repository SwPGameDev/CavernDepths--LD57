using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    InputAction movementInput;
    InputAction jumpInput;

    float horizontalMovement;

    Rigidbody2D rb;
    Collider2D col;

    [SerializeField] float customMass = 1;

    [SerializeField] float speed = 10;
    [SerializeField] float acceleration = 20;
    [SerializeField] float deceleration = 40;


    [SerializeField] float jumpHeight = 10;
    [SerializeField] float extraDownGravity = 2;
    [SerializeField] float downDampening = 2;
    [SerializeField] float customGravityScale;
    float gravity;
    [SerializeField] LayerMask groundingLayerMask;

    bool canJump = false;
    bool jumpPressed = false;
    bool doJump = false;
    bool grounded = false;

    [SerializeField] float jumpCooldown = 0.15f;
    float jumpTimer = 0;

    SpriteRenderer rbSprite;

    public GameObject headLight;
    public Transform lightPos1;
    public Transform lightPos2;

    void Start()
    {
        movementInput = InputSystem.actions.FindAction("Move");
        jumpInput = InputSystem.actions.FindAction("Jump");
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        rbSprite = GetComponent<SpriteRenderer>();

        rb.mass = customMass;
        rb.gravityScale = customGravityScale;

        gravity = Physics2D.gravity.y;
    }


    void Update()
    {
        horizontalMovement = movementInput.ReadValue<Vector2>().x;
        if (horizontalMovement > 0)
        {
            rbSprite.flipX = false;
            //headLight.transform.localPosition = new Vector3(0.25f, 0.33f, 0);
            headLight.transform.localPosition = lightPos1.localPosition;
        }
        else if (horizontalMovement < 0)
        {
            rbSprite.flipX = true;
            //headLight.transform.localPosition = new Vector3(-0.25f, 0.33f, 0);
            headLight.transform.localPosition = lightPos2.localPosition;
        }

        if (jumpInput.WasPressedThisFrame()) // Make buffer, sets to false after a time
        {
            if (canJump)
            {
                jumpPressed = true;
                doJump = true;
            }
        }

        if (grounded && !canJump)
        {
            jumpTimer += Time.deltaTime;
            if (jumpTimer > jumpCooldown)
            { 
                jumpTimer = 0;
                canJump = true;
            }
        }

        if (jumpInput.WasReleasedThisFrame())
        {
            jumpPressed = false;
            // Lerp gravity scale
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

        if (grounded)
        {
            rb.gravityScale = customGravityScale;

            if (doJump)
            {
                Jump(jumpHeight);
            }
            
            
        }
        else if (!grounded && !jumpPressed)
        {
            rb.gravityScale = Mathf.Lerp(rb.gravityScale, extraDownGravity, downDampening * Time.fixedDeltaTime);
        }
    }

    void Jump(float height)
    {
        doJump = false;
        canJump = false;

        float jumpForce = (Mathf.Sqrt(height * gravity * -2)) * rb.mass;
        Debug.Log("JUMP FORCE: " +  jumpForce);
        rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
    }
}
