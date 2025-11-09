using UnityEngine;
using System.Linq;

public class Tower : MonoBehaviour
{
    [Header("Stats")]
    public float range = 10f;
    public float fireRate = 1f;
    public int damage = 20;  // ZVÝŠENÉ na 20! (nastav v Inspectore ak chceš rôzne veže)

    public GameObject projectilePrefab;

    private Transform target;
    private float fireCountdown = 0f;

    void Update()
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, range, LayerMask.GetMask("Default"));
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
        Transform head = transform.Find("TurretHead");
        Vector3 spawnPos = head.position + head.forward * 1f;  // Offset: Žiadny trigger s TurretHead
        GameObject proj = Instantiate(projectilePrefab, spawnPos, head.rotation);
        Projectile p = proj.GetComponent<Projectile>();
        p.SetTarget(target, damage);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}