using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Path")]
    public Transform[] waypoints;
    [Header("Stats")]
    public float speed = 5f;

    [Header("Animator")]
    private Animator animator;  // Pridaj reference

    private int currentWaypoint = 0;
    private bool isAttacking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null) animator = GetComponentInChildren<Animator>();

        // Auto waypoints
        if (waypoints.Length == 0)
        {
            Transform path = GameObject.Find("Path")?.transform;
            if (path) {
                waypoints = new Transform[path.childCount];
                for (int i = 0; i < path.childCount; i++)
                    waypoints[i] = path.GetChild(i);
            }
        }

        // Start Run anim
        SetRunning(true);
    }

    void Update()
    {
        if (currentWaypoint < waypoints.Length && !isAttacking)
        {
            Vector3 direction = (waypoints[currentWaypoint].position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime, Space.World);

            if (Vector3.Distance(transform.position, waypoints[currentWaypoint].position) < 0.5f)
            {
                currentWaypoint++;
            }
        }
        else if (!isAttacking)
        {
            // Dosiahol koniec - Attack!
            StartCoroutine(AttackAndDestroy());
        }
    }

    void SetRunning(bool running)
    {
        if (animator) animator.SetBool("isRunning", running);
    }

    IEnumerator AttackAndDestroy()
    {
        isAttacking = true;
        SetRunning(false);
        if (animator) animator.SetBool("isAttacking", true);

        // Počkaj dĺžku animácie (Z_Attack ~1s)
        yield return new WaitForSeconds(1.2f);  // Nastav podľa anim (Animator > Length)

        Destroy(gameObject);  // Destroy po útoku
    }
}