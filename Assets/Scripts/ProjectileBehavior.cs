using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    private float damage;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 7) // 7 is block layer
        {
            collision.gameObject.GetComponent<Block>().TakeHit(damage);
        }
        else if (collision.gameObject.layer == 9) // monster layer
        {
            collision.gameObject.GetComponent<EnemyBehavior>().TakeHit(damage);
        }

        // Hit effects

        Destroy(gameObject);
    }

    public void SetDamage(float damageParam)
    {
        damage = damageParam;
    }
}