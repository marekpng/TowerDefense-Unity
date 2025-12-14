using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    // --- NOVÉ: Pause a Settings ---
    [Header("Pause & Settings")]
    public GameObject pausePanel;
    public GameObject settingsPanel;

    public Button resumeButton;
    public Button settingsButton;
    public Button mainMenuButton;

    public Slider volumeSlider;
    public TMP_Dropdown trackDropdown;
    public Button settingsBackButton;

    private bool isGameOver = false;
    private bool isPaused = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // pôvodný kód
        if (LogManager.Instance != null)
        {
            LogManager.Instance.StartNewSession("player-default");
        }
        Time.timeScale = 1f;
        UpdateUI();
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);

        // --- NOVÉ: Inicializácia Pause menu ---
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        if (resumeButton != null) resumeButton.onClick.AddListener(ResumeGame);
        if (settingsButton != null) settingsButton.onClick.AddListener(OpenSettings);
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(MainMenu);

        if (settingsBackButton != null) settingsBackButton.onClick.AddListener(CloseSettings);

        if (volumeSlider != null)
        {
            volumeSlider.value = PlayerPrefs.GetFloat("AudioVolume", 0.5f);
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        if (trackDropdown != null && SoundController.Instance != null)
        {
            trackDropdown.ClearOptions();
            var options = new System.Collections.Generic.List<string>();
            for (int i = 0; i < SoundController.Instance.musicTracks.Length; i++)
                options.Add(SoundController.Instance.musicTracks[i].name);
            trackDropdown.AddOptions(options);

            int savedTrack = PlayerPrefs.GetInt("MusicTrack", 0);
            trackDropdown.value = savedTrack;
            trackDropdown.onValueChanged.AddListener(SetTrack);

            SoundController.Instance.ChangeMusicTrack(savedTrack); // spusti uložený track
        }
    }

    void Update()
    {
        // --- NOVÉ: ESC na pauzu ---
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    // --- PÔVODNÉ FUNKCIE ---
    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            int oldMoney = money;
            money -= amount;
            UpdateUI();

            if (LogManager.Instance != null)
            {
                LogManager.Instance.LogGenericEvent(
                    playerId: "player-default",
                    eventName: $"moneySpent_old_{oldMoney}_new_{money}_amount_{amount}",
                    towerId: null,
                    zombieId: null,
                    position: Vector3.zero
                );
            }

            return true;
        }
        return false;
    }

    public void AddMoney(int amount)
    {
        int oldMoney = money;
        money += amount;
        UpdateUI();

        if (LogManager.Instance != null)
        {
            LogManager.Instance.LogGenericEvent(
                playerId: "player-default",
                eventName: $"moneyGained_old_{oldMoney}_new_{money}_amount_{amount}",
                towerId: null,
                zombieId: null,
                position: Vector3.zero
            );
        }
    }

    void UpdateUI()
    {
        if (hpText != null) hpText.text = "HP: " + playerHP;
        if (moneyText != null) moneyText.text = "$: " + money;
    }

    public void TakeDamage(int damage)
    {
        if (isGameOver) return;
        SoundController.Instance.PlayBaseHit();


        int hpBefore = playerHP;
        playerHP -= damage;
        UpdateUI();

        if (LogManager.Instance != null)
        {
            LogManager.Instance.LogGenericEvent(
                playerId: "player-default",
                eventName: $"playerHit_hpBefore_{hpBefore}_hpAfter_{playerHP}_damage_{damage}",
                towerId: null,
                zombieId: null,
                position: Vector3.zero
            );
        }

        if (playerHP <= 0)
            GameOver();
    }

    private void GameOver()
    {
        isGameOver = true;
        SoundController.Instance.PlayGameOver();

        Time.timeScale = 0f;
        if (LogManager.Instance != null)
        {
            LogManager.Instance.LogGameOver("player-default", false);
        }
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    public void Victory()
    {
        Time.timeScale = 0f;
        SoundController.Instance.PlayLevelComplete();

        if (LogManager.Instance != null)
        {
            LogManager.Instance.LogGameOver("player-default", true);
        }
        if (victoryPanel != null) victoryPanel.SetActive(true);
    }

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
        SceneManager.LoadScene("MainMenu");
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;

        string currentScene = SceneManager.GetActiveScene().name;

        string nextScene = currentScene switch
        {
            "Level1" => "Level2",
            "Level2" => "Level3",
            "Level3" => "Level4",
            "Level4" => "MainMenu",
            _ => "MainMenu"
        };

        SceneManager.LoadScene(nextScene);
    }

    // --- NOVÉ FUNKCIE PRE PAUSE MENU ---
    void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    void OpenSettings()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    void SetVolume(float value)
    {
        PlayerPrefs.SetFloat("AudioVolume", value);
        PlayerPrefs.Save();

        if (SoundController.Instance != null)
            SoundController.Instance.SetVolume(value);
    }

    void SetTrack(int index)
    {
        PlayerPrefs.SetInt("MusicTrack", index);
        PlayerPrefs.Save();

        if (SoundController.Instance != null)
            SoundController.Instance.ChangeMusicTrack(index);
    }
}
