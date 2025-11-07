using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Path")]
    public Transform[] waypoints; // Drag Path children here (0=spawn, last=end)

    [Header("Stats")]
    public float speed = 5f;

    private int currentWaypoint = 0;

    void Start()
    {
        // Auto-find waypoints from parent "Path" children
        if (waypoints.Length == 0)
        {
            Transform path = GameObject.Find("Path")?.transform;
            if (path) waypoints = new Transform[path.childCount];
            for (int i = 0; i < path.childCount; i++)
                waypoints[i] = path.GetChild(i);
        }
    }

    void Update()
    {
        if (currentWaypoint < waypoints.Length)
        {
            Vector3 direction = (waypoints[currentWaypoint].position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime, Space.World);

            if (Vector3.Distance(transform.position, waypoints[currentWaypoint].position) < 0.5f)
            {
                currentWaypoint++;
            }
        }
        else
        {
            // Reached end - handled by Base trigger
            Destroy(gameObject);
        }
    }
}