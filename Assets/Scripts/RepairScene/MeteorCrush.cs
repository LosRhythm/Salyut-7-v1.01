using UnityEngine;

public class MeteorCrush : MonoBehaviour
{
    [Header("移动与旋转")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 90f;
    public Vector2 moveDirection = new Vector2(-1, 0); // 默认向左

    [Header("碰撞检测（距离版）")]
    public float collisionRadius = 0.5f;
    private Transform astronaut;
    private bool hasCollided = false;

    [Header("存活设置")]
    public float maxLifetime = 10f;
    public float destroyXBoundary = -20f;
    private float lifetimeTimer = 0f;

    private void Start()
    {
        FindAstronaut();
        Debug.Log($"陨石生成！位置：{transform.position}，初始存活时间：{maxLifetime}秒", this);
        rotationSpeed = Random.value < 0.5f ? -rotationSpeed : rotationSpeed;
    }

    // 新增：设置移动方向的方法（供生成器调用）
    public void SetMoveDirection(Vector2 direction)
    {
        moveDirection = direction.normalized; // 标准化方向向量，确保速度一致
    }

    private void FindAstronaut()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            astronaut = player.transform;
            Debug.Log($"陨石找到Player标签的宇航员：{player.name}", this);
            return;
        }

        AstronautManager manager = FindObjectOfType<AstronautManager>();
        if (manager != null && manager.astronaut != null)
        {
            astronaut = manager.astronaut.transform;
            Debug.Log($"陨石通过AstronautManager找到宇航员：{astronaut.name}", this);
            return;
        }

        Debug.LogWarning("陨石未找到宇航员！请确保场景中有Tag为Player的物体", this);
    }

    private void Update()
    {
        lifetimeTimer += Time.deltaTime;
        if (lifetimeTimer >= maxLifetime)
        {
            Debug.Log($"陨石因存活时间到销毁！存活了{lifetimeTimer:F2}秒", this);
            Destroy(gameObject);
            return;
        }

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        /*if (transform.position.x < destroyXBoundary)
        {
            Debug.Log($"陨石因超出X边界销毁！当前X：{transform.position.x}，边界：{destroyXBoundary}", this);
            Destroy(gameObject);
            return;
        }*/

        if (!hasCollided && astronaut != null)
        {
            CheckDistanceCollision();
        }
    }

    private void CheckDistanceCollision()
    {
        float distance = Vector2.Distance(
            new Vector2(transform.position.x, transform.position.y),
            new Vector2(astronaut.position.x, astronaut.position.y)
        );

        if (distance <= collisionRadius)
        {
            hasCollided = true;
            Debug.Log("陨石击中宇航员！", this);
            if (AstronautManager.Instance != null)
            {
                AstronautManager.Instance.OnAstronautHit();
            }
            //Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, collisionRadius);

        Gizmos.color = Color.yellow;
        Vector3 maxMoveDistance = (Vector3)moveDirection * moveSpeed * maxLifetime;
        Gizmos.DrawLine(transform.position, transform.position + maxMoveDistance);
    }
}
