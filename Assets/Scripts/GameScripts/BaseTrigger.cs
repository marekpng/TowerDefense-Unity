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
                GameManager.Instance.TakeDamage(10);
                health.Die(silentKill: true); // ← OKAMŽITÁ SMRŤ BEZ ANIMÁCIE!
            }
        }
    }
}