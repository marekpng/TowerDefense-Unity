using UnityEngine;
using UnityEngine.UI;
using TMPro; // 🔴 DÔLEŽITÉ
using UnityEngine.SceneManagement;

public class OptionsController : MonoBehaviour
{
    public Slider audioSlider;
    public TMP_Dropdown musicDropdown; // 🔴 ZMENA
    public Button backButton;

    private SoundController soundController;

    void Start()
    {
        soundController = FindObjectOfType<SoundController>();

        if (audioSlider != null)
        {
            audioSlider.value = PlayerPrefs.GetFloat("AudioVolume", 0.5f);
            audioSlider.onValueChanged.AddListener(SetAudioVolume);
        }

        if (musicDropdown != null && soundController != null)
        {
            musicDropdown.ClearOptions();

            foreach (AudioClip clip in soundController.musicTracks)
            {
                musicDropdown.options.Add(new TMP_Dropdown.OptionData(clip.name));
            }

            musicDropdown.onValueChanged.AddListener(ChangeMusicTrack);

            int savedTrack = PlayerPrefs.GetInt("MusicTrack", 0);
            musicDropdown.value = savedTrack;
            soundController.ChangeMusicTrack(savedTrack);
        }

        if (backButton != null)
            backButton.onClick.AddListener(BackToMenu);
    }

    public void SetAudioVolume(float volume)
    {
        AudioListener.volume = volume;
        soundController.SetVolume(volume);
        PlayerPrefs.SetFloat("AudioVolume", volume);
        
    }

    public void ChangeMusicTrack(int index)
    {
        soundController.ChangeMusicTrack(index);
        PlayerPrefs.SetInt("MusicTrack", index);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
