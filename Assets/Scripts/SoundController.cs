using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController Instance;

    public AudioSource musicSource;
    public AudioClip[] musicTracks;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource.loop = true;

        float volume = PlayerPrefs.GetFloat("AudioVolume", 0.5f);
        musicSource.volume = volume;

        int trackIndex = PlayerPrefs.GetInt("MusicTrack", 0);
        ChangeMusicTrack(trackIndex);
    }

    public void ChangeMusicTrack(int index)
    {
        if (index < 0 || index >= musicTracks.Length)
            return;

        if (musicSource.clip == musicTracks[index])
            return;

        musicSource.clip = musicTracks[index];
        musicSource.Play();
    }

    public void SetVolume(float volume)
    {
        musicSource.volume = volume;
    }
}
