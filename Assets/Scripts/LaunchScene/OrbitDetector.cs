using System.Transactions;
using TMPro;
using UnityEngine;

public class OrbitDetector : MonoBehaviour
{
    [Header("轨道参数")]
    public Vector2 orbitCenter = Vector2.zero; // 椭圆中心
    public float semiMajorAxis = 100f; // 主轨道长半轴
    public float semiMinorAxis = 60f; // 主轨道短半轴
    public float orbitRotation = 0f; // 椭圆旋转角度（度）
    public float orbitTolerance = 10f; // 轨道容差范围
    public float requiredOrbitTime = 3f; // 进入轨道所需时间

    [Header("轨道显示设置")]
    public Color mainOrbitColor = Color.cyan; // 主轨道颜色
    public Color innerOrbitColor = new Color(0.5f, 1f, 1f, 0.5f); // 内轨道颜色（半透明）
    public Color outerOrbitColor = new Color(0.5f, 1f, 1f, 0.5f); // 外轨道颜色（半透明）
    public float orbitLineWidth = 2f; // 轨道线宽
    public bool drawOrbitInGame = true; // 是否在游戏视图中显示轨道

    [Header("引用")]
    public Transform rocket; // 火箭引用
    public GameManager gameManager; // 游戏管理器
    public TMP_Text orbitStatusText; // 轨道状态文本

    private float currentOrbitTime; // 当前在轨道内的时间
    private bool isInOrbitRange; // 是否在轨道范围内
    private bool hasEnteredOrbit; // 是否已进入轨道
    private bool isDisplayingCountdown; // 是否正在显示倒计时

    // 计算内外轨道参数
    private float innerSemiMajorAxis => semiMajorAxis - orbitTolerance;
    private float innerSemiMinorAxis => semiMinorAxis - orbitTolerance;
    private float outerSemiMajorAxis => semiMajorAxis + orbitTolerance;
    private float outerSemiMinorAxis => semiMinorAxis + orbitTolerance;

    private void Start()
    {
        // 默认引用设置
        if (rocket == null)
            rocket = transform;

        // 初始化UI
        if (orbitStatusText != null)
        {
            orbitStatusText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (hasEnteredOrbit) return;

        // 检查是否在椭圆轨道范围内（考虑容差）
        if (rocket != null)
        {
            isInOrbitRange = IsPointInEllipse(rocket.position, orbitCenter, semiMajorAxis, semiMinorAxis, orbitRotation, orbitTolerance);
        }
        

        // 检查是否沿切线方向运动
        bool isMovingTangentially = IsMovingTangentially();

        // 轨道判定逻辑
        if (isInOrbitRange && isMovingTangentially)
        {
            currentOrbitTime += Time.deltaTime;

            // 显示倒计时
            if (orbitStatusText != null)
            {
                isDisplayingCountdown = true;
                orbitStatusText.gameObject.SetActive(true);
                float remainingTime = requiredOrbitTime - currentOrbitTime;
                orbitStatusText.text = $"正在进入轨道，倒计时：{Mathf.Ceil(remainingTime)}秒";
            }

            // 达到所需时间，判定为进入轨道
            if (currentOrbitTime >= requiredOrbitTime)
            {
                EnterOrbit();
            }
        }
        else
        {
            // 不符合条件则重置
            currentOrbitTime = 0;

            // 隐藏倒计时（如果正在显示）
            if (isDisplayingCountdown && orbitStatusText != null)
            {
                orbitStatusText.gameObject.SetActive(false);
                isDisplayingCountdown = false;
            }
        }
    }

    // 检查点是否在椭圆容差范围内
    private bool IsPointInEllipse(Vector2 point, Vector2 center, float majorAxis, float minorAxis, float rotation, float tolerance)
    {
        // 将点转换到椭圆局部坐标系
        Vector2 translatedPoint = point - center;
        float rotationRad = rotation * Mathf.Deg2Rad;

        // 旋转点以抵消椭圆的旋转
        float cos = Mathf.Cos(rotationRad);
        float sin = Mathf.Sin(rotationRad);
        float x = translatedPoint.x * cos + translatedPoint.y * sin;
        float y = -translatedPoint.x * sin + translatedPoint.y * cos;

        // 计算椭圆方程（考虑容差范围）
        float innerThreshold = (majorAxis - tolerance) * (majorAxis - tolerance);
        float outerThreshold = (majorAxis + tolerance) * (majorAxis + tolerance);

        // 椭圆方程的变形检查，同时检查内外边界
        float value = (x * x) * (minorAxis * minorAxis) + (y * y) * (majorAxis * majorAxis);
        float maxValue = (majorAxis + tolerance) * (majorAxis + tolerance) * (minorAxis + tolerance) * (minorAxis + tolerance);
        float minValue = (majorAxis - tolerance) * (majorAxis - tolerance) * (minorAxis - tolerance) * (minorAxis - tolerance);

        return value <= maxValue && value >= minValue;
    }

    // 检查是否沿切线方向运动
    private bool IsMovingTangentially()
    {
        if(rocket == null)
        {
            return false;
        }
        Rigidbody2D rb = rocket.GetComponent<Rigidbody2D>();
        if (rb == null || rb.velocity.sqrMagnitude < 0.1f)
            return false;

        // 计算椭圆上该点的法线方向
        Vector2 normal = GetEllipseNormalAtPoint(rocket.position);

        // 计算速度方向与法线的夹角
        float dotProduct = Vector2.Dot(normal, rb.velocity.normalized);

        // 接近0表示垂直，即沿切线方向
        return Mathf.Abs(dotProduct) < 0.3f;
    }

    // 获取椭圆上某点的法线方向
    private Vector2 GetEllipseNormalAtPoint(Vector2 point)
    {
        // 计算从中心到点的向量
        Vector2 toPoint = point - orbitCenter;
        float rotationRad = orbitRotation * Mathf.Deg2Rad;

        // 旋转向量以匹配椭圆角度
        float cos = Mathf.Cos(rotationRad);
        float sin = Mathf.Sin(rotationRad);
        float x = toPoint.x * cos + toPoint.y * sin;
        float y = -toPoint.x * sin + toPoint.y * cos;

        // 计算椭圆法线
        float normalX = x / (semiMajorAxis * semiMajorAxis);
        float normalY = y / (semiMinorAxis * semiMinorAxis);

        // 旋转回原坐标系
        Vector2 normal = new Vector2(
            normalX * cos - normalY * sin,
            normalX * sin + normalY * cos
        ).normalized;

        return normal;
    }

    // 进入轨道处理
    private void EnterOrbit()
    {
        hasEnteredOrbit = true;
        Debug.Log("成功进入轨道！");

        // 隐藏倒计时文本
        if (orbitStatusText != null)
        {
            orbitStatusText.gameObject.SetActive(false);
        }

        // 触发场景切换
        if (gameManager != null)
        {

            gameManager.LoadDockingScene();

        }
    }

    // 在游戏视图中绘制椭圆轨道和容差范围
    private void OnDrawGizmos()
    {
        if (!drawOrbitInGame) return;

        // 保存当前Gizmos矩阵用于线宽设置
        Matrix4x4 originalMatrix = Gizmos.matrix;

        // 绘制内轨道
        Gizmos.color = innerOrbitColor;
        Gizmos.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1, 1, 1));
        DrawEllipse(orbitCenter, innerSemiMajorAxis, innerSemiMinorAxis, orbitRotation);

        // 绘制主轨道
        Gizmos.color = mainOrbitColor;
        DrawEllipse(orbitCenter, semiMajorAxis, semiMinorAxis, orbitRotation);

        // 绘制外轨道
        Gizmos.color = outerOrbitColor;
        DrawEllipse(orbitCenter, outerSemiMajorAxis, outerSemiMinorAxis, orbitRotation);

        // 恢复原始矩阵
        Gizmos.matrix = originalMatrix;
    }

    // 绘制椭圆
    private void DrawEllipse(Vector2 center, float majorAxis, float minorAxis, float rotationDegrees)
    {
        int segments = 60; // 椭圆分段数，越多越平滑
        float rotation = rotationDegrees * Mathf.Deg2Rad;

        // 计算第一个点
        Vector2 prevPoint = center + RotatePoint(
            new Vector2(majorAxis, 0),
            rotation
        );

        // 绘制椭圆线段
        for (int i = 1; i <= segments; i++)
        {
            float angle = (i / (float)segments) * Mathf.PI * 2;
            Vector2 currentPoint = center + RotatePoint(
                new Vector2(Mathf.Cos(angle) * majorAxis, Mathf.Sin(angle) * minorAxis),
                rotation
            );

            Gizmos.DrawLine(prevPoint, currentPoint);
            prevPoint = currentPoint;
        }
    }

    // 旋转点
    private Vector2 RotatePoint(Vector2 point, float rotation)
    {
        float cos = Mathf.Cos(rotation);
        float sin = Mathf.Sin(rotation);
        return new Vector2(
            point.x * cos - point.y * sin,
            point.x * sin + point.y * cos
        );
    }
}
