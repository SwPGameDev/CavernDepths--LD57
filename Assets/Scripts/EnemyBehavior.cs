using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float maxHP = 3;
    float currentHP;
    bool alive = true;

    [Header("Movement")]
    [SerializeField] float aggroRange = 5;
    [SerializeField] float mass = 3;
    [SerializeField] float speed = 5;
    [SerializeField] float acceleration = 5;
    [SerializeField] float deceleration = 5;
    [SerializeField] float leapForce = 10;
    [SerializeField] float jumpHeight = 3;

    Vector2 currentPos;
    Vector2 currentMovementVector;
    GameObject target;
    float distanceToTarget;
    PlayerBehavior playerBehavior;

    bool aggro = false;
    bool facingRight = true;
    bool grounded = false;
    [SerializeField] LayerMask groundingLayerMask;

    Collider2D col;
    Rigidbody2D rb;
    SpriteRenderer sr;

    float gravity;

    private void Start()
    {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();


        rb.mass = mass;

        playerBehavior = PlayerBehavior.Instance;
        target = playerBehavior.gameObject;

        gravity = Physics2D.gravity.y;


        currentHP = maxHP;
    }

    private void Update()
    {
        currentPos = transform.position;
        distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
        currentMovementVector = rb.linearVelocity + currentPos;

        float dot = Vector2.Dot(transform.right, currentMovementVector.normalized);
        if (dot > 0)
        {
            facingRight = true;
            sr.flipY = false;
        }
        else
        {
            facingRight = false;
            sr.flipY = true;
        }

    }

    private void FixedUpdate()
    {
        grounded = col.IsTouchingLayers(groundingLayerMask);

        //if (horizontalMovement != 0)
        //{
        //    rb.linearVelocityX = Mathf.MoveTowards(rb.linearVelocityX, speed * horizontalMovement, acceleration * Time.fixedDeltaTime);
        //}
        //else
        //{
        //    rb.linearVelocityX = Mathf.MoveTowards(rb.linearVelocityX, 0, deceleration * Time.fixedDeltaTime);
        //}

        // MoveLeft
        // MoveRight
        // Jump(jumpHeight);
        // JumpAtTarget();
    }

    void Jump(float height)
    {
        float jumpForce = (Mathf.Sqrt(height * gravity * -2)) * rb.mass;
        Debug.Log("JUMP FORCE: " + jumpForce);
        rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
    }

    void JumpAtTarget()
    {
        Vector2 direction = (transform.position - target.transform.position).normalized;
        rb.AddForce(direction * leapForce, ForceMode2D.Impulse);
    }


    public void TakeHit(float damageParam)
    {
        currentHP -= damageParam;
        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        alive = false;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, currentMovementVector);
    }
}
