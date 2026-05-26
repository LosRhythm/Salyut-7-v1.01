using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 控制物体沿Y轴向上移动的脚本，支持定时移动及移动结束后场景切换
/// </summary>
public class MoveUp : MonoBehaviour
{
    [Tooltip("物体向上移动的速度，单位为米/秒")]
    public float moveSpeed = 1.0f;

    [Tooltip("物体持续移动的时间（单位：秒），若设置为0则会一直移动")]
    public float moveTime = 5.0f;

    // 已流逝的移动时间
    private float _elapsedTime = 0f;
    // 移动状态标记：true表示正在移动，false表示停止移动
    private bool _isMoving = true;

    void Update()
    {
        // 仅在允许移动的状态下执行移动逻辑
        if (_isMoving)
        {
            // 计算当前帧应移动的距离（速度 * 帧间隔时间）
            float moveDistance = moveSpeed * Time.deltaTime;

            // 沿Y轴正方向移动物体
            transform.Translate(Vector3.up * moveDistance);

            // 当设置了有效移动时间（大于0）时，进行计时
            if (moveTime > 0)
            {
                _elapsedTime += Time.deltaTime;

                // 当已移动时间达到设定时间时，停止移动并切换场景
                if (_elapsedTime >= moveTime)
                {
                    _isMoving = false;
                    SceneManager.LoadScene("Assets/Scenes/BeginScene.unity");
                }
            }
        }
    }

    /// <summary>
    /// 开始物体移动（可通过外部调用重新激活移动）
    /// </summary>
    public void StartMoving()
    {
        _isMoving = true;
        _elapsedTime = 0f; // 重置计时
    }

    /// <summary>
    /// 停止物体移动（可通过外部调用暂停移动）
    /// </summary>
    public void StopMoving()
    {
        _isMoving = false;
    }
}