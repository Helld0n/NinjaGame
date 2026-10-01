using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreenManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject deathScreen;      // Панель экрана смерти

    [Header("Сцены")]
    public string townSceneName = "MainTown";   // Имя начальной локации

    void Start()
    {
        if (deathScreen != null) deathScreen.SetActive(false);
    }

    // Показать экран смерти
    public void ShowDeathScreen()
    {
        if (deathScreen != null) deathScreen.SetActive(true);

        // Останавливаем время (опционально)
        // Time.timeScale = 0f;
    }

    // Перезапустить текущий уровень
    public void RestartLevel()
    {
        // Time.timeScale = 1f;   // Если останавливал время — вернуть
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Вернуться в начальную локацию
    public void GoToTown()
    {
        // Time.timeScale = 1f;
        SceneManager.LoadScene(townSceneName);
    }
}