using UnityEngine;
using UnityEngine.UI;

public class PlacementManager : MonoBehaviour
{
    [Header("Tower Placement")]
    [Tooltip("Prefab that includes both the tower base and turret.")]
    public GameObject turretPrefab;          // Combined prefab (Tower + Turret)
    public int towerCost = 50;

    [Header("UI")]
    [Tooltip("Assign the turret button from your UI here.")]
    public Button turretButton;              // Button in bottom center UI

    private bool placementMode = false;

    void Update()
    {
        if (Camera.main == null)
            return;

        // --- LEFT CLICK: Place turret on a BuildSpot ---
        if (Input.GetMouseButtonDown(0) && placementMode && turretPrefab != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                BuildSpot spot = hit.collider.GetComponent<BuildSpot>();

                if (spot != null && !spot.isOccupied)
                {
                    // Check if player can afford
                    if (GameManager.Instance.SpendMoney(towerCost))
                    {
                        // Place tower
                        spot.PlaceTower(turretPrefab);

                        // Pass cost info to the TurretSell script (for refunds)
                        if (spot.placedTower != null)
                        {
                            TurretSell sell = spot.placedTower.GetComponent<TurretSell>();
                            if (sell != null)
                                sell.cost = towerCost;
                        }

                        Deselect();
                    }
                    else
                    {
                        Debug.Log("❌ Not enough money to place tower!");
                    }
                }
                else
                {
                    Debug.Log("⚠️ Not a valid build spot or already occupied!");
                }
            }
        }

        // --- RIGHT CLICK: Cancel placement ---
        if (Input.GetMouseButtonDown(1))
        {
            Deselect();
        }
    }

    // Called by the turret UI button
    public void SelectTurret()
    {
        placementMode = true;
        UpdateButtonFeedback();
    }

    private void Deselect()
    {
        placementMode = false;
        UpdateButtonFeedback();
    }

    private void UpdateButtonFeedback()
    {
        if (turretButton != null)
        {
            var colors = turretButton.colors;
            colors.normalColor = placementMode ? Color.yellow : Color.white;
            turretButton.colors = colors;
        }
    }
}
