using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int health;
    private Animator animator;

    void Start()
    {
        health = maxHealth;
        animator = GetComponentInChildren<Animator>();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // ZASTAV POHYB!
        EnemyMovement movement = GetComponent<EnemyMovement>();
        if (movement) movement.enabled = false;

        GameManager.Instance.AddMoney(10);
        StartCoroutine(DeathAnimation());
    }

    IEnumerator DeathAnimation()
    {
        if (animator)
        {
            animator.SetBool("isDead", true);
            animator.SetBool("isRunning", false);
        }
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }
}