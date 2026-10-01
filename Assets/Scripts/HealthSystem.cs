using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [Header("Настройки HP")]
    public int maxHealth = 12;
    public int currentHealth;

    [Header("UI")]
    public Image hpBarImage;
    public Sprite[] hpBarSprites;

    [Header("Ссылки")]
    public Animator animator;
    public PlayerController playerController;
    public DeathScreenManager deathScreenManager;   // ← новое поле

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (playerController == null) playerController = GetComponent<PlayerController>();
        if (deathScreenManager == null) deathScreenManager = FindObjectOfType<DeathScreenManager>();

        currentHealth = maxHealth;
        UpdateHPBar();
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        if (playerController != null && playerController.IsSliding()) return;

        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHPBar();

        if (currentHealth <= 0) Die();
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

        int spriteIndex = Mathf.Clamp(currentHealth - 1, 0, hpBarSprites.Length - 1);
        if (currentHealth <= 0) spriteIndex = 0;

        hpBarImage.sprite = hpBarSprites[spriteIndex];
    }

    void Die()
    {
        Debug.Log("Игрок умер!");

        if (playerController != null) playerController.enabled = false;

        if (animator != null)
        {
            animator.SetTrigger("IsDead");
        }

        if (deathScreenManager != null)
        {
            Invoke("ShowDeathScreen", 2f);
        }
    }

    void ShowDeathScreen()
    {
        if (deathScreenManager != null)
        {
            deathScreenManager.ShowDeathScreen();
        }
    }
}