using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ItemBehavior : MonoBehaviour
{
    public enum ItemTypes
    {
        None,
        Melee,
        Gun,
        Bomb
    }

    public ItemTypes ItemType = ItemTypes.None;

    private bool held = false;
    private GameObject owner = null;

    // STATS
    [Header("General Stats")]
    public float useCooldown = 0.25f;

    private float useTimer = 0f;
    private bool canUse = true;
    [SerializeField] private float damage;
    [SerializeField] private float mass;

    public bool FacingRight = true;
    Vector2 currentPos;
    Vector2 throwDirection = Vector2.zero;

    // GUN
    [Header("Gun")]
    public int maxAmmoCount = 1;
    private int currentAmmo;
    private bool reloading = false;
    public float reloadCooldown = 0.75f;
    private float reloadTimer = 0f;
    [SerializeField] private float projectileVelocity = 10;
    [SerializeField] private float casingVelocity = 5;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject casingPrefab;
    [SerializeField] private Transform muzzle;
    [SerializeField] private Transform ejector;
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] private float muzzleFlashCooldown = 0.05f;
    float muzzleFlashTimer = 0;


    // Bomb
    [Header("Bomb")]
    public float explosionRadius = 2;
    public float fuseDelay = 1.5f;
    public GameObject fuse;
    float fuseTimer = 0;
    bool fuseLit = false;
    public bool sticky = false;



    // Audio
    [Header("Audio")]
    [SerializeField] AudioResource useSuccessSound;
    [SerializeField] AudioResource useFailSound;
    [SerializeField] AudioResource reloadStart;
    [SerializeField] AudioResource reloadEnd;
    [SerializeField] AudioResource fuseSound;
    [SerializeField] AudioResource explosionSound;
    AudioSource audioSource;



    //
    private Animator anim;

    private Collider2D col;
    private Rigidbody2D rb;

    private SpriteRenderer sr;

    private Dictionary<Collider2D, bool> hitDict = new Dictionary<Collider2D, bool>();

    private void Start()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.mass = mass;

        sr = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        currentAmmo = maxAmmoCount;
    }

    private void Update()
    {
        currentPos = new Vector2(transform.position.x, transform.position.y);
        if (held)
        {
            if (!canUse)
            {
                useTimer += Time.deltaTime;
                if (useTimer > useCooldown)
                {
                    useTimer = 0f;
                    canUse = true;
                }
            }

            if (reloading)
            {
                reloadTimer += Time.deltaTime;
                if (reloadTimer > reloadCooldown)
                {
                    reloadTimer = 0f;
                    reloading = false;
                    currentAmmo = maxAmmoCount;
                }
            }

            float dot = Vector2.Dot(owner.transform.right, (transform.position - owner.transform.position).normalized);
            if (dot > 0)
            {
                FacingRight = true;
                sr.flipY = false;
            }
            else
            {
                FacingRight = false;
                sr.flipY = true;
            }
        }

        if (ItemType == ItemTypes.Gun && muzzleFlash.activeInHierarchy)
        {
            muzzleFlashTimer += Time.deltaTime;
            if (muzzleFlashTimer > muzzleFlashCooldown)
            {
                muzzleFlashTimer = 0f;
                muzzleFlash.SetActive(false);
            }
        }



        if (fuseLit)
        {
            if (fuseTimer > fuseDelay)
            {
                Explode();
            }
            else
            {
                fuseTimer += Time.deltaTime;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (ItemType == ItemTypes.Melee)
        {
            if (collider.gameObject.layer == 7)
            {
                if (hitDict.ContainsKey(collider) == false)
                {
                    hitDict.Add(collider, true);
                    collider.GetComponent<Block>().TakeHit(damage);
                }
            }
            //else if (collider.gameObject.layer == 9) monster layer
            //  if (hitDict.ContainsKey(collider) == false)
            //{
            //    hitDict.Add(collider, true);
            //    collider.GetComponent<Block>().TakeHit(damage);
            //}
        }
    }

    public void TryUse()
    {
        Debug.Log("USE: " + gameObject.name);

        if (canUse)
        {
            switch (ItemType)
            {
                case ItemTypes.Melee:
                    anim.SetTrigger("swing");
                    break;

                case ItemTypes.Gun:
                    //anim.SetTrigger("shoot");
                    if (currentAmmo > 0 && !reloading)
                    {
                        Shoot();
                    }
                    else
                    {
                        UseFail();

                        if (!reloading)
                        {
                            reloading = true;
                            reloadTimer = 0f;
                        }
                    }

                    break;

                case ItemTypes.Bomb:
                    if (!fuseLit)
                    {
                        //anim.SetTrigger("light");
                        LightFuse();
                    }
                    break;
            }

            canUse = false;
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

    void LightFuse()
    {
        if (!fuseLit)
        {
            audioSource.resource = fuseSound;
            audioSource.loop = true;
            audioSource.Play();
            fuse.SetActive(true);
            fuseLit = true;
        }
    }

    void Explode()
    {
        Debug.Log("BOOM");
        // Explosion sound
        audioSource.resource = explosionSound;
        audioSource.loop = false;
        audioSource.Play();

        // Effects
        fuse.SetActive(false);
        fuseLit = false;
        //explosion effects


        // Circle cast
        // foreach
        //  collider try component
        //  do damage
        //  explosive force
    }

    private void Shoot()
    {
        // Play Audio
        audioSource.resource = useSuccessSound;
        audioSource.Play();


        // Muzzle Flash
        muzzleFlash.SetActive(true);

        // Spawn bullet
        currentAmmo--;

        GameObject spawnedProjectile = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);

        Vector2 projectileVector = (muzzle.position - ejector.position).normalized * projectileVelocity;

        spawnedProjectile.GetComponent<Rigidbody2D>().AddForce(projectileVector, ForceMode2D.Impulse);
        spawnedProjectile.GetComponent<ProjectileBehavior>().SetDamage(damage);

        // Spawn casing

        GameObject spawnedCasing = Instantiate(casingPrefab, ejector.position, ejector.rotation);

        Vector2 casingVector;

        if (FacingRight)
        {
            casingVector = ejector.up * casingVelocity;
        }
        else
        {
            casingVector = -1 * casingVelocity * ejector.up;
        }

        spawnedCasing.GetComponent<Rigidbody2D>().AddForce(casingVector, ForceMode2D.Impulse);
        spawnedCasing.GetComponent<Rigidbody2D>().AddTorque(5, ForceMode2D.Impulse);
        Destroy(spawnedCasing, 2);
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

    public void ActivateCollider()
    {
        col.enabled = true;
    }

    public void DeactivateCollider()
    {
        col.enabled = false;
        hitDict = new Dictionary<Collider2D, bool>();
    }

    private void OnDrawGizmos()
    {
        if (held && ItemType == ItemTypes.Gun)
        {
            Gizmos.color = Color.green;
            Ray one = new Ray(muzzle.position, (muzzle.position - ejector.position).normalized * projectileVelocity);
            Gizmos.DrawRay(one);

            if (FacingRight)
            {
                Gizmos.color = Color.yellow;
                Ray two = new Ray(ejector.position, ejector.up * casingVelocity);
                Gizmos.DrawRay(two);
            }
            else
            {
                Gizmos.color = Color.yellow;
                Ray two = new Ray(ejector.position, -1 * casingVelocity * ejector.up);
                Gizmos.DrawRay(two);
            }
        }

        if (throwDirection != Vector2.zero)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(currentPos, currentPos + throwDirection);
        }
    }
}
