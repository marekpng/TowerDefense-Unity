using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int health;
    private Animator animator;
    private bool isDead = false;

    [Header("Death Settings")]
    public float deathDelay = 0.2f; // short delay before despawn

    public bool IsDead => isDead; // read-only for Tower script

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
            Die();
    }

    void Die()
{
    if (isDead) return;
    isDead = true;

    if (TryGetComponent(out EnemyMovement move))
    {
        move.Die(); // ✅ trigger animation through the movement script
    }

    if (TryGetComponent(out Rigidbody rb))
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
    }
    if (TryGetComponent(out Collider col)) col.enabled = false;

    GameManager.Instance.AddMoney(10);

    StartCoroutine(DeathRoutine());
}

IEnumerator DeathRoutine()
{
    yield return new WaitForSeconds(1.1f);  // Wait full animation length exactly

    animator.enabled = false;  // Freeze animator immediately to stop any transitions

    // Optional fade/sink effect (keep as you want)
    float fadeTime = 0.5f;
    Vector3 start = transform.position;
    Vector3 end = start - new Vector3(0, 1f, 0);
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
