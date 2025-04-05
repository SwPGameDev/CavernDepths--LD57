using System.Collections.Generic;
using UnityEngine;

public class ItemBehavior : MonoBehaviour
{
    public enum ItemTypes
    {
        None,
        Pickaxe,
        Gun,
        Bomb
    }

    public ItemTypes ItemType = ItemTypes.None;
    private bool held = false;

    private bool canUse = true;
    public float useCooldown = 0.25f;
    private float useTimer = 0f;

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
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.layer == 7)
        {
            if (!hitDict.ContainsKey(collider))
            {
                hitDict.Add(collider, true);
                collider.GetComponent<Block>().TakeHit(damage);
            }
        }

        foreach (KeyValuePair<Collider2D, bool> items in hitDict)
        {
            Debug.Log("HitDict: " + items.Key + " " + items.Value);
        }
    }

    public void TryUse()
    {
        Debug.Log("USE: " + gameObject.name);

        if (canUse)
        {
            switch (ItemType)
            {
                case ItemTypes.Pickaxe:
                    anim.SetTrigger("swing");
                    break;
                case ItemTypes.Gun:
                    //anim.SetTrigger("shoot");
                    //Shoot();
                    break;
                case ItemTypes.Bomb:
                    //anim.SetTrigger("light");
                    //LightBomb();
                    break;
            }

            canUse = false;
        }
    }

    public void Throw()
    {
        Debug.Log("THROW: " + gameObject.name);
    }

    public void Pickup(GameObject holder)
    {
        Debug.Log("Holder: " + holder.name + " Pickup: " + gameObject.name);
        col.isTrigger = true;
        rb.bodyType = RigidbodyType2D.Kinematic;
        transform.parent = holder.transform;
        transform.localPosition = Vector3.zero;
    }

    public void Drop()
    {
        Debug.Log("DROP: " + gameObject.name);
        col.isTrigger = false;
        col.enabled = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
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