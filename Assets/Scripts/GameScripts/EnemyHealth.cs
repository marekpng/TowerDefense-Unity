using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 50f;
    public float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
		GameManager.Instance.AddMoney(10);
        Destroy(gameObject);
    }
}