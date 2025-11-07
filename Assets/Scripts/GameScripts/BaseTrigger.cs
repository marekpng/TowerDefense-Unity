using UnityEngine;

public class BaseTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Base Trigger Hit by: " + other.name + " (Tag: " + other.tag + ")"); // Logs EVERY hit

        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Enemy confirmed! Calling TakeDamage...");
            
            if (GameManager.Instance == null)
            {
                Debug.LogError("🚨 GAME MANAGER INSTANCE IS NULL! Fix singleton.");
            }
            else
            {
                Debug.Log("GameManager found - Dealing 10 damage");
                GameManager.Instance.TakeDamage(10);
            }
            
            Destroy(other.gameObject);
        }
    }
}