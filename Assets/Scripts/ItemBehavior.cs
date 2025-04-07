using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public float useCooldown = 0.25f;

    private float useTimer = 0f;
    private bool canUse = true;
    [SerializeField] private float damage;
    [SerializeField] private float mass;

    public bool FacingRight = true;

    // GUN
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

    // Bomb
    // explosive damage
    // radius
    // fuse timer
    // sticky

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

        currentAmmo = maxAmmoCount;
    }

    private void Update()
    {
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
                        reloading = true;
                        reloadTimer = 0f;
                    }

                    break;

                case ItemTypes.Bomb:
                    //anim.SetTrigger("light");
                    //LightFuse();
                    break;
            }

            canUse = false;
        }
    }

    private void Shoot()
    {
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

    public void Throw()
    {
        Debug.Log("THROW: " + gameObject.name);
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
        transform.localPosition = Vector3.zero;
    }

    public void Drop()
    {
        Debug.Log("DROP: " + gameObject.name);

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
                Ray two = new Ray(ejector.position, ejector.up * -1 * casingVelocity);
                Gizmos.DrawRay(two);
            }
        }
    }
}
