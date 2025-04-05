using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerActions : MonoBehaviour
{
    private InputAction leftClick;
    private InputAction rightClick;
    private InputAction interactAction;

    public ItemHolder itemHolder;
    public ItemBehavior heldItem;

    public GameObject interactButton;
    public float interactYOffset;
    public GameObject interactableIndicatorPrefab;
    public GameObject sphereOfInfluence;
    public ItemBehavior closestItem = null;
    public float range = 5;
    public float indicatorRange = 2;
    public float pickUpRange = 1;

    private Camera cam;

    private void Start()
    {
        leftClick = InputSystem.actions.FindAction("Click");
        rightClick = InputSystem.actions.FindAction("RightClick");
        interactAction = InputSystem.actions.FindAction("Interact");

        cam = Camera.main;
    }

    private void Update()
    {
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
        }
        else
        {
            interactButton.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
    }
}