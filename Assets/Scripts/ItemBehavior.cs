using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ItemBehavior : MonoBehaviour, IHoldable
{
    public GameObject HoldableObject => gameObject;
    public HoldableType ItemType;
    HoldableType IHoldable.ItemType => ItemType;

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
    private Vector2 currentPos;
    private Vector2 throwDirection = Vector2.zero;

    // GUN
    [Header("Gun")]
    public int maxAmmoCount = 1;

    public int currentAmmo;
    private bool reloading = false;
    public float reloadCooldown = 0.75f;
    private float reloadTimer = 0f;
    [SerializeField] private float projectileVelocity = 10;
    [SerializeField] private float casingVelocity = 5;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject casingPrefab;
    [SerializeField] private Transform muzzle;
    [SerializeField] private Transform ejector;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private float muzzleFlashCooldown = 0.05f;
    [SerializeField] private bool shotgun = false;
    [SerializeField] private int extraPellets = 1;
    [SerializeField] private float spreadAngle = 3;
    private float muzzleFlashTimer = 0;

    // Bomb
    [Header("Bomb")]
    public float explosionRadius = 2;

    public float fuseDelay = 1.5f;
    public GameObject fuseDecal;
    public GameObject fuseLight;
    private float fuseTimer = 0;
    private bool fuseLit = false;
    public bool sticky = false;
    public Transform lightPos1;
    public Transform lightPos2;

    // Audio
    [Header("Audio")]
    [SerializeField] private AudioResource useSuccessSound;

    [SerializeField] private AudioResource useFailSound;
    [SerializeField] private AudioResource reloadStart;
    [SerializeField] private AudioResource reloadEnd;
    [SerializeField] private AudioResource fuseSound;
    [SerializeField] private AudioResource explosionSound;
    private AudioSource audioSource;

    //
    private Animator anim;

    private Collider2D col;

    [Tooltip("For melee")]
    public Collider2D hitCollider;

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
            }
            else
            {
                FacingRight = false;
            }
        }

        if (FacingRight)
        {
            sr.flipY = false;
            if (fuseDecal || fuseLight)
            {
                fuseDecal.GetComponent<SpriteRenderer>().flipY = false;
                fuseDecal.transform.position = lightPos1.position;
            }
        }
        else
        {
            sr.flipY = true;
            if (fuseDecal || fuseLight)
            {
                fuseDecal.GetComponent<SpriteRenderer>().flipY = true;
                fuseDecal.transform.position = lightPos2.position;
            }
        }

        if (ItemType == HoldableType.Gun && muzzleFlash.activeInHierarchy)
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
        if (ItemType == HoldableType.Melee)
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
                case HoldableType.Melee:
                    anim.SetTrigger("swing");
                    break;

                case HoldableType.Gun:
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

                case HoldableType.Bomb:
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

    private void UseFail()
    {
        audioSource.resource = useFailSound;
        audioSource.loop = false;
        audioSource.Play();
    }

    private void LightFuse()
    {
        if (!fuseLit)
        {
            audioSource.resource = fuseSound;
            audioSource.loop = true;
            audioSource.Play();
            fuseDecal.SetActive(true);
            fuseLight.SetActive(true);
            fuseLit = true;
        }
    }

    private void Explode()
    {
        Debug.Log("BOOM");
        // Explosion sound
        audioSource.resource = explosionSound;
        audioSource.loop = false;
        audioSource.Play();

        // Effects
        fuseDecal.SetActive(false);
        fuseLight.SetActive(false);
        fuseLit = false;
        //explosion effects

        Destroy(gameObject, 1);
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

        // if shotgun
        if (shotgun)
        {
            currentAmmo--;

            Vector2 projectileVector = (muzzle.position - ejector.position).normalized * projectileVelocity;

            GameObject spawnedProjectile = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);
            spawnedProjectile.name = "Center";
            spawnedProjectile.GetComponent<Rigidbody2D>().AddForce(projectileVector, ForceMode2D.Impulse);
            spawnedProjectile.GetComponent<ProjectileBehavior>().SetDamage(damage);

            Vector2 lastVector = projectileVector;

            // Going up
            for (int i = 0; i < (extraPellets / 2); i++)
            {
                Vector2 newVector = Quaternion.AngleAxis(spreadAngle, Vector2.right) * lastVector;
                GameObject spawnedPelletTop = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);
                spawnedPelletTop.name = "Top" + i;

                spawnedPelletTop.GetComponent<Rigidbody2D>().AddForce(newVector, ForceMode2D.Impulse);
                spawnedPelletTop.GetComponent<ProjectileBehavior>().SetDamage(damage);

                lastVector = newVector;
            }

            lastVector = projectileVector;
            // Going down
            for (int i = 0; i < (extraPellets / 2); i++)
            {
                Vector2 newVector = Quaternion.AngleAxis(-1 * spreadAngle, Vector2.right) * lastVector;
                GameObject spawnedPelletBottom = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);
                spawnedPelletBottom.name = "Bottom" + i;

                spawnedPelletBottom.GetComponent<Rigidbody2D>().AddForce(newVector, ForceMode2D.Impulse);
                spawnedPelletBottom.GetComponent<ProjectileBehavior>().SetDamage(damage);

                lastVector = newVector;
            }
        }
        else
        {
            // Spawn bullet
            currentAmmo--;

            GameObject spawnedProjectile = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);

            Vector2 projectileVector = (muzzle.position - ejector.position).normalized * projectileVelocity;

            spawnedProjectile.GetComponent<Rigidbody2D>().AddForce(projectileVector, ForceMode2D.Impulse);
            spawnedProjectile.GetComponent<ProjectileBehavior>().SetDamage(damage);
        }

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
        Destroy(spawnedCasing, 5);

        if (currentAmmo <= 0 && !reloading)
        {
            reloading = true;
            reloadTimer = 0f;
        }
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
        rb.angularVelocity = 0;
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
        hitDict = new Dictionary<Collider2D, bool>();
    }

    public void DeactivateCollider()
    {
        col.enabled = false;
        hitDict = new Dictionary<Collider2D, bool>();
    }

    private void OnDrawGizmos()
    {
        if (held && ItemType == HoldableType.Gun)
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