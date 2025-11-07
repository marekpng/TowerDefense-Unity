using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // For restarting

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player")]
    public int playerHP = 100;
    public int money = 200;

    [Header("UI")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI moneyText;

    [Header("Panels")]
    public GameObject gameOverPanel; // assign this in Inspector (GameOverCanvas or child panel)

    private bool isGameOver = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        Time.timeScale = 1f; // ensure normal time on start
        UpdateUI();
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
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

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (hpText != null)
            hpText.text = "HP: " + playerHP;
        if (moneyText != null)
            moneyText.text = "$: " + money;
    }

    public void TakeDamage(int damage)
    {
        if (isGameOver) return;

        Debug.Log("🎯 TAKE DAMAGE CALLED: -" + damage + " HP → " + (playerHP - damage));
        playerHP -= damage;
        UpdateUI();

        if (playerHP <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        isGameOver = true;
        Debug.Log("💀 GAME OVER!");
        Time.timeScale = 0f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    // Optional: hook these to buttons
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
