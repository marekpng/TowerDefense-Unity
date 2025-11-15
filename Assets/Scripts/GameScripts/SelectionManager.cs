using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionManager : MonoBehaviour
{
    [Header("UI")]
    public Canvas uiCanvas;                     // assign your HUD canvas
    public SellPanelController sellPanelPrefab; // assign SellPanel prefab (controller component on prefab)

    [Header("Settings")]
    public float uiYOffset = 1.5f;              // how high above turret to show panel

    private SellPanelController sellPanelInstance;

    void Start()
    {
        // instantiate panel under canvas, keep it inactive
        if (sellPanelPrefab != null && uiCanvas != null)
        {
            sellPanelInstance = Instantiate(sellPanelPrefab, uiCanvas.transform);
            sellPanelInstance.Hide();
        }
    }

    void Update()
    {
        // Right-click cancels selection (optional)
        if (Input.GetMouseButtonDown(1))
        {
            HidePanel();
            return;
        }

        // prevent clicks when pointer is over UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                // check if we clicked a turret (by component)
                TurretSell turret = hit.collider.GetComponentInParent<TurretSell>();
                if (turret != null)
                {
                    ShowPanelAtTurret(turret);
                }
                else
                {
                    // clicked something else -> hide panel
                    HidePanel();
                }
            }
        }
    }

    void ShowPanelAtTurret(TurretSell turret)
    {
        if (sellPanelInstance == null) return;

        // position in screen-space above turret
        Vector3 worldPos = turret.transform.position + Vector3.up * uiYOffset;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        // move panel: its RectTransform anchored to screen
        RectTransform rt = sellPanelInstance.GetComponent<RectTransform>();
        rt.position = screenPos;

        sellPanelInstance.Show(turret);
    }

    void HidePanel()
    {
        if (sellPanelInstance != null)
            sellPanelInstance.Hide();
    }
}
