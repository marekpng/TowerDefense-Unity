using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionsController : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider audioSlider;
    public Button backButton;

    void Start()
    {
        // Auto-nájdi ak nie priradené
        if (audioSlider == null) audioSlider = transform.Find("AudioSlider")?.GetComponent<Slider>();
        if (backButton == null) backButton = transform.Find("BackButton")?.GetComponent<Button>();

        // Načítaj uložené volume (default 0.5)
        audioSlider.value = PlayerPrefs.GetFloat("AudioVolume", 0.5f);
        audioSlider.onValueChanged.AddListener(SetAudioVolume);

        // Back button
        backButton.onClick.AddListener(BackToMenu);
    }

    public void SetAudioVolume(float volume)
    {
        AudioListener.volume = volume; // Nastav globálne volume (0-1)
        PlayerPrefs.SetFloat("AudioVolume", volume); // Ulož pre budúce spustenia
        PlayerPrefs.Save();
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}