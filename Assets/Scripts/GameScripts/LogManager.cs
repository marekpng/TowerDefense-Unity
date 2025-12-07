using System;
using System.IO;
using UnityEngine;

public class LogManager : MonoBehaviour
{
    public static LogManager Instance;

    [Header("Config")]
    [Tooltip("Ak chceš fixného playera, môžeš ho tu nastaviť, inak pošli z GameManagera.")]
    public string defaultPlayerId = "player-1";

    private string logDirectory;
    private string currentLogFilePath;
    private string currentSessionId;
    private float sessionStartTime;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Logs budú v: Application.persistentDataPath/Logs
        logDirectory = Path.Combine(Application.persistentDataPath, "Logs");

        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }
    }

    /// <summary>
    /// Zavolaj pri každom novom spustení hry / novej hre (napr. GameManager.StartGame).
    /// Vytvorí nový JSONL súbor.
    /// </summary>
    public void StartNewSession(string playerId = null, int level = 0)
    {
        if (string.IsNullOrEmpty(playerId))
        {
            playerId = defaultPlayerId;
        }

        currentSessionId = Guid.NewGuid().ToString();
        sessionStartTime = Time.time;

        string fileName = $"log_level{level}_{DateTime.UtcNow:yyyyMMdd_HHmmss}_{currentSessionId}.jsonl";
        currentLogFilePath = Path.Combine(logDirectory, fileName);

        // Uložíme cestu, aby si ju vedel spätne nájsť
        PlayerPrefs.SetString("LastLogPath", currentLogFilePath);
        PlayerPrefs.Save();

        Debug.Log($"[LOG] New session started. SessionId={currentSessionId}");
        Debug.Log($"[LOG] Writing logs to: {currentLogFilePath}");

        // Zapíšeme prvý event typu "sessionStart"
        LogGenericEvent(playerId, $"sessionStart_level_{level}", null, null, Vector3.zero);
    }

    /// <summary>
    /// Interná metóda – zapisuje event do JSONL (1 riadok = 1 JSON).
    /// </summary>
    private void WriteEvent(GameEvent gameEvent)
    {
        if (string.IsNullOrEmpty(currentLogFilePath))
        {
            Debug.LogWarning("[LOG] currentLogFilePath is null or empty. Call StartNewSession() first.");
            return;
        }

        string json = JsonUtility.ToJson(gameEvent);
        try
        {
            File.AppendAllText(currentLogFilePath, json + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[LOG] Failed to write log: {ex.Message}");
        }
    }

    /// <summary>
    /// Generic logger – ak potrebuješ len eventName + optional tower/zombie/position.
    /// </summary>
    public void LogGenericEvent(
        string playerId,
        string eventName,
        string towerId,
        string zombieId,
        Vector3 position)
    {
        if (string.IsNullOrEmpty(playerId))
            playerId = defaultPlayerId;

        GameEvent e = new GameEvent
        {
            playerId = playerId,
            sessionId = currentSessionId,
            eventName = eventName,
            timestamp = DateTime.UtcNow.ToString("o"),
            sessionTime = Time.time - sessionStartTime,
            towerId = towerId ?? "",
            zombieId = zombieId ?? "",
            posX = position.x,
            posY = position.y,
            posZ = position.z,
        };

        WriteEvent(e);
    }

    // --------- Špecifické helper metódy ---------

    /// <summary>
    /// Log tower shot – tu riešiš towerId + zombieId.
    /// </summary>
    public void LogTowerShot(
        string playerId,
        string towerId,
        string zombieId,
        Vector3 position)
    {
        LogGenericEvent(playerId, "towerShot", towerId, zombieId, position);
    }

    /// <summary>
    /// Log spawn zombíka.
    /// </summary>
    public void LogZombieSpawn(
        string playerId,
        string zombieId,
        Vector3 position)
    {
        LogGenericEvent(playerId, "zombieSpawn", null, zombieId, position);
    }

    /// <summary>
    /// Log zabitie zombíka.
    /// </summary>
    public void LogZombieKilled(
        string playerId,
        string zombieId,
        Vector3 position)
    {
        LogGenericEvent(playerId, "zombieKilled", null, zombieId, position);
    }

    /// <summary>
    /// Log koniec hry.
    /// </summary>
    public void LogGameOver(string playerId, bool win)
    {
        string result = win ? "win" : "lose";
        LogGenericEvent(playerId, $"gameOver_{result}", null, null, Vector3.zero);
    }

    /// <summary>
    /// Pre debug – vráti cestu k aktuálnemu logu.
    /// </summary>
    public string GetCurrentLogPath()
    {
        return currentLogFilePath;
    }

    /// <summary>
    /// Pre debug – vráti poslednú cestu z PlayerPrefs.
    /// </summary>
    public static string GetLastLogPath()
    {
        return PlayerPrefs.GetString("LastLogPath", "NO_LOG_FOUND");
    }
}