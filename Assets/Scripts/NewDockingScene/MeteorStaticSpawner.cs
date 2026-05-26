using UnityEngine;

public class MeteorStaticSpawner : MonoBehaviour
{
    public MeteorStatic2D meteorPrefab; // 陨石预制体（使用MeteorStatic2D类型）
    public float spawnInterval = 1f; // 生成间隔
    public Transform target; // 目标航天器（拖拽赋值）

    [Header("生成范围设置")]
    public float minDistance = 8f; // 距目标最小距离
    public float maxDistance = 20f; // 距目标最大距离
    public float spawnRangeAngle = 360f; // 生成角度范围（360为全方向）

    [Header("陨石属性范围")]
    public float minMoveSpeed = 0.1f;
    public float maxMoveSpeed = 0.5f;
    public float minLifeTime = 8f;
    public float maxLifeTime = 15f;

    private float spawnTimer;

    private void Update()
    {
        // 计时生成陨石
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            SpawnMeteor();
            spawnTimer = 0;
        }
    }

    private void SpawnMeteor()
    {
        if (meteorPrefab == null || target == null) return;

        // 在目标周围随机生成位置
        Vector2 randomPos = GetRandomPositionAroundTarget();

        // 生成陨石
        MeteorStatic2D meteor = Instantiate(meteorPrefab, randomPos, Quaternion.identity);

        // 设置陨石属性
        meteor.moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
        meteor.lifeTime = Random.Range(minLifeTime, maxLifeTime);

        // 设置移动方向（指向目标）
        meteor.SetDirection(target);
    }

    // 计算目标周围的随机位置
    private Vector2 GetRandomPositionAroundTarget()
    {
        // 随机角度（0到指定角度范围）
        float angle = Random.Range(0, spawnRangeAngle) * Mathf.Deg2Rad;
        // 随机距离（在最小和最大之间）
        float distance = Random.Range(minDistance, maxDistance);

        // 计算位置（极坐标转直角坐标）
        float x = target.position.x + Mathf.Cos(angle) * distance;
        float y = target.position.y + Mathf.Sin(angle) * distance;

        return new Vector2(x, y);
    }

    // 绘制生成范围（编辑器辅助）
    private void OnDrawGizmos()
    {
        if (target != null)
        {
            Gizmos.color = Color.yellow;
            // 绘制最大范围
            DrawCircle(target.position, maxDistance);
            // 绘制最小范围
            Gizmos.color = Color.green;
            DrawCircle(target.position, minDistance);
        }
    }

    // 绘制2D圆形辅助线
    private void DrawCircle(Vector3 center, float radius)
    {
        int segments = 32;
        Vector3[] points = new Vector3[segments + 1];

        for (int i = 0; i <= segments; i++)
        {
            float angle = (i * 2f * Mathf.PI) / segments;
            points[i] = new Vector3(
                center.x + Mathf.Cos(angle) * radius,
                center.y + Mathf.Sin(angle) * radius,
                center.z
            );
        }

        for (int i = 0; i < segments; i++)
        {
            Gizmos.DrawLine(points[i], points[i + 1]);
        }
    }
}
