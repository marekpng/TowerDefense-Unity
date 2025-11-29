using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    [Header("Spawn")]
    public GameObject enemyPrefab;

    private Coroutine currentSpawnRoutine = null;
    private int zombiesAlive = 0; // Počet živých zombie v aktuálnej vlne

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // ← SPRÁVNE: Destroy(gameObject)
            return;
        }
    }

    public void SpawnWave(int enemyCount, float spawnInterval)
    {
        if (currentSpawnRoutine != null)
            StopCoroutine(currentSpawnRoutine);

        zombiesAlive = enemyCount;

        currentSpawnRoutine = StartCoroutine(SpawnWaveCoroutine(enemyCount, spawnInterval));
    }

    private IEnumerator SpawnWaveCoroutine(int count, float interval)
    {
        for (int i = 0; i < count; i++)
        {
            if (enemyPrefab == null) yield break;

            GameObject zombie = Instantiate(enemyPrefab, transform.position, Quaternion.identity);

            EnemyHealth health = zombie.GetComponent<EnemyHealth>();
            if (health != null)
            {
                health.onDeath += OnZombieDied; // Registrácia smrti
            }

            yield return new WaitForSeconds(interval);
        }
    }

    // Volané z EnemyHealth pri smrti (aj v base!)
    private void OnZombieDied()
    {
        zombiesAlive--;

        if (zombiesAlive <= 0)
        {
            WaveManager.Instance?.WaveEnded(); // Spustí ďalšiu vlnu!
        }
    }
}