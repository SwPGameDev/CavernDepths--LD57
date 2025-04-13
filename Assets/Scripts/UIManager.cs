using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public PlayerActions playerActions;
    public PlayerBehavior playerBehavior;
    public TextMeshProUGUI ammoCountText;
    public Slider PlayerHPBar;

    private void Start()
    {
        PlayerHPBar.maxValue = playerBehavior.maxHitPoints;
        UpdateAmmoCount();

    }

    private void Update()
    {
        UpdateAmmoCount();
    }


    public void UpdateAmmoCount()
    {
        if (playerBehavior)
        {
            PlayerHPBar.value = playerBehavior.currentHitPoints;
        }

        if (playerActions && playerActions.heldItem != null)
        {
            if (playerActions.heldItem.ItemType == HoldableType.Melee)
            {
                ammoCountText.text = "--";
            }
            else if (playerActions.heldItem.ItemType == HoldableType.Gun)
            {
                ItemBehavior itemBehavior = playerActions.heldItem.HoldableObject.GetComponent<ItemBehavior>();
                ammoCountText.text = itemBehavior.currentAmmo + " / " + itemBehavior.maxAmmoCount;
            }
            else if (playerActions.heldItem.ItemType == HoldableType.Bomb)
            {
                ammoCountText.text = "--";
            }
        }
        else if (playerActions.heldItem == null)
        {
            ammoCountText.text = "--";
        }
    }
}
