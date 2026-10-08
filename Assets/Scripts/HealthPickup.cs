using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Настройки лечения")]
    public int healAmount = 6;         // Сколько HP восстанавливает (половина от 12)
    public float lifetime = 10f;        // Через сколько секунд исчезнет (0 = не исчезает)

    void Start()
    {
        if (lifetime > 0)
        {
            Destroy(gameObject, lifetime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        HealthSystem playerHealth = other.GetComponent<HealthSystem>();
        if (playerHealth != null)
        {
            // Лечим игрока
            playerHealth.Heal(healAmount);

            // Уничтожаем хилку
            Destroy(gameObject);
        }
    }
}