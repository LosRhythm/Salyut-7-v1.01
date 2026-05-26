using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Header("目标设置")]
    public Transform target; // 要跟随的火箭

    [Header("缩放设置")]
    [Tooltip("发射前的初始缩放（值越小放大倍数越大）")]
    public float initialOrthoSize = 3f;

    [Tooltip("最大缩小值（值越大显示范围越广）")]
    public float maxOrthoSize = 10f;

    [Tooltip("缩放变化的灵敏度")]
    public float zoomSensitivity = 0.05f;

    [Header("跟随设置")]
    [Tooltip("平滑跟随系数")]
    public float smoothFactor = 0.1f;

    [Tooltip("摄像机相对于目标的偏移量")]
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("发射检测")]
    [Tooltip("判断为发射的最小速度阈值")]
    public float launchSpeedThreshold = 2f;

    [Tooltip("是否已发射")]
    public bool isLaunched = false;

    private Rigidbody2D targetRb;
    private Camera mainCamera;
    private float currentOrthoSize;
    private Vector3 currentVelocity = Vector3.zero;

    void Start()
    {
        mainCamera = GetComponent<Camera>();

        // 确保摄像机为正交模式（2D必备）
        if (!mainCamera.orthographic)
        {
            mainCamera.orthographic = true;
            Debug.LogWarning("已自动将摄像机设置为正交模式以支持2D缩放");
        }

        // 初始化缩放
        currentOrthoSize = initialOrthoSize;
        mainCamera.orthographicSize = currentOrthoSize;

        // 获取火箭的刚体组件
        if (target != null)
        {
            targetRb = target.GetComponent<Rigidbody2D>();
        }
        else
        {
            Debug.LogError("请指定要跟随的目标火箭！");
        }
    }

    void FixedUpdate()
    {
        if (target == null || targetRb == null) return;

        // 检测发射状态
        CheckLaunchStatus();

        // 如果已发射，根据速度调整缩放
        if (isLaunched)
        {
            AdjustZoomBasedOnSpeed();
        }

        // 平滑跟随目标位置
        Vector3 desiredPosition = target.position + offset;
        desiredPosition.z = transform.position.z; // 保持Z轴不变

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref currentVelocity,
            smoothFactor
        );

        // 应用当前缩放
        mainCamera.orthographicSize = Mathf.Lerp(
            mainCamera.orthographicSize,
            currentOrthoSize,
            smoothFactor * 5
        );
    }

    // 检测火箭是否已经发射
    private void CheckLaunchStatus()
    {
        if (!isLaunched && targetRb.velocity.magnitude > launchSpeedThreshold)
        {
            isLaunched = true;
            Debug.Log("火箭已发射，开始动态调整相机缩放");
        }
    }

    // 根据火箭速度调整相机缩放
    private void AdjustZoomBasedOnSpeed()
    {
        // 获取火箭当前速度
        float speed = targetRb.velocity.magnitude;

        // 根据速度计算目标缩放（不超过最大值）
        float targetOrthoSize = initialOrthoSize + (speed * zoomSensitivity);
        targetOrthoSize = Mathf.Clamp(targetOrthoSize, initialOrthoSize, maxOrthoSize);

        // 平滑过渡到目标缩放
        currentOrthoSize = Mathf.Lerp(currentOrthoSize, targetOrthoSize, smoothFactor * 2);
    }

    // 绘制辅助线
    void OnDrawGizmos()
    {
        if (target != null)
        {
            Gizmos.color = isLaunched ? Color.blue : Color.green;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
}
