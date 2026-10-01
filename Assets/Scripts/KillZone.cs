using UnityEngine;

public class KillZone : MonoBehaviour
{
    [Header("Урон при падении")]
    public int damage = 9999;   // Мгновенная смерть

    void OnTriggerEnter2D(Collider2D other)
    {
        // Реагируем только на игрока
        if (!other.CompareTag("Player")) return;

        HealthSystem playerHealth = other.GetComponent<HealthSystem>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }
}