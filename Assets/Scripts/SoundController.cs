using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController Instance;

    [Header("Music")]
    public AudioSource musicSource;
    public AudioClip[] musicTracks;

    [Header("SFX")]
    public AudioClip shootSFX;        // strela veže
    public AudioClip enemyDieSFX;     // zomrie nepriateľ
    public AudioClip baseHitSFX;      // poškodenie základne
    public AudioClip levelCompleteSFX;// level complete
    public AudioClip gameOverSFX;     // game over

    [Header("SFX Volume")]
    [Range(0f, 1f)]
    public float sfxVolume = 1f;      // globálna hlasitosť efektov

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

    // Prehranie hudby
    public void ChangeMusicTrack(int index)
    {
        if (musicTracks == null || musicTracks.Length == 0) return;
        if (index < 0 || index >= musicTracks.Length) return;

        if (musicSource.clip == musicTracks[index]) return;

        musicSource.clip = musicTracks[index];
        musicSource.Play();
    }

    public void SetVolume(float volume)
    {
        musicSource.volume = volume;
    }

    // Prehratie zvukového efektu s relatívnou hlasitosťou
    public void PlaySFX(AudioClip clip, float relativeVolume = 1f)
    {
        if (clip == null) return;

        // vypočíta hlasitosť: globálna sfxVolume * relatívna hlasitosť, min 0.1
        float vol = Mathf.Max(sfxVolume * relativeVolume, 0.1f);

        musicSource.PlayOneShot(clip, vol);
    }

    // Pomocné metódy pre konkrétne zvuky
    public void PlayShoot() => PlaySFX(shootSFX, 0.1f);      // tichšie o 90%
    public void PlayEnemyDie() => PlaySFX(enemyDieSFX, 1.2f); // hlasnejšie o 20%
    public void PlayBaseHit() => PlaySFX(baseHitSFX, 1f);
    public void PlayLevelComplete() => PlaySFX(levelCompleteSFX, 1f);
    public void PlayGameOver() => PlaySFX(gameOverSFX, 1f);
}
