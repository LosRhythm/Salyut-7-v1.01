using UnityEngine;

public class Meteor2D : MonoBehaviour
{
    public float moveSpeed = 5f; // 移动速度
    private Vector3 moveDirection; // 直线移动方向
    public float lifeTime = 10f; // 自动销毁时间
    public GameObject explosionEffect; // 爆炸特效（可选）

    // 初始化移动方向（只在生成时计算一次）
    public void SetDirection(Transform target)
    {
        if (target != null)
        {
            // 计算从陨石到玩家的方向（只执行一次）
            moveDirection = (target.position - transform.position).normalized;
        }
        else
        {
            // 如果找不到玩家，默认向下移动
            moveDirection = Vector3.down;
        }
    }

    private void Update()
    {
        // 沿初始方向直线移动
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

        // 自动销毁
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    // 碰撞检测
    private void OnTriggerEnter(Collider other)
    {
        // 碰到玩家
        if (other.CompareTag("Player"))
        {
            // 查找生命管理器并减少生命值
            HealthManager healthManager = FindObjectOfType<HealthManager>();
            if (healthManager != null)
            {
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
