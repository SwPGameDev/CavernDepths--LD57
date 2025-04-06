using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour
{
    private InputAction leftClick;
    private InputAction rightClick;
    private InputAction interactAction;

    public ItemHolder itemHolder;
    public ItemBehavior heldItem;
    private Camera cam;

    [Header("Interact")]
    public GameObject interactButton;

    public float interactYOffset;
    public GameObject interactableIndicatorPrefab;
    public ItemBehavior closestItem = null;

    public float range = 5;
    public float indicatorRange = 2;
    public float pickUpRange = 1;
    public float refreshCooldown = 0.1f;
    private float refreshTimer = 0;
    public LayerMask itemLayerMask;

    [Header("Throw")]
    public float throwForce = 10;

    private void Start()
    {
        leftClick = InputSystem.actions.FindAction("Click");
        rightClick = InputSystem.actions.FindAction("RightClick");
        interactAction = InputSystem.actions.FindAction("Interact");

        cam = Camera.main;
    }

    private void Update()
    {
        // INTERACT
        if (refreshTimer > refreshCooldown)
        {
            refreshTimer = 0;
            closestItem = GetClosestItem();
        }
        else
        {
            refreshTimer += Time.deltaTime;
        }

        // USE ITEM
        if (leftClick.WasPressedThisFrame())
        {
            Debug.Log("LEFT CLICK");
            if (heldItem)
            {
                heldItem.TryUse();
            }
        }

        // THROW ITEM AT MOUSE POS
        if (rightClick.WasPressedThisFrame())
        {
            Debug.Log("RIGHT CLICK");
            // Throw item
        }

        // PICKUP OR DROP
        if (interactAction.WasPressedThisFrame())
        {
            Debug.Log("INTERACT");
            if (!heldItem)
            {
                // Get closest item if in range
                if (closestItem)
                {
                    closestItem.Pickup(itemHolder.gameObject);
                    heldItem = closestItem;
                    itemHolder.NewHeldItem(heldItem);
                }
            }
            else
            {
                heldItem.Drop();
                heldItem = null;
                itemHolder.ClearHeldItem();
            }
        }
    }

    private void LateUpdate()
    {
        if (!heldItem)
        {
            interactButton.SetActive(true);

            if (closestItem)
            {
                Vector3 offsetPos = new Vector3(closestItem.transform.position.x, closestItem.transform.position.y + interactYOffset, 0);

                interactButton.transform.position = cam.WorldToScreenPoint(offsetPos);
            }
            else
            {
                interactButton.SetActive(false);
            }
        }
        else
        {
            interactButton.SetActive(false);
        }
    }

    private ItemBehavior GetClosestItem()
    {
        ItemBehavior bestItem = null;
        float closestDistance = Mathf.Infinity;

        Collider2D[] nearbyItems = Physics2D.OverlapCircleAll(transform.position, range, itemLayerMask);

        if (nearbyItems.Length > 0)
        {
            foreach (Collider2D item in nearbyItems)
            {
                Debug.DrawLine(transform.position, item.transform.position, Color.yellow, refreshCooldown);


                float distanceToItem = (item.transform.position - transform.position).magnitude;
                if (distanceToItem < closestDistance)
                {
                    closestDistance = distanceToItem;
                    bestItem = item.GetComponent<ItemBehavior>();
                }
            }
        }

        return bestItem;
    }

    private void OnDrawGizmos()
    {
    }
}