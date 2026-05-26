using UnityEngine;

public class ResolutionController : MonoBehaviour
{
    void Awake() // 建议用Awake确保在Start前执行
    {
        // 窗口模式1920x1080
        Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);

        // 若需全屏模式，可改为：
        // Screen.SetResolution(1920, 1080, FullScreenMode.FullScreen);
    }
}