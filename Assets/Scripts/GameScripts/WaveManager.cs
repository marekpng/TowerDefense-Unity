using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("UI")]
    public TMP_Text waveText;
    public TMP_Text nextWaveText;

    [Header("Spawner")]
    public EnemySpawner spawner;

    [Header("Level Waves")]
    public WaveData[] level1Waves = new WaveData[6];
    public WaveData[] level2Waves = new WaveData[6];
    public WaveData[] level3Waves = new WaveData[6];
    public WaveData[] level4Waves = new WaveData[6];

    private List<WaveData> currentWaves;
    private int currentWave = 0;
    private int totalWaves = 0;
    private bool waveActive = false;
    private float waveTimer = 0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        ResetWaveManager();
    }

    void Start()
    {
        StartLevelFromScene();
        UpdateUI();
    }

    void Update()
    {
        if (waveActive || totalWaves == 0) return;

        waveTimer -= Time.deltaTime;
        if (waveTimer <= 0 && currentWave < totalWaves)
        {
            StartNextWave();
        }

        if (nextWaveText != null)
            nextWaveText.text = $"Next Wave in {Mathf.CeilToInt(waveTimer)}s";
    }

    // NOVÉ: Automaticky zisti level podľa scény
    void StartLevelFromScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName.Contains("Level1") || sceneName == "Level1")
            StartLevel(1);
        else if (sceneName.Contains("Level2") || sceneName == "Level2")
            StartLevel(2);
        else if (sceneName.Contains("Level3") || sceneName == "Level3")
            StartLevel(3);
        else if (sceneName.Contains("Level4") || sceneName == "Level4")
            StartLevel(4);
        else
            StartLevel(1); // fallback
    }

    public void StartLevel(int level)
    {
        currentWaves = level switch
        {
            1 => new List<WaveData>(level1Waves.Where(w => w != null)),
            2 => new List<WaveData>(level2Waves.Where(w => w != null)),
            3 => new List<WaveData>(level3Waves.Where(w => w != null)),
            4 => new List<WaveData>(level4Waves.Where(w => w != null)),
            _ => new List<WaveData>()
        };

        totalWaves = currentWaves.Count;
        currentWave = 0;
        waveActive = false;
        waveTimer = totalWaves > 0 ? currentWaves[0].waveDelay : 0f;
        UpdateUI();
    }

    void StartNextWave()
    {
        if (currentWave >= totalWaves) return;

        WaveData wave = currentWaves[currentWave];
        if (spawner != null)
            spawner.SpawnWave(wave.enemyCount, 1f / wave.spawnRate);

        waveActive = true;
        currentWave++;
        UpdateUI();
    }

    public void WaveEnded()
    {
        waveActive = false;
        if (currentWave < totalWaves)
        {
            waveTimer = currentWaves[currentWave].waveDelay;
        }
        else
        {
            Debug.Log($"Level {GetCurrentLevel()} Complete!");
            if (GameManager.Instance != null)
                GameManager.Instance.Victory();
        }
        UpdateUI();
    }

    // Pomocná metóda
    int GetCurrentLevel()
    {
        string scene = SceneManager.GetActiveScene().name;
        if (scene.Contains("1")) return 1;
        if (scene.Contains("2")) return 2;
        if (scene.Contains("3")) return 3;
        if (scene.Contains("4")) return 4;
        return 1;
    }

    void UpdateUI()
    {
        if (waveText != null)
            waveText.text = $"Wave {currentWave}/{totalWaves}";
    }

    public void ResetWaveManager()
    {
        currentWave = 0;
        waveActive = false;
        waveTimer = 0f;
        UpdateUI();
    }
}