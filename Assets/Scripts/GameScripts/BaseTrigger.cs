using UnityEngine;

public class BaseTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth health = other.GetComponent<EnemyHealth>();
            if (health != null)
            {
                string enemyType = "Enemy";
                var typeField = typeof(EnemyHealth).GetField("enemyType");
                if (typeField != null)
                {
                    enemyType = (string)typeField.GetValue(health);
                }

                GameLogger.Instance.LogEvent(
                    "baseHit",
                    enemyType,
                    10
                );

                GameManager.Instance.TakeDamage(10);
                health.Die(silentKill: true); // ← OKAMŽITÁ SMRŤ BEZ ANIMÁCIE!
            }
        }
    }
}