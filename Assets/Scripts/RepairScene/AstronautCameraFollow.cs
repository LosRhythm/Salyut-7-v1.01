using UnityEngine;

public class AstronautCameraFollow : MonoBehaviour
{
    [Header("跟踪目标设置")]
    [SerializeField] private Transform target; // 要跟踪的宇航员Transform
    [SerializeField] private float followDelay = 0.1f; // 平滑跟随延迟（值越小越灵敏）

    [Header("视角偏移设置")]
    [SerializeField] private Vector2 offset = new(0, 0.5f); // 相机相对宇航员的偏移（Y轴偏移避免遮挡）

    [Header("边界限制设置")]
    [SerializeField] private bool useBoundary = true; // 是否启用场景边界限制
    [SerializeField] private Vector2 minBoundary = new(-10, -5); // 相机左/下边界
    [SerializeField] private Vector2 maxBoundary = new(10, 5); // 相机右/上边界

    [Header("相机大小设置")]
    [SerializeField] private float cameraSize = 5f; // 2D相机的正交大小（控制视野范围）

    private Camera mainCamera;
    private Vector3 smoothVelocity = Vector3.zero; // 平滑跟随的速度缓存


    void Start()
    {
        // 获取主相机组件（确保是2D场景的正交相机）
        mainCamera = GetComponent<Camera>();
        if (mainCamera == null)
        {
            Debug.LogError("脚本挂载的对象没有Camera组件！请将脚本挂载到主相机上");
            enabled = false; // 禁用脚本避免报错
            return;
        }

        // 初始化相机正交大小
        mainCamera.orthographicSize = cameraSize;

        // 检查是否指定了跟踪目标
        if (target == null)
        {
            Debug.LogWarning("未指定跟踪目标！尝试自动查找宇航员...");
            FindAstronautTarget();
        }
    }


    void FixedUpdate()
    {
        // 如果没有跟踪目标，直接返回
        if (target == null) return;

        // 计算相机的目标位置（加入偏移量，Z轴保持相机原始深度）
        Vector3 targetPosition = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            transform.position.z // 保持相机Z轴不变（避免穿模）
        );

        // 应用场景边界限制（防止相机显示场景外空白）
        if (useBoundary)
        {
            targetPosition = ClampToBoundary(targetPosition);
        }

        // 平滑跟随（使用Vector3.SmoothDamp实现缓动效果，避免相机抖动）
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref smoothVelocity,
            followDelay
        );
    }


    /// <summary>
    /// 将相机位置限制在设定的边界内
    /// </summary>
    private Vector3 ClampToBoundary(Vector3 position)
    {
        // 计算相机边缘对应的世界坐标（根据正交大小和屏幕宽高比）
        float halfWidth = mainCamera.orthographicSize * mainCamera.aspect;
        float halfHeight = mainCamera.orthographicSize;

        // 限制X轴（左右边界）
        float clampedX = Mathf.Clamp(
            position.x,
            minBoundary.x + halfWidth,  // 左边界 = 最小X + 相机半宽
            maxBoundary.x - halfWidth   // 右边界 = 最大X - 相机半宽
        );

        // 限制Y轴（上下边界）
        float clampedY = Mathf.Clamp(
            position.y,
            minBoundary.y + halfHeight, // 下边界 = 最小Y + 相机半高
            maxBoundary.y - halfHeight  // 上边界 = 最大Y - 相机半高
        );

        return new Vector3(clampedX, clampedY, position.z);
    }


    /// <summary>
    /// 自动查找场景中的宇航员（依赖Astronaut脚本标记）
    /// </summary>
    private void FindAstronautTarget()
    {
        PlayerController astronaut = FindAnyObjectByType<PlayerController>();
        if (astronaut != null)
        {
            target = astronaut.transform;
            Debug.Log($"自动找到宇航员：{astronaut.gameObject.name}");
        }
        else
        {
            Debug.LogError("场景中未找到挂载Astronaut脚本的对象！请手动指定跟踪目标");
        }
    }


    /// <summary>
    /// 在Scene视图绘制边界Gizmos（方便调试）
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (!useBoundary) return;

        // 绘制边界矩形（黄色线框）
        Gizmos.color = Color.yellow;
        Vector3 bottomLeft = new(minBoundary.x, minBoundary.y, 0);
        Vector3 bottomRight = new(maxBoundary.x, minBoundary.y, 0);
        Vector3 topRight = new(maxBoundary.x, maxBoundary.y, 0);
        Vector3 topLeft = new(minBoundary.x, maxBoundary.y, 0);

        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);

        // 绘制相机当前视野范围（青色线框）
        if (mainCamera != null)
        {
            Gizmos.color = Color.cyan;
            float halfWidth = mainCamera.orthographicSize * mainCamera.aspect;
            float halfHeight = mainCamera.orthographicSize;
            Vector3 camBottomLeft = new(transform.position.x - halfWidth, transform.position.y - halfHeight, 0);
            Vector3 camBottomRight = new(transform.position.x + halfWidth, transform.position.y - halfHeight, 0);
            Vector3 camTopRight = new(transform.position.x + halfWidth, transform.position.y + halfHeight, 0);
            Vector3 camTopLeft = new(transform.position.x - halfWidth, transform.position.y + halfHeight, 0);

            Gizmos.DrawLine(camBottomLeft, camBottomRight);
            Gizmos.DrawLine(camBottomRight, camTopRight);
            Gizmos.DrawLine(camTopRight, camTopLeft);
            Gizmos.DrawLine(camTopLeft, camBottomLeft);
        }
    }
}