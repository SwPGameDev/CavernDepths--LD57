using System.Collections.Generic;
using UnityEngine;

public class GetNearbyItems : MonoBehaviour
{
    public PlayerActions player;
    public float range = 5;
    public float refreshCooldown = 0.1f;
    private float refreshTimer = 0;
    public LayerMask itemLayerMask;

    private ItemBehavior closestItem = null;

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

    private ItemBehavior GetClosestItem()
    {
        ItemBehavior bestItem = null;
        float closestDistance = Mathf.Infinity;

        Collider2D[] nearbyItems = Physics2D.OverlapCircleAll(player.transform.position, range, itemLayerMask);

        if (nearbyItems.Length > 0)
        {
            foreach (Collider2D item in nearbyItems)
            {
                float distanceToItem = (item.transform.position - player.transform.position).magnitude;
                if (distanceToItem < closestDistance)
                {
                    closestDistance = distanceToItem;
                    bestItem = item.GetComponent<ItemBehavior>();
                }
            }
        }

        return bestItem;
    }
}