using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Path")]
    public Transform[] waypoints;

    [Header("Stats")]
    public float speed = 5f;
    public float groundOffset = 0.0f; // ← NOVÉ: Výška od zeme (uprav v Inspectore!)

    [Header("Rotation")]
    public float rotationSpeed = 10f;

    private Animator animator;
    private int currentWaypoint = 0;
    private bool isAttacking = false;
    private bool isDead = false;

    void Start()
    {
    animator = GetComponentInChildren<Animator>();

        // Auto-find waypoints
        if (waypoints == null || waypoints.Length == 0)
        {
            Transform path = GameObject.Find("Path")?.transform;
            if (path)
            {
                waypoints = new Transform[path.childCount];
                for (int i = 0; i < path.childCount; i++)
                    waypoints[i] = path.GetChild(i);
            }
        }

        // Spusti beh
        if (animator)
        {
            animator.SetBool("isRunning", true);
            animator.SetBool("isDead", false);
            animator.SetBool("isAttacking", false);
        }

        // VYNÚTIŤ POZÍCIU NA ZEM (pri štarte)
        AdjustHeightToGround();
    }

    void Update()
    {
        if (isDead || waypoints == null || waypoints.Length == 0) return;

        if (currentWaypoint < waypoints.Length && !isAttacking)
        {
            Vector3 targetPos = waypoints[currentWaypoint].position;
            targetPos.y = transform.position.y; // Zachovaj aktuálnu výšku (groundOffset)

            Vector3 direction = (targetPos - transform.position).normalized;

            // Smooth rotation
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Pohyb vpred (po zemi)
            transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);

            // Udrž výšku (groundOffset)
            AdjustHeightToGround();

            // Prejdenie waypointu
            if (Vector3.Distance(transform.position, waypoints[currentWaypoint].position) < 0.5f)
                currentWaypoint++;
        }
        else if (!isAttacking)
        {
            StartCoroutine(AttackAndDestroy());
        }
    }

    void AdjustHeightToGround()
    {
        // Udržuje zombie na zemi + groundOffset
        Vector3 pos = transform.position;
        pos.y = waypoints[0].position.y + groundOffset; // Použi Y prvého waypointu + offset
        transform.position = pos;
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
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