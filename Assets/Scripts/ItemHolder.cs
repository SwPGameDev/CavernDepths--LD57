using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    public GameObject player;
    public PlayerActions playerActions;

    public ItemBehavior heldItem;
    private SpriteRenderer itemSprite;

    private void Start()
    {
        playerActions = player.GetComponent<PlayerActions>();

        if (playerActions.heldItem)
        {
            heldItem = playerActions.heldItem;
            itemSprite = heldItem.GetComponent<SpriteRenderer>();
        }
    }

    private void Update()
    {
        if (heldItem)
        {
            float dot = Vector2.Dot(player.transform.right, (heldItem.transform.position - player.transform.position).normalized);
            if (dot > 0)
            {
                itemSprite.flipY = false;
                //heldItem.transform.rotation = Quaternion.Euler(heldItem.transform.rotation.eulerAngles.x, 0, heldItem.transform.rotation.eulerAngles.z);
            }
            else
            {
                itemSprite.flipY = true;
                //heldItem.transform.rotation = Quaternion.Euler(heldItem.transform.rotation.eulerAngles.x, 180, heldItem.transform.rotation.eulerAngles.z);
            }
        }
    }

    public void NewHeldItem(ItemBehavior newItem)
    {
        heldItem = newItem;
        itemSprite = heldItem.GetComponent<SpriteRenderer>();
    }

    public void ClearHeldItem()
    {
        heldItem = null;
        itemSprite = null;
    }
}