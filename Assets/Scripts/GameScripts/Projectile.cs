using UnityEngine;

public class Projectile : MonoBehaviour
{
    private int damage;
    public float speed = 50f;
    public float lifetime = 1.0f;

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
                int oldHp = 0; // placeholder for health.currentHP
                string enemyType = "Unknown"; // placeholder for health.enemyType

                // Try to get currentHP and enemyType if they exist
                var hpField = typeof(EnemyHealth).GetField("currentHP");
                if (hpField != null)
                {
                    oldHp = (int)hpField.GetValue(health);
                }
                var typeField = typeof(EnemyHealth).GetField("enemyType");
                if (typeField != null)
                {
                    enemyType = (string)typeField.GetValue(health);
                }

                health.TakeDamage(damage);

                // Log hit event
                GameLogger.Instance.LogEvent(
                    "enemyHit",
                    oldHp,
                    damage
                );

                // Log kill event if HP drops to 0 or below
                if (oldHp > 0 && oldHp - damage <= 0)
                {
                    GameLogger.Instance.LogEnemyKilled(
                        enemyType,   // enemyType or placeholder
                        -1,                 // turretId unknown here
                        0                   // reward unknown here
                    );
                }
            }
            Destroy(gameObject);
        }
    }
}