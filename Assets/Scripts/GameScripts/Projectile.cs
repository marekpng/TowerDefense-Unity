using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 50f;
    public float turnSpeed = 3000f;
    public float lifetime = 3f;
    public float hitRadius = 2f; // ← NOVÉ: vzdialenosť, pri ktorej projektil trafí cieľ

    private int damage;
    private Transform target;
    private float timer;

    public string towerId;

    public void SetTarget(Transform targetTransform, int dmg)
    {
        target = targetTransform;
        damage = dmg;
        timer = lifetime;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // NOVÉ: SNAP HIT na blízku vzdialenosť
        if (Vector3.Distance(transform.position, target.position) <= hitRadius)
        {
            HitTarget();
            return;
        }

        Vector3 direction = (target.position - transform.position).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );

        transform.position += transform.forward * speed * Time.deltaTime;

        timer -= Time.deltaTime;
        if (timer <= 0f)
            Destroy(gameObject);
    }

    private void HitTarget()
    {
        EnemyHealth health = target.GetComponent<EnemyHealth>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        if (other.transform != target) return;

        HitTarget();
    }
}
