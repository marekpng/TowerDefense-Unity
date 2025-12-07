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

    [Header("Logging")]
    public string zombieId;
    public int hitCount = 0;            // how many times zombie was hit
    public string zombieType = "default"; // fill externally if you have types
    public int waveNumber = 0;          // spawner should assign this
    public string lastHitTowerId = "";  // which tower made the final hit
    public int lastDamage = 0;          // damage that caused final death blow

    public event Action onDeath;

    void Start()
    {
        health = maxHealth;
        animator = GetComponentInChildren<Animator>();

        // Generate zombieId if missing
        if (string.IsNullOrEmpty(zombieId))
            zombieId = Guid.NewGuid().ToString();

        // Assign correct wave number
        waveNumber = WaveManager.CurrentWaveNumber;

        // Log zombie spawn
        if (LogManager.Instance != null)
        {
            LogManager.Instance.LogZombieSpawn(
                playerId: "player-default",
                zombieId: zombieId,
                position: transform.position
            );
        }
    }

    public void TakeDamage(int damage)
    {
        // Ensure waveNumber is always correct (backup)
        waveNumber = WaveManager.CurrentWaveNumber;

        if (isDead) return;
        int hpBefore = health;
        health -= damage;
        // track last damage applied
        lastDamage = damage;
        hitCount++;

        // Log ONLY first hit with detailed info
        if (hitCount == 1 && LogManager.Instance != null)
        {
            int hpAfter = health;
            LogManager.Instance.LogGenericEvent(
                playerId: "player-default",
                eventName: $"zombieFirstHit_hpBefore_{hpBefore}_hpAfter_{hpAfter}_damage_{damage}_wave_{waveNumber}_type_{zombieType}",
                towerId: lastHitTowerId,
                zombieId: zombieId,
                position: transform.position
            );
        }

        // update last hit tower (filled externally by tower)
        // LastHitTower must be set BEFORE calling TakeDamage

        if (health <= 0)
            Die(false); // normálna smrť (s animáciou)
    }

    // NOVÉ: silentKill = true → žiadna animácia, len despawn
    public void Die(bool silentKill = false)
    {
        if (isDead) return;
        isDead = true;

        // Log zombie death
        if (LogManager.Instance != null)
        {
            LogManager.Instance.LogGenericEvent(
                playerId: "player-default",
                eventName: $"zombieKilled_hp_0_hits_{hitCount}_finalDamage_{lastDamage}_wave_{waveNumber}_type_{zombieType}_killedBy_{lastHitTowerId}",
                towerId: lastHitTowerId,
                zombieId: zombieId,
                position: transform.position
            );
        }

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

        GameManager.Instance.AddMoney(4);

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