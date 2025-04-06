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

    // STATS
    public float useCooldown = 0.25f;
    private float useTimer = 0f;
    private bool canUse = true;

    // GUN
    public int maxAmmoCount = 1;
    private int currentAmmo;
    bool reloading = false;
    public float reloadCooldown = 0.75f;
    float reloadTimer = 0f;


    private Animator anim;
    private Collider2D col;
    private Rigidbody2D rb;

    private Dictionary<Collider2D, bool> hitDict = new Dictionary<Collider2D, bool>();

    [SerializeField] private float damage;

    private void Start()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

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
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (ItemType == ItemTypes.Melee)
        {
            if (collider.gameObject.layer == 7)
            {
                if (!hitDict.ContainsKey(collider))
                {
                    hitDict.Add(collider, true);
                    collider.GetComponent<Block>().TakeHit(damage);
                }
            }
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
                    Shoot();
                    break;
                case ItemTypes.Bomb:
                    //anim.SetTrigger("light");
                    //LightBomb();
                    break;
            }

            canUse = false;
        }
    }

    void Shoot()
    {
        // Spawn bullet

    }

    public void Throw()
    {
        Debug.Log("THROW: " + gameObject.name);
    }

    public void Pickup(GameObject holder)
    {
        Debug.Log("Holder: " + holder.name + " Pickup: " + gameObject.name);

        held = true;
        col.isTrigger = true;
        rb.linearVelocity = Vector3.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        gameObject.layer = 0; // 0 is default
        transform.parent = holder.transform;
        transform.localPosition = Vector3.zero;
    }

    public void Drop()
    {
        Debug.Log("DROP: " + gameObject.name);

        held = false;
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
}