using UnityEngine;
using UnityEngine.Audio;

public class TreasureBehavior : MonoBehaviour, IHoldable
{
    public GameObject HoldableObject => gameObject;
    public HoldableType ItemType;
    HoldableType IHoldable.ItemType => ItemType;
    public int TreasureValue = 1;
    public float mass = 1;



    AudioSource audioSource;
    [SerializeField] AudioResource useSuccessSound;
    [SerializeField] AudioResource useFailSound;

    Vector2 currentPos;
    Vector2 throwDirection;
    bool canUse = true;
    bool held = false;

    GameObject owner;

    SpriteRenderer sr;
    Collider2D col;
    Rigidbody2D rb;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        rb.mass = mass;
    }

    private void Update()
    {
        currentPos = transform.position;
    }

    public void SetTresureValue(int treasureValueParam)
    {
        TreasureValue = treasureValueParam;
    }

    public void Throw(Vector2 targetPos, float throwStrength)
    {
        Debug.Log("THROW: " + gameObject.name);
        currentPos = new Vector2(transform.position.x, transform.position.y);
        throwDirection = (targetPos - currentPos).normalized;

        Drop();

        rb.AddForce(throwDirection * throwStrength, ForceMode2D.Impulse);
    }

    public void Pickup(GameObject ownerParam, GameObject parentTransform)
    {
        Debug.Log("Holder: " + parentTransform.name + " Pickup: " + gameObject.name);

        held = true;
        owner = ownerParam;
        col.isTrigger = true;
        rb.linearVelocity = Vector3.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        gameObject.layer = 11; // 11 is holding
        transform.parent = parentTransform.transform;
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    public void Drop()
    {
        Debug.Log("DROP: " + gameObject.name);

        sr.flipY = false;
        held = false;
        owner = null;
        col.isTrigger = false;
        col.enabled = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        gameObject.layer = 8; // 8 is item
        transform.parent = null;
    }

    public void TryUse()
    {
        Debug.Log("USE: " + gameObject.name);

        if (canUse)
        {

        }
        else
        {
            UseFail();
        }
    }

    void UseFail()
    {
        audioSource.resource = useFailSound;
        audioSource.loop = false;
        audioSource.Play();
    }

}
