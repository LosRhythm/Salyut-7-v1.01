using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(VideoPlayer))]
public class VideoToScene : MonoBehaviour
{
    [Header("跳转设置")]
    [Tooltip("视频播放完成后要跳转的场景名称")]
    public string targetSceneName;

    [Header("控制设置")]
    [Tooltip("是否允许按任意键跳过视频")]
    public bool allowSkip = true;

    [Tooltip("是否隐藏鼠标光标")]
    public bool hideCursor = true;

    private VideoPlayer videoPlayer;
    private bool hasSkipped = false;

    void Awake()
    {
        // 获取组件引用
        videoPlayer = GetComponent<VideoPlayer>();

        // 确保视频不会循环播放
        //videoPlayer.loop = false;
    }

    void Start()
    {
        // 隐藏鼠标光标
        if (hideCursor)
            Cursor.visible = false;

        // 注册视频播放完成事件
        videoPlayer.loopPointReached += OnVideoFinished;

        // 开始播放视频
        videoPlayer.Play();
    }

    void Update()
    {
        // 处理跳过视频功能
        if (allowSkip && !hasSkipped && Input.anyKeyDown)
        {
            SkipVideo();
        }
    }

    // 视频播放完成时调用
    private void OnVideoFinished(VideoPlayer source)
    {
        if (!hasSkipped)
        {
            LoadTargetScene();
        }
    }

    // 跳过视频
    private void SkipVideo()
    {
        hasSkipped = true;
        videoPlayer.Stop();
        LoadTargetScene();
    }

    // 加载目标场景
    private void LoadTargetScene()
    {
        // 恢复鼠标显示
        if (hideCursor)
            Cursor.visible = true;


        try
        {
            // 检查场景是否存在于Build Settings中
            if (IsSceneInBuildSettings(targetSceneName))
            {

                SceneManager.LoadScene(targetSceneName);
            }
            else
            {
                Debug.LogError($"场景 {targetSceneName} 未添加到Build Settings中！");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"加载场景时出错: {e.Message}");
        }
    }

    // 检查场景是否已添加到Build Settings
    private bool IsSceneInBuildSettings(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneFileName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if (sceneFileName == sceneName)
            {
                return true;
            }
        }
        return false;
    }

    // 移除事件监听，防止内存泄漏
    void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }
}
