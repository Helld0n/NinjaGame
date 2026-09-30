using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [Header("Настройки HP")]
    public int maxHealth = 12;
    public int currentHealth;

    [Header("UI")]
    public Image hpBarImage;              // Ссылка на Image в Canvas
    public Sprite[] hpBarSprites;         // 12 спрайтов бара (от 0 до 11)

    [Header("Ссылки")]
    public Animator animator;             // Animator игрока (для анимации смерти)

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHPBar();
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;

        if (currentHealth < 0) currentHealth = 0;

        UpdateHPBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        UpdateHPBar();
    }

    void UpdateHPBar()
    {
        if (hpBarImage == null || hpBarSprites == null || hpBarSprites.Length == 0) return;

        // Индекс спрайта: 0 = пусто, 11 = полный
        int spriteIndex = Mathf.Clamp(currentHealth - 1, 0, hpBarSprites.Length - 1);

        // Если HP = 0 — показываем пустой бар (индекс 0)
        if (currentHealth <= 0) spriteIndex = 0;

        hpBarImage.sprite = hpBarSprites[spriteIndex];
    }

    void Die()
    {
        Debug.Log("Игрок умер!");

        // Проигрываем анимацию смерти
        if (animator != null)
        {
            animator.SetTrigger("Dead");
        }

        // Здесь можно добавить: перезапуск уровня, экран Game Over и т.д.
    }
}