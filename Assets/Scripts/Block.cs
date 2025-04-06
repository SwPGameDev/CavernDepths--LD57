using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] float maxHitPoints = 5;
    public float currentHitPoints;

    public bool ouchy = false;
    public float ouchyAmount = 0.5f;
    public float ouchyTickTime = 0.25f;
    public float ouchyTimer = 0;

    // Drop loot chance
    // Drop loot table

    void Start()
    {
        currentHitPoints = maxHitPoints;
    }

    public void TakeHit(float damage)
    {
        currentHitPoints -= damage;
        if (currentHitPoints <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        DropLoot();
        Destroy(gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6) // 6 is player layer
        {
            if (ouchy)
            {
                ouchyTimer += Time.deltaTime;
                if (ouchyTimer > ouchyTickTime)
                {
                    collision.gameObject.GetComponent<PlayerBehavior>().TakeHit(ouchyAmount);
                }
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 6) // 6 is player layer
        {
            if (ouchy)
            {
                ouchyTimer = 0;
            }
        }
    }

    void DropLoot()
    {

    }
}
