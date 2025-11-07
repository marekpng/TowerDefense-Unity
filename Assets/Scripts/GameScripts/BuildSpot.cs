using UnityEngine;

public class BuildSpot : MonoBehaviour
{
    [HideInInspector] public bool isOccupied = false;   // One tower per spot
    [HideInInspector] public GameObject placedTower;    // Reference to the tower on this spot

    private Renderer rend;
    private Color originalColor;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
            originalColor = rend.material.color;
    }

    void OnMouseEnter()
    {
        if (rend != null && !isOccupied)
            rend.material.color = Color.green; // Highlight available spot
    }

    void OnMouseExit()
    {
        if (rend != null)
            rend.material.color = originalColor;
    }

    /// <summary>
    /// Places a tower on this build spot.
    /// </summary>
    public void PlaceTower(GameObject towerPrefab)
	{
    if (isOccupied) return;

    // Instantiate tower slightly above the build spot
    GameObject tower = Instantiate(towerPrefab, transform.position, Quaternion.identity);

    // Optional: parent it for organization (but keep world position)
    tower.transform.SetParent(transform, worldPositionStays: true);

    // Adjust Y offset if needed (you can tweak 0.5f or 1f depending on prefab size)
    Vector3 pos = tower.transform.position;
    pos.y = transform.position.y + tower.GetComponent<Renderer>().bounds.extents.y;
    tower.transform.position = pos;

    isOccupied = true;
    rend.material.color = Color.red;
	}




    /// <summary>
    /// Frees the build spot when a tower is sold or destroyed.
    /// </summary>
    public void ClearSpot()
    {
        isOccupied = false;
        placedTower = null;

        if (rend != null)
            rend.material.color = originalColor;
    }
}
