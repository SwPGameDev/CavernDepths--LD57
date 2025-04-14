using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float maxHP = 3;
    float currentHP;
    bool alive = true;

    [Header("Pathing")]
    [SerializeField] float idleCooldown = 3;
    float idleTimer = 0;
    public int lateralMovementDirection = 0;

    // Raycasts for walls/gaps
    


    [Header("Movement")]
    [SerializeField] float aggroRange = 5;
    [SerializeField] float mass = 3;
    [SerializeField] float speed = 5;
    [SerializeField] float idleSpeed = 2;
    [SerializeField] float acceleration = 5;
    [SerializeField] float deceleration = 5;
    [SerializeField] float leapForce = 10;
    [SerializeField] float jumpHeight = 3;
    [SerializeField] float checkDistance = 1;

    Vector2 currentPos;
    Vector2 currentMovementDirection;
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
        currentMovementDirection = rb.linearVelocity.normalized + currentPos;

        if (aggro)
        {
            float targetDot = Vector2.Dot(transform.right, target.transform.position.normalized);
            if (targetDot > 0)
            {
                lateralMovementDirection = 1;
            }
            else
            {
                lateralMovementDirection = -1;
            }


            // leap timer
        }
        else
        {
            if (idleTimer > idleCooldown)
            {
                idleTimer = 0;
                NewIdleAction();
            }
            else
            {
                idleTimer += Time.deltaTime;
            }

            // Look for void in floor
        }


        float movementDot = Vector2.Dot(transform.right, currentMovementDirection);
        if (movementDot > 0)
        {
            facingRight = true;
            sr.flipX = false;
        }
        else
        {
            facingRight = false;
            sr.flipX = true;
        }


        if (distanceToTarget < aggroRange)
        {
            if (!aggro)
            {
                aggro = true;
                // Active aggro
                // Sounds
            }
        }




    }

    private void FixedUpdate()
    {
        grounded = col.IsTouchingLayers(groundingLayerMask);

        // Move
        rb.linearVelocityX = Mathf.MoveTowards(rb.linearVelocityX, lateralMovementDirection * speed, acceleration * Time.fixedDeltaTime);

        if (grounded)
        {
            // Check Left
            RaycastHit2D leftHitInfo = Physics2D.Raycast(transform.position, -1 * transform.right, checkDistance, groundingLayerMask);
            if (leftHitInfo)
            {
                Jump(jumpHeight);
            }

            // Check Right
            RaycastHit2D rightHitInfo = Physics2D.Raycast(transform.position, transform.right, checkDistance, groundingLayerMask);
            if (rightHitInfo)
            {
                Jump(jumpHeight);
            }
        }

    }


    void NewIdleAction()
    {
        lateralMovementDirection = Random.Range(-1, 2);
    }


    void Jump(float height)
    {
        float jumpForce = (Mathf.Sqrt(height * gravity * -2)) * rb.mass;
        Debug.Log("JUMP FORCE: " + jumpForce);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        Debug.DrawLine(currentPos, Vector2.up * jumpForce, Color.white, 2);
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

        //sr.sprite = deadSprite;
        sr.flipY = true;
        gameObject.layer = 10;
    }


    private void OnDrawGizmos()
    {
        //if (aggro)
        //{
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawLine(transform.position, target.transform.position);
        //}


        //Gizmos.color = Color.cyan;
        //Gizmos.DrawLine(transform.position, transform.position + transform.right);

        //Gizmos.color = Color.yellow;
        //Gizmos.DrawLine(transform.position, currentMovementDirection);


        // LEFT
        Gizmos.color = new Color(0.172f, 0.12f, 0.186f, 1);
        Gizmos.DrawLine(transform.position, transform.position + (-1 * checkDistance * transform.right));

        // RIGHT
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (transform.right * checkDistance));

        // UP
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + (transform.up * jumpHeight));
    }
}
