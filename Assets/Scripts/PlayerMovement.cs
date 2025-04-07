using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    InputAction movementInput;
    InputAction jumpInput;

    float horizontalMovement;

    Rigidbody2D rb;
    Collider2D col;
    SpriteRenderer spriteRenderer;

    [Header("Movement Stats")]
    [SerializeField] float customMass = 1;
    [SerializeField] float speed = 10;
    [SerializeField] float acceleration = 20;
    [SerializeField] float deceleration = 40;

    [Header("Jump")]
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


    [Header("Audio")]
    [SerializeField] AudioResource walkSound;
    [SerializeField] AudioResource jumpSound;
    AudioSource audioSource;
    [SerializeField] float walkSoundCooldown = 0.4f;
    float walkSoundTimer = 0;

    [Header("Light")]
    public GameObject headLight;
    public Transform lightPos1;
    public Transform lightPos2;

    void Start()
    {
        movementInput = InputSystem.actions.FindAction("Move");
        jumpInput = InputSystem.actions.FindAction("Jump");
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        rb.mass = customMass;
        rb.gravityScale = customGravityScale;

        gravity = Physics2D.gravity.y;
    }


    void Update()
    {
        horizontalMovement = movementInput.ReadValue<Vector2>().x;
        if (horizontalMovement > 0)
        {
            spriteRenderer.flipX = false;
            //headLight.transform.localPosition = new Vector3(0.25f, 0.33f, 0);
            headLight.transform.localPosition = lightPos1.localPosition;
        }
        else if (horizontalMovement < 0)
        {
            spriteRenderer.flipX = true;
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

        if (grounded)
        {
            if (!canJump)
            {
                jumpTimer += Time.deltaTime;
                if (jumpTimer > jumpCooldown)
                {
                    jumpTimer = 0;
                    canJump = true;
                }
            }

            if (horizontalMovement != 0)
            {
                if (walkSoundTimer > walkSoundCooldown)
                {
                    walkSoundTimer = 0;
                    // Play walk sound
                    audioSource.resource = walkSound;
                    audioSource.Play();
                }
                else
                {
                    walkSoundTimer += Time.deltaTime;
                }
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
            rb.linearVelocityX = Mathf.MoveTowards(rb.linearVelocityX, speed * horizontalMovement, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            rb.linearVelocityX = Mathf.MoveTowards(rb.linearVelocityX, 0, deceleration * Time.fixedDeltaTime);
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

        audioSource.resource = jumpSound;
        audioSource.Play();
        

        float jumpForce = (Mathf.Sqrt(height * gravity * -2)) * rb.mass;
        Debug.Log("JUMP FORCE: " +  jumpForce);
        rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
    }
}
