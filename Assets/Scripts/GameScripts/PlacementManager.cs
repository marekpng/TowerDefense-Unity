using UnityEngine;
using UnityEngine.UI;

public class PlacementManager : MonoBehaviour
{
    [Header("Towers")]
    public GameObject selectedTowerPrefab;   // Set by UI buttons
    public GameObject turretPrefab;          // Default turret prefab
    public int towerCost = 50;

    [Header("Layers")]
    [Tooltip("Select the Ground layer used for placement.")]
    public LayerMask groundLayer = 1 << 3;   // Ground layer (Layer 3)

    [Header("UI Feedback")]
    public Button turretButton;              // Assign TurretButton in Inspector

    private bool placementMode = false;

    void Update()
    {
        if (Camera.main == null)
            return;

        // --- LEFT CLICK: place turret ---
        if (Input.GetMouseButtonDown(0) && placementMode && selectedTowerPrefab != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
            {
                if (GameManager.Instance.SpendMoney(towerCost))
                {
                    Vector3 spawnPos = hit.point + Vector3.up * 0.5f; // Lift a bit above ground
                    Instantiate(selectedTowerPrefab, spawnPos, Quaternion.identity);
                    Deselect();
                }
            }
        }

        // --- RIGHT CLICK: cancel placement ---
        if (Input.GetMouseButtonDown(1))
        {
            Deselect();
        }
    }

    // Called by turret button
    public void SelectTurret()
    {
        selectedTowerPrefab = turretPrefab;
        placementMode = true;
        UpdateButtonFeedback();
    }

    private void Deselect()
    {
        selectedTowerPrefab = null;
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
