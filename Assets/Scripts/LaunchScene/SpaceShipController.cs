using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SpaceshipController : MonoBehaviour
{
    [Header("移动设置")]
    [Tooltip("飞船移动速度")]
    public float moveSpeed = 10f;

    private Rigidbody rb;

    private void Awake()
    {
        // 获取刚体组件
        rb = GetComponent<Rigidbody>();

        // 冻结旋转，防止物理效果导致飞船倾斜
        rb.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        // 获取输入
        float zInput = Input.GetAxis("Horizontal"); // A/D键控制Z轴
        float xInput = Input.GetAxis("Vertical");   // W/S键控制X轴

        // 计算移动方向
        Vector3 movement = new Vector3(xInput, 0f, zInput) * moveSpeed * Time.fixedDeltaTime;

        // 应用移动
        rb.MovePosition(rb.position + movement);
    }
}
