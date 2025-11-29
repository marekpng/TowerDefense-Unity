using UnityEngine;
using System.Collections;
using System;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int health;
    private Animator animator;
    private bool isDead = false;

    [Header("Death Settings")]
    public float deathDelay = 0.2f;
    public bool IsDead => isDead;

    public event Action onDeath;

    void Start()
    {
        health = maxHealth;
        animator = GetComponentInChildren<Animator>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        health -= damage;
        if (health <= 0)
            Die(false); // normálna smrť (s animáciou)
    }

    // NOVÉ: silentKill = true → žiadna animácia, len despawn
    public void Die(bool silentKill = false)
    {
        if (isDead) return;
        isDead = true;

        if (TryGetComponent(out EnemyMovement move))
            move.Die();

        if (TryGetComponent(out Rigidbody rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (TryGetComponent(out Collider col))
            col.enabled = false;

        GameManager.Instance.AddMoney(10);

        // Hlásime Spawneru smrť (dôležité pre wave!)
        onDeath?.Invoke();

        if (silentKill)
        {
            // OKAMŽITÝ DESPAWN – žiadna animácia
            Destroy(gameObject);
        }
        else
        {
            // Normálna death animácia + fade
            StartCoroutine(DeathRoutine());
        }
    }

    IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(1.1f);
        if (animator != null)
            animator.enabled = false;

        float fadeTime = 0.5f;
        Vector3 start = transform.position;
        Vector3 end = start - Vector3.up;
        float elapsed = 0f;

        while (elapsed < fadeTime)
        {
            transform.position = Vector3.Lerp(start, end, elapsed / fadeTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}