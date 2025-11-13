using UnityEngine;

public class BuildSpot : MonoBehaviour
{
    [HideInInspector] public bool isOccupied = false;
    [HideInInspector] public GameObject placedTower;  // FIXED: Teraz sa priradí v PlaceTower

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
            rend.material.color = Color.green; // Highlight available
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

        // Instantiate tower
        GameObject tower = Instantiate(towerPrefab, transform.position, Quaternion.identity);
        tower.transform.SetParent(transform, worldPositionStays: true);  // Parent pre organizáciu

        // Y offset: Na vrch spotu (uprav 0.5f podľa prefab veľkosti)
        Renderer towerRend = tower.GetComponent<Renderer>();
        if (towerRend != null)
        {
            Vector3 pos = tower.transform.position;
            pos.y = transform.position.y + towerRend.bounds.extents.y;
            tower.transform.position = pos;
        }

        placedTower = tower;  // FIXED: Priradí reference – PlacementManager môže GetComponent<TurretSell>()

        isOccupied = true;
        if (rend != null) rend.material.color = Color.red;  // Occupied = red
    }

    /// <summary>
    /// Frees the build spot (sell/destroy tower).
    /// </summary>
    public void ClearSpot()
    {
        if (placedTower != null)
        {
            Destroy(placedTower);  // Zničí tower
        }
        isOccupied = false;
        placedTower = null;
        if (rend != null) rend.material.color = originalColor;
    }
}