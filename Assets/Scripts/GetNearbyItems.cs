using System.Collections.Generic;
using UnityEngine;

public class GetNearbyItems : MonoBehaviour
{
    public PlayerActions player;
    public float range = 5;

    public float refreshCooldown = 0.1f;
    private float refreshTimer = 0;
    private List<ItemBehavior> nearbyItems = new List<ItemBehavior>();

    private ItemBehavior closestItem = null;

    CircleCollider2D col;

    private void Start()
    {
        col = GetComponent<CircleCollider2D>();
        col.radius = range;
    }

    private void Update()
    {
        if (refreshTimer > refreshCooldown)
        {
            refreshTimer = 0;
            closestItem = GetClosestItem();
            if (closestItem)
            {
                player.closestItem = closestItem;
            }
        }
        else
        {
            refreshTimer += Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 8 && other.TryGetComponent<ItemBehavior>(out ItemBehavior item)) // 8 is item
        {
            if (!nearbyItems.Contains(item))
            {
                nearbyItems.Add(item);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 8 && other.TryGetComponent<ItemBehavior>(out ItemBehavior item)) // 8 is item
        {
            if (nearbyItems.Contains(item))
            {
                nearbyItems.Remove(item);
            }
        }
    }

    private ItemBehavior GetClosestItem()
    {
        ItemBehavior bestItem = null;
        float closestDistance = Mathf.Infinity;

        if (nearbyItems.Count > 0)
        {
            foreach (ItemBehavior item in nearbyItems)
            {
                float distanceToItem = (item.transform.position - player.transform.position).magnitude;
                if (distanceToItem < closestDistance)
                {
                    closestDistance = distanceToItem;
                    bestItem = item;
                }
            }
        }

        return bestItem;
    }
}