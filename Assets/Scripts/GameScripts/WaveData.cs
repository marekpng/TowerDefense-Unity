using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "TowerDefense/WaveData")]
public class WaveData : ScriptableObject
{
    [Header("Wave Config")]
    public int enemyCount = 50;
    [Tooltip("Zombies per second")]
    public float spawnRate = 1f; // 1 zombie/sec
    [Tooltip("Pauza pred touto wave (s)")]
    public float waveDelay = 10f;
}