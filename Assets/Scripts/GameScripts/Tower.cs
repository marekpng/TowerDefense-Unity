using UnityEngine;
using System.Linq;

public class Tower : MonoBehaviour
{
    [Header("Stats")]
    public float range = 10f;
    public float fireRate = 1f;
    public int damage = 20;
    public GameObject projectilePrefab;

    private Transform target;
    private float fireCountdown = 0f;

    void Update()
    {
        // Check for enemies within range
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, range, LayerMask.GetMask("Default"));

        // Find closest alive enemy
        var enemy = enemiesInRange
            .Where(c => c.CompareTag("Enemy"))
            .Select(c => c.GetComponent<EnemyHealth>())
            .Where(h => h != null && !h.IsDead)
            .OrderBy(h => Vector3.Distance(transform.position, h.transform.position))
            .FirstOrDefault();

        if (enemy != null)
        {
            target = enemy.transform;

            // Rotate tower to face enemy
            transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));

            // Fire logic
            if (fireCountdown <= 0f)
            {
                Shoot();
                fireCountdown = 1f / fireRate;
            }
        }
        else
        {
            target = null;
        }

        fireCountdown -= Time.deltaTime;
    }

    void Shoot()
    {
        // Find turret head (optional child object for shooting direction)
        Transform head = transform.Find("TurretHead");
        Vector3 spawnPos = head ? head.position + head.forward * 1f : transform.position + transform.forward * 1f;

        // Spawn projectile
        GameObject proj = Instantiate(projectilePrefab, spawnPos, head ? head.rotation : transform.rotation);
        Projectile p = proj.GetComponent<Projectile>();
        if (p != null && target != null)
        {
            p.SetTarget(target, damage);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
