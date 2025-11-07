using UnityEngine;
using UnityEngine.UI;
using TMPro; // ✅ Add this for TextMeshPro

public class SellPanelController : MonoBehaviour
{
    public Button sellButton;
    public TextMeshProUGUI sellText; // ✅ TMP text instead of regular UI text

    private TurretSell currentTurret;

    void Awake()
    {
        if (sellButton != null)
            sellButton.onClick.AddListener(OnSellClicked);
    }

    public void Show(TurretSell turret)
    {
        currentTurret = turret;
        gameObject.SetActive(true);

        int refund = Mathf.FloorToInt(turret.cost * 0.5f);

        if (sellText != null)
            sellText.text = $"Sell (${refund})";
    }

    public void Hide()
    {
        currentTurret = null;
        gameObject.SetActive(false);
    }

    private void OnSellClicked()
    {
        if (currentTurret != null)
        {
            currentTurret.Sell();
            Hide();
        }
    }

    void OnDestroy()
    {
        if (sellButton != null)
            sellButton.onClick.RemoveListener(OnSellClicked);
    }
}
