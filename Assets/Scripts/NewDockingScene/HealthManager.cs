using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    [Header("生命值设置")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("UI引用")]
    public Image healthBar;
    public Sprite[] healthSprites; // 0:满, 1:中, 2:低, 3:空
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    public Button restartButton; // 重新开始按钮


    [Header("玩家引用")]
    public GameObject playerShip;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // 绑定重新开始按钮事件
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
    }

    public void LoseLife()
    {
        currentHealth--;
        currentHealth = Mathf.Max(0, currentHealth);
        UpdateHealthUI();

        SoundManager.instance.Play("Exploded");

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthBar != null && healthSprites != null && healthSprites.Length > 0)
        {
            int spriteIndex = Mathf.Min(currentHealth, healthSprites.Length - 1);
            healthBar.sprite = healthSprites[spriteIndex];
        }
    }

    private void GameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (gameOverText != null)
            gameOverText.text = "游戏结束";

        if (playerShip != null)
        {
            MonoBehaviour[] controllers = playerShip.GetComponents<MonoBehaviour>();
            foreach (var controller in controllers)
                controller.enabled = false;

            Rigidbody rb = playerShip.GetComponent<Rigidbody>();
            if (rb != null)
                rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        MeteorSpawner spawner = FindObjectOfType<MeteorSpawner>();
        if (spawner != null)
            spawner.enabled = false;

        // 恢复时间流逝（确保按钮可点击）
        Time.timeScale = 1;
    }

    // 重新开始游戏
    public void RestartGame()
    {
        // 重新加载当前场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        // 恢复时间流逝
        Time.timeScale = 1;
    }
}
