using UnityEngine;
using System.Linq;
using System.Collections;

public class Tower : MonoBehaviour
{
    [Header("Stats")]
    public float range = 10f;
    public float fireRate = 1f;
    public int damage = 20;
    public GameObject projectilePrefab;

    [Header("Rapid Burst")]
    public float burstDelay = 0.05f;

    [Header("Aim Height")]
    public float zombieAimHeight = 1.5f;  // Výška torza (uprav v Inspectore!)

    [Header("Debug")]
    public bool showShootPoints = true;

    private Transform target;
    private float fireCountdown = 0f;
    private Transform[] shootPoints;
    private bool initialized = false;

    void Start() { InitializeShootPoints(); }
    void OnEnable() { InitializeShootPoints(); }

    void InitializeShootPoints()
    {
        if (initialized) return;
        initialized = true;

        Transform turretHead = transform.Find("TurretHead");
        if (turretHead == null)
        {
            Debug.LogError("TurretHead not found!", this);
            shootPoints = new Transform[] { transform };
            return;
        }

        shootPoints = turretHead.GetComponentsInChildren<Transform>(true)
            .Where(t => t.name.StartsWith("shootPoint"))
            .OrderBy(t => GetShootPointIndex(t.name))
            .ToArray();

        if (shootPoints.Length == 0)
        {
            Debug.LogWarning("No shootPoint found. Using TurretHead.", this);
            shootPoints = new Transform[] { turretHead };
        }
    }

    int GetShootPointIndex(string name)
    {
        string numStr = name.Replace("shootPoint", "").Trim();
        if (string.IsNullOrEmpty(numStr)) return 1;
        int.TryParse(numStr, out int num);
        return num > 0 ? num : 999;
    }

    void Update()
    {
        if (!initialized) return;

        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, range, LayerMask.GetMask("Default"));
        var enemy = enemiesInRange
            .Where(c => c.CompareTag("Enemy"))
            .Select(c => c.GetComponent<EnemyHealth>())
            .Where(h => h != null && !h.IsDead)
            .OrderBy(h => Vector3.Distance(transform.position, h.transform.position))
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
        else target = null;

        fireCountdown -= Time.deltaTime;
    }

    void Shoot()
    {
        if (shootPoints.Length == 1)
            FireFromShootPoint(shootPoints[0]);
        else if (shootPoints.Length == 2)
        {
            FireFromShootPoint(shootPoints[0]);
            StartCoroutine(FireDelayed(shootPoints[1], burstDelay));
        }
        else
        {
            int index = GetAlternatingIndex(shootPoints.Length);
            FireFromShootPoint(shootPoints[index]);
        }
    }

    private int alternatingIndex = 0;
    int GetAlternatingIndex(int count)
    {
        int idx = alternatingIndex;
        alternatingIndex = (alternatingIndex + 1) % count;
        return idx;
    }

    IEnumerator FireDelayed(Transform shootPoint, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (target != null) FireFromShootPoint(shootPoint);
    }

    void FireFromShootPoint(Transform shootPoint)
{
    Vector3 zombieTorso = target.position + Vector3.up * zombieAimHeight;
    Vector3 shootDirection = (zombieTorso - shootPoint.position).normalized;

    GameObject proj = Instantiate(projectilePrefab, shootPoint.position, Quaternion.LookRotation(shootDirection));
    Projectile p = proj.GetComponent<Projectile>();
    if (p != null)
    {
        p.SetTarget(shootDirection, damage);
    }
    GameLogger.Instance.LogEvent(
        "towerShot",
        transform.position.x,
        transform.position.y,
        transform.position.z,
        damage
    );
}

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    void OnDrawGizmos()
    {
        if (!showShootPoints || shootPoints == null || target == null) return;

        foreach (Transform sp in shootPoints)
        {
            if (sp != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(sp.position, 0.08f);
                
                // SMER NA TORZO!
                Vector3 zombieTorso = target.position + Vector3.up * zombieAimHeight;
                Gizmos.color = Color.green;
                Gizmos.DrawLine(sp.position, zombieTorso);
            }
        }
    }
}