using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 主界面管理类
/// </summary> <summary>
/// 主界面管理类，负责主界面的按钮事件
/// 包含开始游戏和退出游戏的按钮事件
/// 开始游戏按钮事件，加载游戏场景
/// 退出游戏按钮事件，退出游戏
/// 调用Application.Quit()方法退出游戏
/// 注意：在编辑器中测试时，需要先构建项目才能退出游戏
/// </summary>

public class PanelSystem : MonoBehaviour
{
    public Button startButton;
    public Button quitButton;


    // Start is called before the first frame update
    void Start()
    {
        startButton.onClick.AddListener(StartGame);

        quitButton.onClick.AddListener(ExitGame);
    }


    void ExitGame()
    {
        Application.Quit();

        Debug.Log("程序退出");
    }

    void StartGame()
    {
        SceneManager.LoadScene("Assets/Scenes/LaunchScene.unity");
    }

}
