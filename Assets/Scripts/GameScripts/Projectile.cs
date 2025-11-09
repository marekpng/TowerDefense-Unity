using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target;
    private int damage;
    public float speed = 50f;

    public void SetTarget(Transform t, int dmg)
    {
        target = t;
        damage = dmg;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = target.position - transform.position;
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);

        if (Vector3.Distance(transform.position, target.position) < 1f)
        {
            Hit();
        }
    }

    void Hit()
    {
        if (target != null)
        {
            EnemyHealth health = target.GetComponent<EnemyHealth>();
            if (health) health.TakeDamage(damage);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Hit();
        }
    }
}