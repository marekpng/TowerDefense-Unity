using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [Header("UI References")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false); // Hide at start
    }

    public void ShowGameOver()
    {
        if (gameOverPanel == null) return;

        gameOverPanel.SetActive(true);
        if (gameOverText != null)
            gameOverText.text = "GAME OVER";

        // Stop time and gameplay
        Time.timeScale = 0f;
    }

    // Called by Retry button
    public void RetryLevel()
    {
        Time.timeScale = 1f;
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }

    // Called by Main Menu button
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // Make sure this matches your scene name
    }
}
