using UnityEngine;

public class SatelliteOrbit : MonoBehaviour
{
    // 地球的Transform组件，作为旋转中心
    [Tooltip("指定要围绕旋转的地球对象")]
    public Transform earthTransform;

    // 轨道半径
    [Tooltip("卫星与地球中心的距离")]
    public float orbitRadius = 10f;

    // 公转速度（度/秒）
    [Tooltip("卫星绕地球旋转的速度")]
    public float orbitSpeed = 30f;

    // 轨道倾斜角度
    [Tooltip("轨道与水平面的夹角（度）")]
    public float orbitTilt = 0f;

    void Start()
    {
        // 如果未指定地球，尝试查找名为"Earth"的对象
        if (earthTransform == null)
        {
            GameObject earth = GameObject.Find("Earth");
            if (earth != null)
            {
                earthTransform = earth.transform;
            }
        }

        // 初始化卫星位置到轨道上
        if (earthTransform != null)
        {
            SetInitialPosition();
        }
    }

    void Update()
    {
        // 如果地球引用存在，则执行旋转
        if (earthTransform != null)
        {
            OrbitAroundEarth();
        }
    }

    // 设置卫星初始位置
    void SetInitialPosition()
    {
        // 计算初始位置（在X轴方向距离地球orbitRadius的位置）
        Vector3 initialPosition = new Vector3(orbitRadius, 0, 0);

        // 应用轨道倾斜
        if (orbitTilt != 0)
        {
            initialPosition = Quaternion.Euler(orbitTilt, 0, 0) * initialPosition;
        }

        // 设置卫星位置（相对于地球）
        transform.position = earthTransform.position + initialPosition;

        // 让卫星始终面向地球
        transform.LookAt(earthTransform);
    }

    // 执行绕地球旋转
    void OrbitAroundEarth()
    {
        // 计算旋转角度（使用Time.deltaTime确保速度稳定）
        float angle = orbitSpeed * Time.deltaTime;

        // 绕地球的Y轴旋转（考虑轨道倾斜）
        transform.RotateAround(
            earthTransform.position,
            Quaternion.Euler(orbitTilt, 0, 0) * Vector3.up,
            angle
        );

        // 保持卫星始终面向地球（可选）
        transform.LookAt(earthTransform);
    }
}
