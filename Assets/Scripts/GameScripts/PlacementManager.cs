using UnityEngine;
using UnityEngine.UI;

public class PlacementManager : MonoBehaviour
{
    [Header("Tower Types")]
    [Tooltip("Basic Tower - $100")]
    public GameObject basicTowerPrefab;   // TowerBase prefab
    [Tooltip("Rapid Tower - $150")]
    public GameObject rapidTowerPrefab;   // RapidFireCannon prefab
    [Tooltip("Heavy Tower - $200")]
    public GameObject heavyTowerPrefab;   // RapidFireCannon_2 prefab

    private int[] towerCosts = {100, 150, 200};  // Matches prefab order above

    [Header("UI Buttons")]
    public Button basicButton;
    public Button rapidButton;
    public Button heavyButton;

    private GameObject selectedPrefab;
    private int selectedCost;
    private bool placementMode = false;

    void Update()
{
    if (Input.GetMouseButtonDown(0) && placementMode && selectedPrefab != null)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            BuildSpot spot = hit.collider.GetComponent<BuildSpot>();
            if (spot != null && !spot.isOccupied)
            {
                // 💰 Spend money only when building is valid
                if (GameManager.Instance.SpendMoney(selectedCost))
                {
                    spot.PlaceTower(selectedPrefab);
                    TurretSell sell = spot.placedTower.GetComponent<TurretSell>();
                    if (sell != null) sell.cost = selectedCost;
                    Deselect();
                }
            }
        }
    }

    if (Input.GetMouseButtonDown(1))
        Deselect();
}


    // Called by Basic button OnClick
    public void SelectBasic() { SelectTower(0, basicTowerPrefab); }
    // Called by Rapid button OnClick
    public void SelectRapid() { SelectTower(1, rapidTowerPrefab); }
    // Called by Heavy button OnClick
    public void SelectHeavy() { SelectTower(2, heavyTowerPrefab); }

    private void SelectTower(int index, GameObject prefab)
    {
        selectedPrefab = prefab;
        selectedCost = towerCosts[index];
        placementMode = true;
        UpdateButtonFeedback();
    }

    private void Deselect()
    {
        selectedPrefab = null;
        placementMode = false;
        UpdateButtonFeedback();
    }

    private void UpdateButtonFeedback()
    {
        Color activeColor = Color.yellow;
        Color inactiveColor = Color.white;

        // Robust color update with null checks
        SetButtonColor(basicButton, placementMode && selectedPrefab == basicTowerPrefab ? activeColor : inactiveColor);
        SetButtonColor(rapidButton, placementMode && selectedPrefab == rapidTowerPrefab ? activeColor : inactiveColor);
        SetButtonColor(heavyButton, placementMode && selectedPrefab == heavyTowerPrefab ? activeColor : inactiveColor);
    }

    private void SetButtonColor(Button btn, Color c)
    {
        if (btn != null)
        {
            ColorBlock block = GetColorBlock(c);
            btn.colors = block;
        }
    }

    private ColorBlock GetColorBlock(Color normalColor)
    {
        ColorBlock block = new ColorBlock();
        block.normalColor = normalColor;
        block.highlightedColor = normalColor * 1.1f;  // Slight brighten on hover
        block.pressedColor = normalColor * 0.8f;     // Darken on click
        block.selectedColor = normalColor;
        block.colorMultiplier = 1f;
        block.fadeDuration = 0.1f;
        return block;
    }
}