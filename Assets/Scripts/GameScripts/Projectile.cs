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
                health.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}