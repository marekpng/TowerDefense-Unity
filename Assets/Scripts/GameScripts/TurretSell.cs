using UnityEngine;

public class TurretSell : MonoBehaviour
{
    [Tooltip("Original purchase cost of this turret.")]
    public int cost = 50;

    private BuildSpot parentSpot;

    private void Start()
    {
        // Find the BuildSpot this tower is placed on
        parentSpot = GetComponentInParent<BuildSpot>();
    }

    public void Sell()
    {
        int refund = Mathf.FloorToInt(cost * 0.5f);
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddMoney(refund);
        }

        // ✅ Reset the build spot
        if (parentSpot != null)
        {
            parentSpot.isOccupied = false;
            parentSpot.placedTower = null;  // optional, safe cleanup
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        // Safety check: even if destroyed without Sell(), free the spot
        if (parentSpot != null)
        {
            parentSpot.isOccupied = false;
            parentSpot.placedTower = null;
        }
    }
}
