using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target;
    private float damage;
    public float speed = 50f;

    public void SetTarget(Transform t, float dmg)
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

        if (Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            Hit();
        }
    }

    void Hit()
    {
        if (target != null)
        {
            target.GetComponent<EnemyHealth>().TakeDamage(damage);
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