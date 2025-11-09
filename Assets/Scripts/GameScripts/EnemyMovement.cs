using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Path")]
    public Transform[] waypoints;
    [Header("Stats")]
    public float speed = 5f;
    [Header("Rotation")]
    public float rotationSpeed = 10f;

    private Animator animator;
    private int currentWaypoint = 0;
    private bool isAttacking = false;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();

        // Auto-find waypoints if not assigned
        if (waypoints.Length == 0)
        {
            Transform path = GameObject.Find("Path")?.transform;
            if (path)
            {
                waypoints = new Transform[path.childCount];
                for (int i = 0; i < path.childCount; i++)
                    waypoints[i] = path.GetChild(i);
            }
        }

        // ✅ Start running immediately
        if (animator)
        {
            animator.SetBool("isRunning", true);
            animator.SetBool("isDead", false);
            animator.SetBool("isAttacking", false);
        }
    }

    void Update()
    {
        if (isDead) return;

        if (currentWaypoint < waypoints.Length && !isAttacking)
        {
            Vector3 direction = (waypoints[currentWaypoint].position - transform.position).normalized;

            // Smooth rotation
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Move forward
            transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);

            // Check distance to waypoint
            if (Vector3.Distance(transform.position, waypoints[currentWaypoint].position) < 0.5f)
                currentWaypoint++;
        }
        else if (!isAttacking)
        {
            StartCoroutine(AttackAndDestroy());
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        // Stop movement
        if (animator)
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isAttacking", false);
            animator.SetBool("isDead", true);
        }
    }

    IEnumerator AttackAndDestroy()
    {
        isAttacking = true;
        if (animator)
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isAttacking", true);
        }

        yield return new WaitForSeconds(1.2f);
        Destroy(gameObject);
    }
}
