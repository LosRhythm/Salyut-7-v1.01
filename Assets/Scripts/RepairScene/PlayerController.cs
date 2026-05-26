
using UnityEngine;

/// <summary>
/// 玩家控制器：处理玩家移动、旋转、动画及输入响应
/// 自动要求挂载Rigidbody2D、Animator和SpriteRenderer组件
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    [Header("移动参数")]
    [Tooltip("推进力大小（控制移动加速度）")]
    public float thrustForce = 5f;
    [Tooltip("最大移动速度限制（已废弃）")]
    public float maxSpeed = 10f;
    [Tooltip("旋转时的扭矩大小")]
    public float rotationSpeed = 10f;
    [Tooltip("最大旋转角度限制")]
    public float maxRotation = 10f;
    [Tooltip("移动阻尼系数（值越大减速越快）")]
    public float drag = 0.8f;

    [Header("动画参数")]
    [Tooltip("移动动画播放速度倍率")]
    public float moveAnimationSpeed = 1f;

    private Rigidbody2D rb;           // 2D刚体组件，用于物理移动
    private Animator anim;           // 动画组件，控制动画状态
    private SpriteRenderer spriteRenderer;  // 精灵渲染器，用于翻转显示
    private Vector2 movementInput;   // 存储移动输入（X:水平方向，Y:垂直方向）
    private float rotationInput;     // 存储旋转输入（1:逆时针，-1:顺时针，0:无旋转）

    void Start()
    {
        // 获取组件引用
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 初始化物理参数
        rb.gravityScale = 0;          // 禁用重力影响
        rb.drag = drag;               // 设置移动阻尼
        rb.angularDrag = 0.8f;        // 设置旋转阻尼
    }

    void Update()
    {
        // 读取玩家输入
        GetInput();

        // 处理水平方向移动时的精灵翻转
        HandleADFlip();

        // 更新动画状态
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        // 应用移动力（物理帧更新）
        ApplyMovement();

        // 应用旋转扭矩（物理帧更新）
        ApplyRotation();

        // 限制最大移动速度
        LimitSpeed();
    }

    /// <summary>
    /// 读取玩家输入：
    /// - 水平方向（A/D键）控制左右移动
    /// - 垂直方向（W/S键）控制上下移动
    /// - 旋转（Q/E键）控制顺时针/逆时针旋转
    /// </summary>
    private void GetInput()
    {
        // 水平输入：D键为+1，A键为-1
        movementInput.x = Input.GetAxisRaw("Horizontal");

        // 垂直输入：W键为+1，S键为-1
        movementInput.y = Input.GetAxisRaw("Vertical");

        // 旋转输入：Q键逆时针旋转(+1)，E键顺时针旋转(-1)
        rotationInput = 0;
        if (Input.GetKey(KeyCode.Q))
        {
            rotationInput = 1f;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            rotationInput = -1f;
        }
    }

    /// <summary>
    /// 应用移动力到刚体
    /// 当输入向量长度大于0.1时（避免微小输入），规范化向量后添加推进力
    /// </summary>
    private void ApplyMovement()
    {
        if (movementInput.sqrMagnitude > 0.1f)
        {
            movementInput.Normalize();  // 标准化输入向量，确保斜向移动速度与轴向一致
            rb.AddForce(movementInput * thrustForce);
        }
    }

    /// <summary>
    /// 应用旋转扭矩到刚体
    /// 仅在有旋转输入时添加扭矩
    /// </summary>
    private void ApplyRotation()
    {
        if (rotationInput != 0)
        {
            rb.AddTorque(rotationInput * rotationSpeed * Time.fixedDeltaTime);
        }
    }

    /// <summary>
    /// 限制刚体的最大移动速度
    /// 当当前速度超过maxSpeed时，将速度规范化后乘以maxSpeed
    /// </summary>
    private void LimitSpeed()
    {
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    /// <summary>
    /// 处理A/D键输入时的精灵翻转
    /// 向右移动（D键）时不翻转，向左移动（A键）时翻转精灵
    /// </summary>
    private void HandleADFlip()
    {
        if (Mathf.Abs(movementInput.x) > 0.1f)
        {
            spriteRenderer.flipX = movementInput.x > 0;
        }
    }

    /// <summary>
    /// 更新动画状态
    /// 根据当前速度与最大速度的比值设置动画参数"Speed"
    /// </summary>
    private void UpdateAnimation()
    {
        float speedMagnitude = rb.velocity.magnitude;
        anim.SetFloat("Speed", speedMagnitude / maxSpeed);
    }

    /// <summary>
    /// 场景视图绘制调试 gizmo
    /// 以蓝色射线显示当前移动输入方向
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, movementInput * 0.5f);
    }
}
