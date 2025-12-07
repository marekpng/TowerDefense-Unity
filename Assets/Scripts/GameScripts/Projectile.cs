using UnityEngine;

public class Projectile : MonoBehaviour
{
    private int damage;
    public float speed = 50f;
    public float lifetime = 1.0f;

    public string towerId; // assigned by Tower.cs when firing

    private Vector3 direction;
    private float timer;

    public void SetTarget(Vector3 dir, int dmg)
    {
        direction = dir.normalized;
        damage = dmg;
        timer = lifetime;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth health = other.GetComponent<EnemyHealth>();
            if (health != null)
            {
                // Log projectile hit (DPS tracking)
                if (LogManager.Instance != null)
                {
                    LogManager.Instance.LogGenericEvent(
                        playerId: "player-default",
                        eventName: $"projectileHit_damage_{damage}",
                        towerId: towerId,
                        zombieId: health != null ? health.zombieId : null,
                        position: transform.position
                    );
                }
                health.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}