using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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
    public GameObject gameOverPanel;
    public GameObject victoryPanel;

    private bool isGameOver = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        Time.timeScale = 1f;
        UpdateUI();
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
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
        if (hpText != null) hpText.text = "HP: " + playerHP;
        if (moneyText != null) moneyText.text = "$: " + money;
    }

    public void TakeDamage(int damage)
    {
        if (isGameOver) return;
        playerHP -= damage;
        UpdateUI();
        if (playerHP <= 0)
            GameOver();
    }

    private void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    public void Victory()
    {
        Time.timeScale = 0f;
        if (victoryPanel != null) victoryPanel.SetActive(true);
    }

    // Buttony
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        if (WaveManager.Instance != null)
            WaveManager.Instance.ResetWaveManager();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // Tvoja hlavná menu scéna
    }

    // NOVÉ: Automatický Next Level podľa aktuálnej scény
    public void NextLevel()
    {
        Time.timeScale = 1f;

        string currentScene = SceneManager.GetActiveScene().name;

        string nextScene = currentScene switch
        {
            "Level1" => "Level2",
            "Level2" => "Level3",
            "Level3" => "Level4",
            "Level4" => "MainMenu", // Po Level 4 → späť do menu
            _ => "MainMenu" // Bezpečnostný fallback
        };

        SceneManager.LoadScene(nextScene);
    }
}