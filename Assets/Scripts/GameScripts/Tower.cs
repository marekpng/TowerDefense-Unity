using UnityEngine;
using System.Linq;

public class Tower : MonoBehaviour
{
    [Header("Stats")]
    public float range = 10f;
    public float fireRate = 1f;
    public float damage = 25f;
    public GameObject projectilePrefab; // Create later

    private Transform target;
    private float fireCountdown = 0f;

    void Update()
    {
        // Find nearest enemy in range
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, range, LayerMask.GetMask("Default")); // Enemy layer default
        var enemy = enemiesInRange
            .Where(c => c.CompareTag("Enemy"))
            .OrderBy(c => Vector3.Distance(transform.position, c.transform.position))
            .FirstOrDefault();

        if (enemy != null)
        {
            target = enemy.transform;
            transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));

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
        // Instantiate projectile at turret head
        GameObject proj = Instantiate(projectilePrefab, transform.Find("TurretHead").position, Quaternion.identity);
        Projectile p = proj.GetComponent<Projectile>();
        p.SetTarget(target, damage);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}