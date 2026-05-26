using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrbitTracker : MonoBehaviour
{
    [Header("目标设置")]
    [Tooltip("航天器自身Transform")]
    public Transform spaceship;

    [Tooltip("需要跟踪的空间站")]
    public Transform spaceStation;

    [Header("UI设置")]
    [Tooltip("指示箭头图片")]
    public Image directionArrow;

    [Tooltip("显示距离的文本")]
    public TextMeshProUGUI distanceText;

    [Tooltip("指示箭头的父级容器(应位于屏幕中央区域)")]
    public RectTransform arrowContainer;

    [Header("跟踪设置")]
    [Tooltip("最大指示距离(超出此距离不显示)")]
    public float maxTrackingDistance = 2000f;

    [Tooltip("箭头在屏幕边缘的限制区域(0-1之间)")]
    [Range(0.1f, 0.45f)]
    public float screenEdgeMargin = 0.4f;

    private Camera mainCamera;
    private RectTransform arrowRect;

    private void Awake()
    {
        // 获取主相机
        mainCamera = Camera.main;

        // 获取箭头的RectTransform
        if (directionArrow != null)
        {
            arrowRect = directionArrow.GetComponent<RectTransform>();
        }

        // 初始隐藏指示
        SetIndicatorActive(false);
    }

    private void Update()
    {
        // 检查必要引用
        if (spaceship == null || spaceStation == null || mainCamera == null ||
            directionArrow == null || distanceText == null || arrowContainer == null)
        {
            SetIndicatorActive(false);
            return;
        }

        // 计算航天器到空间站的距离
        float distance = Vector3.Distance(spaceship.position, spaceStation.position);

        // 超出最大跟踪距离则隐藏指示
        if (distance > maxTrackingDistance)
        {
            SetIndicatorActive(false);
            return;
        }

        // 显示指示
        SetIndicatorActive(true);

        // 更新距离文本
        distanceText.text = $"{distance:F1}m";

        // 计算空间站在屏幕上的位置
        UpdateArrowPositionAndRotation();
    }

    // 更新箭头的位置和旋转
    private void UpdateArrowPositionAndRotation()
    {
        // 将空间站世界坐标转换为屏幕坐标
        Vector3 stationScreenPos = mainCamera.WorldToViewportPoint(spaceStation.position);

        // 检查是否在相机视野内(0-1范围)
        bool isInView = stationScreenPos.z > 0 &&
                       stationScreenPos.x > 0 && stationScreenPos.x < 1 &&
                       stationScreenPos.y > 0 && stationScreenPos.y < 1;

        if (isInView)
        {
            // 如果在视野内，箭头显示在实际位置
            SetArrowPosition(stationScreenPos);
        }
        else
        {
            // 如果不在视野内，箭头显示在屏幕边缘
            Vector3 clampedPos = ClampToScreenEdge(stationScreenPos);
            SetArrowPosition(clampedPos);
        }

        // 计算箭头指向(指向空间站方向)
        Vector3 directionToStation = (spaceStation.position - spaceship.position).normalized;
        Vector3 directionInScreen = mainCamera.WorldToScreenPoint(spaceship.position + directionToStation) -
                                   mainCamera.WorldToScreenPoint(spaceship.position);

        // 计算箭头旋转角度
        float angle = Mathf.Atan2(directionInScreen.y, directionInScreen.x) * Mathf.Rad2Deg - 90f;
        arrowRect.rotation = Quaternion.Euler(0, 0, angle);
    }

    // 设置箭头在屏幕上的位置
    private void SetArrowPosition(Vector3 viewportPos)
    {
        // 将视口坐标转换为UI坐标
        Vector2 anchoredPos = new Vector2(
            (viewportPos.x - 0.5f) * arrowContainer.rect.width,
            (viewportPos.y - 0.5f) * arrowContainer.rect.height
        );

        arrowRect.anchoredPosition = anchoredPos;
    }

    // 将位置限制在屏幕边缘
    private Vector3 ClampToScreenEdge(Vector3 viewportPos)
    {
        Vector3 clamped = viewportPos;

        // 如果在相机后方，翻转X和Y
        if (clamped.z < 0)
        {
            clamped.x = 1 - clamped.x;
            clamped.y = 1 - clamped.y;
            clamped.z = 0;
        }

        // 限制X在屏幕边缘范围内
        clamped.x = Mathf.Clamp(clamped.x, 0.5f - screenEdgeMargin, 0.5f + screenEdgeMargin);
        // 限制Y在屏幕边缘范围内
        clamped.y = Mathf.Clamp(clamped.y, 0.5f - screenEdgeMargin, 0.5f + screenEdgeMargin);

        return clamped;
    }

    // 设置指示是否激活
    private void SetIndicatorActive(bool active)
    {
        if (directionArrow != null)
            directionArrow.gameObject.SetActive(active);

        if (distanceText != null)
            distanceText.gameObject.SetActive(active);
    }
}
