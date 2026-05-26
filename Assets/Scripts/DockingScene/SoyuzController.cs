using UnityEngine;

public class SoyuzController : MonoBehaviour
{
    [Header("移动设置")]
    public float forwardSpeed = 8f;      // 前进后退速度
    public float strafeSpeed = 5f;       // 左右平移速度
    public float rotationSpeed = 120f;   // 旋转速度
    public float levelRotationSpeed = 200f; // 水平重置旋转速度

    [Header("质心设置")]
    public Transform centerOfMass;       // 手动指定质心位置（可选）

    private Rigidbody2D rb;
    private float forwardInput;
    private float strafeInput;
    private float rotationInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // 使用动力学刚体以获得更好的物理效果
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0; // 太空中没有重力
        rb.drag = 0.5f;      // 添加一点阻力，使控制更平滑

        // 设置质心（如果指定了质心Transform）
        if (centerOfMass != null)
        {
            rb.centerOfMass = transform.InverseTransformPoint(centerOfMass.position);
        }
    }

    void Update()
    {
        // 获取输入 - 使用WS控制上下(前进后退)
        forwardInput = Input.GetAxis("Vertical"); // W是+1，S是-1

        // 使用AD控制左右(横向移动)
        strafeInput = Input.GetAxis("Horizontal"); // D是+1，A是-1

        // 使用QE控制旋转
        rotationInput = 0;
        if (Input.GetKey(KeyCode.Q))
            rotationInput = 1;
        if (Input.GetKey(KeyCode.E))
            rotationInput = -1;

        // 按F键将旋转重置为水平状态
        if (Input.GetKey(KeyCode.F))
        {
            RotateToLevel();
        }
    }

    void FixedUpdate()
    {
        // 如果没有按F键，才响应正常旋转输入
        if (!Input.GetKey(KeyCode.F))
        {
            // 旋转飞船（围绕质心）
            rb.AddTorque(rotationInput * rotationSpeed * Time.fixedDeltaTime);
        }

        // 前进/后退（沿飞船朝向）
        Vector2 forwardDirection = transform.up;
        rb.AddForce(forwardDirection * forwardInput * forwardSpeed);

        // 左右平移（垂直于飞船朝向）
        Vector2 strafeDirection = transform.right;
        rb.AddForce(strafeDirection * strafeInput * strafeSpeed);
    }

    // 旋转至水平状态（Z轴为0）
    void RotateToLevel()
    {
        // 获取当前Z轴旋转角度（-180到180之间）
        float currentZ = transform.eulerAngles.z;
        currentZ = currentZ > 180 ? currentZ - 360 : currentZ;

        // 计算需要旋转的角度
        float angleDifference = -currentZ;

        // 限制旋转速度，使旋转平滑
        float rotateStep = levelRotationSpeed * Time.fixedDeltaTime;
        float newZ = Mathf.MoveTowards(currentZ, 0, rotateStep);

        // 应用新的旋转角度
        transform.eulerAngles = new Vector3(0, 0, newZ);

        // 重置旋转速度，避免惯性影响
        rb.angularVelocity = 0;
    }

    public float GetRelativeSpeed(Transform target)
    {
        // 计算相对于目标的速度
        Vector2 relativeVelocity = rb.velocity - target.GetComponent<Rigidbody2D>().velocity;
        return relativeVelocity.magnitude;
    }
}
