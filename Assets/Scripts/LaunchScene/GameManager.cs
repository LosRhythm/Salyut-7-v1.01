using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    [Header("UI引用")]
    public GameObject gameOverPanel;
    public CustomGUILabel gameOverText;
    public CustomGUIButton reStartButton;
    public CustomGUIButton quitButton;
    public GameObject startPanel;

    [Header("场景设置")]
    public string dockingSceneName = "DockingScene";

    private bool isGameActive = false;

    private void Start()
    {
        //初始化UI状态
        gameOverPanel.SetActive(false);
        if(startPanel  != null)
        {
            startPanel.SetActive(true);
        }

        if (reStartButton != null)
        {
            reStartButton.clickEvent += RestartGame;
        }

        if (quitButton != null)
        {
            quitButton.clickEvent += QuitGame;
        }
    }

    // 防止脚本被销毁时还保留事件引用
    private void OnDestroy()
    {
        if (reStartButton != null)
        {
            reStartButton.clickEvent -= RestartGame;
        }

        if (quitButton != null)
        {
            quitButton.clickEvent -= QuitGame;
        }
    }

    public void OnLaunch()
    {
        isGameActive = true;
        if (startPanel != null)
        {
            startPanel.SetActive(false);
        }
    }

    public void OnEnterSpace()
    {
        isGameActive =false;
        // 延迟后加载对接场景
        Invoke("LoadDockingScene", 3f); // 等待3秒过场动画

    }

    // 游戏结束
    public void GameOver(string reason)
    {
        if (isGameActive)
        {
            isGameActive = false;
            gameOverPanel.SetActive(true);
            gameOverText.content.text = reason + "\n是否重新开始?";
        }
    }

    // 重新开始游戏
    public void RestartGame()
    {
        SceneManager.LoadScene("LaunchScene");
    }


    // 加载对接场景
    public void LoadDockingScene()
    {
        SceneManager.LoadScene("Scene1-2");
    }

    // 退出游戏
    public void QuitGame()
    {
        Application.Quit();
        // 在编辑器中停止播放
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }


}
