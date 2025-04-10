using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public PlayerActions playerActions;
    public TextMeshProUGUI ammoCountText;

    private void Start()
    {
        if (playerActions && playerActions.heldItem)
        {
            if (playerActions.heldItem.ItemType == ItemBehavior.ItemTypes.Melee)
            {
                ammoCountText.text = "--";
            }
            else if (playerActions.heldItem.ItemType == ItemBehavior.ItemTypes.Gun)
            {
                ammoCountText.text = playerActions.heldItem.currentAmmo + " / " + playerActions.heldItem.maxAmmoCount;
            }
            else if (playerActions.heldItem.ItemType == ItemBehavior.ItemTypes.Bomb)
            {
                ammoCountText.text = "--";
            }
        }

    }

    private void Update()
    {
        if (playerActions && playerActions.heldItem)
        {
            if (playerActions.heldItem.ItemType == ItemBehavior.ItemTypes.Melee)
            {
                ammoCountText.text = "--";
            }
            else if (playerActions.heldItem.ItemType == ItemBehavior.ItemTypes.Gun)
            {
                ammoCountText.text = playerActions.heldItem.currentAmmo + " / " + playerActions.heldItem.maxAmmoCount;
            }
            else if (playerActions.heldItem.ItemType == ItemBehavior.ItemTypes.Bomb)
            {
                ammoCountText.text = "--";
            }
        }
    }
}
