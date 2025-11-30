using System.Collections;
using System.Text;
using UnityEngine;
using System.IO;

public class GameLogger : MonoBehaviour
{
    public static GameLogger Instance;

    [SerializeField] private bool loggingEnabled = true;

    private string playerId;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        playerId = System.Guid.NewGuid().ToString();
        Debug.Log("Log file path: " + Application.persistentDataPath + "/logs.json");
        LogEvent("sessionStart");
    }

    void OnApplicationQuit()
    {
        LogEvent("sessionEnd");
    }

    // ────────────────────────────────────────
    // PUBLIC API — voláš z celej hry
    // ────────────────────────────────────────

    public void LogEvent(string eventName, params object[] data)
    {
        if (!loggingEnabled) return;

        FullLogEntry entry = new FullLogEntry();
        entry.playerId = playerId;
        entry.eventName = eventName;
        entry.timestamp = System.DateTime.UtcNow.ToString("o");
        entry.sessionTime = Time.timeSinceLevelLoad;
        entry.parameters = new System.Collections.Generic.List<ParamEntry>();

        void Add(string k, object v)
        {
            entry.parameters.Add(new ParamEntry
            {
                key = k,
                value = v != null ? v.ToString() : "null"
            });
        }

        switch (eventName)
        {
            case "enemyHit":
                Add("enemyType", data.Length > 0 ? data[0] : null);
                Add("maxHP", data.Length > 1 ? data[1] : null);
                Add("oldHP", data.Length > 2 ? data[2] : null);
                Add("damage", data.Length > 3 ? data[3] : null);
                Add("newHP", data.Length > 4 ? data[4] : null);
                break;

            case "enemyKilled":
                Add("enemyType", data.Length > 0 ? data[0] : null);
                Add("turretId", data.Length > 1 ? data[1] : null);
                Add("reward", data.Length > 2 ? data[2] : null);
                break;

            case "towerShot":
                Add("x", data.Length > 0 ? data[0] : null);
                Add("y", data.Length > 1 ? data[1] : null);
                Add("z", data.Length > 2 ? data[2] : null);
                Add("damage", data.Length > 3 ? data[3] : null);
                break;

            case "turretPlaced":
                Add("type", data.Length > 0 ? data[0] : null);
                Add("x", data.Length > 1 ? data[1] : null);
                Add("y", data.Length > 2 ? data[2] : null);
                Add("z", data.Length > 3 ? data[3] : null);
                Add("cost", data.Length > 4 ? data[4] : null);
                break;

            case "moneyChange":
                Add("oldAmount", data.Length > 0 ? data[0] : null);
                Add("newAmount", data.Length > 1 ? data[1] : null);
                Add("reason", data.Length > 2 ? data[2] : null);
                break;

            case "baseDamage":
                Add("damage", data.Length > 0 ? data[0] : null);
                Add("currentHp", data.Length > 1 ? data[1] : null);
                break;

            case "towerDamage":
                Add("towerId", data.Length > 0 ? data[0] : null);
                Add("damage", data.Length > 1 ? data[1] : null);
                Add("currentHp", data.Length > 2 ? data[2] : null);
                break;

            case "waveStart":
                Add("waveNumber", data.Length > 0 ? data[0] : null);
                Add("enemyCount", data.Length > 1 ? data[1] : null);
                break;

            case "waveEnd":
                Add("waveNumber", data.Length > 0 ? data[0] : null);
                Add("timeSurvived", data.Length > 1 ? data[1] : null);
                break;

            default:
                for (int i = 0; i < data.Length; i++)
                    Add("param" + (i + 1), data[i]);
                break;
        }

        string json = JsonUtility.ToJson(entry);
        AppendJson(json);
    }

    private void AppendJson(string jsonLine)
    {
        string path = Application.persistentDataPath + "/logs.json";
        File.AppendAllText(path, jsonLine + "\n");
    }

    public void LogTowerDamage(int towerId, int damageAmount, int currentHp)
    {
        LogEvent(
            "towerDamage",
            towerId,
            damageAmount,
            currentHp
        );
    }

    // --- NEW EVENTS FOR TOWER DEFENSE -------------------------------------

    // When player places a turret
    public void LogTurretPlaced(string turretType, float x, float y, float z, int cost)
    {
        LogEvent(
            "turretPlaced",
            turretType,
            x, y, z,
            cost
        );
    }

    // When player upgrades a turret
    public void LogTurretUpgrade(int turretId, int oldLevel, int newLevel, int cost)
    {
        LogEvent(
            "turretUpgrade",
            turretId,
            oldLevel,
            newLevel,
            cost
        );
    }

    // When player sells a turret
    public void LogTurretSold(int turretId, string turretType, int refund)
    {
        LogEvent(
            "turretSold",
            turretId,
            turretType,
            refund
        );
    }

    // When any enemy is killed
    public void LogEnemyKilled(string enemyType, int turretId, int reward)
    {
        LogEvent(
            "enemyKilled",
            enemyType,
            turretId,
            reward
        );
    }

    // Wave start
    public void LogWaveStart(int waveNumber, int enemyCount)
    {
        LogEvent(
            "waveStart",
            waveNumber,
            enemyCount
        );
    }

    // Wave end
    public void LogWaveEnd(int waveNumber, float timeSurvived)
    {
        LogEvent(
            "waveEnd",
            waveNumber,
            timeSurvived
        );
    }

    // Resource changes (money gained/lost)
    public void LogMoneyChange(int oldAmount, int newAmount, string reason)
    {
        LogEvent(
            "moneyChange",
            oldAmount,
            newAmount,
            reason
        );
    }

    // Player base damage (HP loss)
    public void LogBaseDamage(int damage, int currentHp)
    {
        LogEvent(
            "baseDamage",
            damage,
            currentHp
        );
    }

    private void SendNow(string payload) {}

    [System.Serializable]
    public class ParamEntry
    {
        public string key;
        public string value;
    }

    [System.Serializable]
    public class FullLogEntry
    {
        public string playerId;
        public string eventName;
        public string timestamp;
        public float sessionTime;
        public System.Collections.Generic.List<ParamEntry> parameters;
    }
}