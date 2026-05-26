using UnityEngine;

public class MeteorStatic2D : MonoBehaviour
{
    public float moveSpeed = 0.5f; // 移动速度
    private Vector2 moveDirection; // 直线移动方向（2D）

    [Header("旋转设置")]
    public float minRotationSpeed = 30f; // 最小旋转速度（度/秒）
    public float maxRotationSpeed = 120f; // 最大旋转速度（度/秒）
    private float rotationSpeed; // 实际旋转速度

    public float lifeTime = 10f; // 自动销毁时间
    public GameObject explosionEffect; // 爆炸特效（可选）

    // 初始化移动方向和旋转（只在生成时计算一次）
    public void SetDirection(Transform target)
    {
        // 设置移动方向（2D）
        if (target != null)
        {
            Vector2 targetPos = new Vector2(target.position.x, target.position.y);
            Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
            moveDirection = (targetPos - currentPos).normalized;
        }
        else
        {
            // 如果找不到目标，默认向下移动
            moveDirection = Vector2.down;
        }

        // 初始化随机旋转速度（只绕Z轴旋转，适合2D）
        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
        // 50%概率反转旋转方向
        if (Random.value < 0.5f)
        {
            rotationSpeed = -rotationSpeed;
        }
    }

    private void Update()
    {
        // 沿初始方向直线移动（2D）
        transform.Translate(new Vector3(moveDirection.x, moveDirection.y, 0) * moveSpeed * Time.deltaTime, Space.World);

        // 绕Z轴旋转（适合2D贴图）
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // 自动销毁
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    // 2D碰撞检测
    private void OnTriggerEnter(Collider other)
    {
        // 碰到玩家/航天器
        if (other.CompareTag("Player"))
        {
            // 查找生命管理器并减少生命值
            HealthManager healthManager = FindObjectOfType<HealthManager>();
            if (healthManager != null)
            {
                Debug.Log("损失生命");
                healthManager.LoseLife();
            }

            // 生成爆炸特效
            if (explosionEffect != null)
            {
                Instantiate(explosionEffect, transform.position, Quaternion.identity);
            }

            //Destroy(gameObject);
        }
    }
}
