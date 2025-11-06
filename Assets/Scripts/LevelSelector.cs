using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    [Header("Buttons")]
    public Button level1Button;
    public Button level2Button;
    public Button level3Button;
    public Button backButton;

    void Start()
    {
        // Auto-wire buttons if not assigned
        if (level1Button == null) level1Button = transform.Find("Level1Button")?.GetComponent<Button>();
        if (level2Button == null) level2Button = transform.Find("Level2Button")?.GetComponent<Button>();
        if (level3Button == null) level3Button = transform.Find("Level3Button")?.GetComponent<Button>();
        if (backButton == null) backButton = transform.Find("BackButton")?.GetComponent<Button>();

        // Wire clicks
        if (level1Button != null) level1Button.onClick.AddListener(() => LoadLevel(1));
        if (level2Button != null) level2Button.onClick.AddListener(() => LoadLevel(2));
        if (level3Button != null) level3Button.onClick.AddListener(() => LoadLevel(3));
        if (backButton != null) backButton.onClick.AddListener(Back);
    }

    public void LoadLevel(int level)
    {
        string sceneName = $"Level{level}";
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning($"Scene {sceneName} not found! Create a placeholder.");
        }
    }

    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }
}