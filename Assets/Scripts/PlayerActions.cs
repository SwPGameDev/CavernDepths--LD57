using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour
{
    private InputAction leftClick;
    private InputAction rightClick;
    private InputAction interactAction;

    public Transform itemHolder;
    public Transform itemOrbitPoint;
    public ItemBehavior heldItem;
    private Camera cam;

    [Header("Interact")]
    public GameObject interactButton;
    public float interactYOffset;
    //public GameObject interactableIndicatorPrefab;
    public ItemBehavior closestItem = null;
    public float pickUpRange = 1;
    //public float indicatorRange = 2;
    //public float pickUpRange = 1;
    public float refreshCooldown = 0.1f;
    private float refreshTimer = 0;
    public LayerMask itemLayerMask;

    [Header("Item Holder")]
    public float rotationItemHolderSpeed = 60;
    private Vector2 mousePos;
    Vector2 currentItemHolderPos;
    Vector2 rotateItemDirection;

    


    [Header("Throw")]
    public float throwStrength = 10;

    private void Start()
    {
        leftClick = InputSystem.actions.FindAction("Click");
        rightClick = InputSystem.actions.FindAction("RightClick");
        interactAction = InputSystem.actions.FindAction("Interact");

        cam = Camera.main;
    }

    private void Update()
    {
        // Rotate Item Holder
        mousePos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        currentItemHolderPos = new Vector2(itemOrbitPoint.position.x, itemOrbitPoint.position.y);
        rotateItemDirection = (mousePos - currentItemHolderPos).normalized;
        float angle = Mathf.Atan2(rotateItemDirection.y, rotateItemDirection.x) * Mathf.Rad2Deg - 90f;
        itemOrbitPoint.rotation = Quaternion.Slerp(itemOrbitPoint.rotation, Quaternion.AngleAxis(angle, Vector3.forward), rotationItemHolderSpeed * Time.deltaTime);



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
            if (heldItem)
            {
                heldItem.Throw(mousePos, throwStrength);
            }
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
                    closestItem.Pickup(gameObject, itemHolder.gameObject);
                    heldItem = closestItem;
                }
            }
            else
            {
                heldItem.Drop();
                heldItem = null;
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

        Collider2D[] nearbyItems = Physics2D.OverlapCircleAll(transform.position, pickUpRange, itemLayerMask);

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
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(mousePos, 0.25f);
    }
}