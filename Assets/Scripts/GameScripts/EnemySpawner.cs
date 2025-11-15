using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; } // ← NOVÉ: Singleton

    [Header("Spawn")]
    public GameObject enemyPrefab;

    private Coroutine currentSpawnRoutine = null;
    private int zombiesToSpawn = 0;
    private int zombiesAlive = 0; // ← NOVÉ: Počítadlo živých

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SpawnWave(int enemyCount, float spawnInterval)
    {
        if (currentSpawnRoutine != null)
            StopCoroutine(currentSpawnRoutine);

        zombiesToSpawn = enemyCount;
        zombiesAlive = enemyCount; // ← Nastav počet živých

        currentSpawnRoutine = StartCoroutine(SpawnWaveCoroutine(enemyCount, spawnInterval));
    }

    private IEnumerator SpawnWaveCoroutine(int count, float interval)
    {
        for (int i = 0; i < count; i++)
        {
            if (enemyPrefab != null)
            {
                GameObject zombie = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
                // NOVÉ: Pripoj EnemyHealth listener
                EnemyHealth health = zombie.GetComponent<EnemyHealth>();
                if (health != null)
                    health.onDeath += OnZombieDied; // ← Registruj smrť
            }
            yield return new WaitForSeconds(interval);
        }
    }

    // NOVÉ: Volané z EnemyHealth
    private void OnZombieDied()
    {
        zombiesAlive--;
        if (zombiesAlive <= 0 && WaveManager.Instance != null)
        {
            WaveManager.Instance.WaveEnded(); // ← LEN keď zomrie posledný!
        }
    }
}