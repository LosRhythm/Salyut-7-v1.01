using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class AstronautManager : MonoBehaviour
{
    public static AstronautManager Instance;

    [Header("引用设置")]
    [Tooltip("宇航员游戏对象")]
    public GameObject astronaut;
    [Tooltip("游戏结束面板")]
    public GameObject gameOverPanel;
    [Tooltip("重新开始按钮")]
    public Button restartButton;

    [Header("游戏设置")]
    [Tooltip("死亡后显示面板的延迟时间")]
    public float deathDelay = 1.5f;

    // 游戏状态
    public bool IsGameOver { get; private set; }

    private void Awake()
    {
        // 确保单例模式
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 注册按钮事件
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        // 初始隐藏游戏结束面板
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    // 宇航员被陨石击中
    public void OnAstronautHit()
    {
        if (IsGameOver) return;

        IsGameOver = true;

        astronaut.GetComponent<Animator>().SetTrigger("IsDied");

        // 禁用宇航员控制
        if (astronaut != null)
        {
            // 禁用移动组件
            MonoBehaviour[] components = astronaut.GetComponents<MonoBehaviour>();
            foreach (var component in components)
            {
                // 保留动画组件，禁用其他组件（如移动控制）
                if (!(component is Animator))
                {
                    component.enabled = false;
                }
            }
        }

        // 延迟显示游戏结束面板
        Invoke(nameof(ShowGameOverPanel), deathDelay);
    }

    // 显示游戏结束面板
    private void ShowGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            // 暂停游戏
            Time.timeScale = 0;
        }
        else
        {
            Debug.LogError("未设置游戏结束面板！", this);
        }
    }

    // 重新开始游戏
    public void RestartGame()
    {
        // 恢复时间缩放
        Time.timeScale = 1;
        // 重新加载当前场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
