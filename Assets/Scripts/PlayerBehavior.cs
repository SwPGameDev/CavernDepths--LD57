using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBehavior : MonoBehaviour
{
    [SerializeField] float maxHitPoints = 5;
    public float currentHitPoints;
    
    void Start()
    {
        currentHitPoints = maxHitPoints;
    }

    void Update()
    {
        
    }


    public void TakeHit(float damage)
    {
        currentHitPoints -= damage;
        if (currentHitPoints <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
