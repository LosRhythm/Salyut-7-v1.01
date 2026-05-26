using UnityEngine;

public class SpaceshipCameraFollow : MonoBehaviour
{
    [Header("目标设置")]
    [Tooltip("要跟随的航天器")]
    public Transform followTarget;

    [Tooltip("参考的空间站目标")]
    public Transform spaceStation;  // 新增：空间站目标

    [Header("跟随设置")]
    [Tooltip("跟随偏移量（相对于目标的位置）")]
    public Vector3 followOffset = new Vector3(0, 5, -10);

    [Tooltip("跟随平滑度（值越小越平滑）")]
    [Range(0.1f, 2f)]
    public float smoothSpeed = 0.5f;

    [Header("旋转设置")]
    [Tooltip("是否跟随目标旋转")]
    public bool followRotation = true;

    [Tooltip("旋转平滑度")]
    [Range(0.1f, 2f)]
    public float rotationSmoothSpeed = 0.5f;

    [Header("缩放设置")]  // 新增：缩放相关设置
    [Tooltip("最大视野角度（远离时）")]
    public float maxFieldOfView = 60f;

    [Tooltip("最小视野角度（靠近时）")]
    public float minFieldOfView = 30f;

    [Tooltip("开始缩放的距离阈值")]
    public float zoomStartDistance = 50f;

    [Tooltip("缩放平滑度")]
    [Range(0.1f, 5f)]
    public float zoomSmoothSpeed = 2f;

    [Header("限制设置")]
    [Tooltip("是否限制相机高度")]
    public bool limitHeight = false;

    [Tooltip("最低高度限制")]
    public float minHeight = 2f;

    [Tooltip("最高高度限制")]
    public float maxHeight = 20f;

    private Camera mainCamera;

    private void Awake()
    {
        // 获取主相机组件
        mainCamera = GetComponent<Camera>();

        // 初始化视野
        if (mainCamera != null)
        {
            mainCamera.fieldOfView = maxFieldOfView;
        }
    }

    private void FixedUpdate()
    {
        if (followTarget == null)
        {
            Debug.LogWarning("没有指定跟随目标，请在Inspector中设置目标航天器");
            return;
        }

        // 计算目标位置（目标位置 + 偏移量）
        Vector3 desiredPosition = followTarget.TransformPoint(followOffset);

        // 应用高度限制
        if (limitHeight)
        {
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minHeight, maxHeight);
        }

        // 平滑移动到目标位置
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime * 10f);
        transform.position = smoothedPosition;

        // 跟随旋转
        if (followRotation)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(followTarget.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, rotationSmoothSpeed * Time.deltaTime * 10f);
        }

        // 新增：根据距离空间站的距离调整视野（镜头缩放）
        AdjustZoomBasedOnDistance();
    }

    // 新增：根据与空间站的距离调整镜头缩放
    private void AdjustZoomBasedOnDistance()
    {
        if (mainCamera == null || followTarget == null || spaceStation == null)
            return;

        // 计算航天器与空间站之间的距离
        float distanceToStation = Vector3.Distance(followTarget.position, spaceStation.position);

        // 根据距离计算目标视野（距离越近，视野越小，看起来越大）
        float targetFOV;
        if (distanceToStation >= zoomStartDistance)
        {
            // 超出距离阈值，使用最大视野
            targetFOV = maxFieldOfView;
        }
        else if (distanceToStation <= 0)
        {
            // 距离为0时使用最小视野
            targetFOV = minFieldOfView;
        }
        else
        {
            // 距离在阈值范围内，插值计算视野
            float t = distanceToStation / zoomStartDistance;
            targetFOV = Mathf.Lerp(minFieldOfView, maxFieldOfView, t);
        }

        // 平滑过渡到目标视野
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, zoomSmoothSpeed * Time.deltaTime);
    }
}
