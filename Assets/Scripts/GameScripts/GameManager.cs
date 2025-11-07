using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("Player")]
    public int playerHP = 100;
    public int money = 200;
    
    [Header("UI")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI moneyText;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    void Start()
    {
        UpdateUI();
    }
    
    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }
    
    void UpdateUI()
    {
        hpText.text = "HP: " + playerHP;
        moneyText.text = "$: " + money;
    }
    
    public void TakeDamage(int damage)
    {
        Debug.Log("🎯 TAKE DAMAGE CALLED: -" + damage + " HP → " + (playerHP - damage));
        playerHP -= damage;
        UpdateUI();
       
        if (playerHP <= 0)
        {
            Debug.Log("💀 GAME OVER!");
            Time.timeScale = 0f;
        }
    }
	
	public void AddMoney(int amount)
	{
    money += amount;
    UpdateUI();
	}
}