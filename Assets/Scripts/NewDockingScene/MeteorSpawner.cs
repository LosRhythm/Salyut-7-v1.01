using UnityEngine;

public class MeteorSpawner : MonoBehaviour
{
    public GameObject meteorPrefab; // 陨石预制体
    public float spawnInterval = 0.5f; // 生成间隔
    public float spawnRange = 10f; // 生成范围
    public Transform player; // 玩家引用（拖拽赋值）


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
        if (meteorPrefab == null) return;

        // 随机生成位置（在生成器周围）
        Vector3 randomPos = new Vector3(
            transform.position.x + Random.Range(-spawnRange, spawnRange),
            transform.position.y,
            transform.position.z + Random.Range(-spawnRange, spawnRange)
        );

        // 生成陨石
        GameObject meteor = Instantiate(meteorPrefab, randomPos, Quaternion.identity);

        // 获取陨石组件并设置移动方向（指向玩家）
        Meteor2D meteorScript = meteor.GetComponent<Meteor2D>();
        if (meteorScript != null)
        {
            meteorScript.SetDirection(player); // 只在生成时设置一次方向
        }
    }

    // 绘制生成范围（编辑器辅助）
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, spawnRange);
    //}
}
