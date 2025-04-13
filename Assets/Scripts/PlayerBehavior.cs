using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBehavior : MonoBehaviour
{
    private static PlayerBehavior _instance;

    public static PlayerBehavior Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("PlayerBehavior is null");
            return _instance;
        }
    }


    public float maxHitPoints = 5;
    public float currentHitPoints;


    private void Awake()
    {
        _instance = this;
    }

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

    public void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
