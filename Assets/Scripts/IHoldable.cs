using UnityEngine;

public interface IHoldable
{
    public GameObject HoldableObject { get; }
    public HoldableType ItemType { get; }
    public void Pickup(GameObject ownerParam, GameObject parentTransform);
    public void Drop();
    public void TryUse();
    public void Throw(Vector2 targetPos, float throwStrength);
}


public enum HoldableType
{
    None,
    Melee,
    Gun,
    Bomb,
    Treasure
}