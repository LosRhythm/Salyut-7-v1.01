using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SpaceshipThrustControllerInverted : MonoBehaviour
{
    [Header("推进力设置")]
    [Tooltip("飞船推进力大小")]
    public float thrustForce = 500f;

    [Tooltip("是否启用阻尼效果(模拟太空阻力)")]
    public bool useDamping = false;

    [Tooltip("阻尼系数(值越大减速越快)")]
    public float dampingFactor = 0.9f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // 配置物理属性以模拟太空环境
        rb.useGravity = false; // 禁用重力
        rb.freezeRotation = true; // 冻结旋转
    }

    private void FixedUpdate()
    {
        // 获取输入
        float zInput = Input.GetAxis("Horizontal"); // A/D控制本地Z轴
        float xInput = Input.GetAxis("Vertical");   // W/S控制本地X轴

        // 基于本地坐标系计算推力方向，并反转所有轴
        // 通过添加负号实现X轴和Z轴的方向反转
        Vector3 thrustDirection = -transform.right * xInput + transform.forward * zInput;

        // 施加力
        if (thrustDirection.sqrMagnitude > 0.1f)
        {
            rb.AddForce(thrustDirection * thrustForce * Time.fixedDeltaTime);
        }

        // 可选的阻尼效果
        if (useDamping && rb.velocity.sqrMagnitude > 0.1f)
        {
            rb.velocity *= dampingFactor;
        }
    }
}
