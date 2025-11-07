using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Buttons")]
    public Button startButton;
    public Button optionsButton;
    public Button quitButton;

    void Start()
    {
        // Auto-wire buttons if not assigned in Inspector
        if (startButton == null) startButton = transform.Find("StartButton")?.GetComponent<Button>();
        if (optionsButton == null) optionsButton = transform.Find("OptionsButton")?.GetComponent<Button>();
        if (quitButton == null) quitButton = transform.Find("QuitButton")?.GetComponent<Button>();

        // Wire up clicks programmatically (backup if not done in Inspector)
        if (startButton != null) startButton.onClick.AddListener(StartGame);
        if (optionsButton != null) optionsButton.onClick.AddListener(OpenOptions);
        if (quitButton != null) quitButton.onClick.AddListener(QuitGame);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("LevelSelection");
    }

    public void OpenOptions()
    {
        SceneManager.LoadScene("Options");
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}