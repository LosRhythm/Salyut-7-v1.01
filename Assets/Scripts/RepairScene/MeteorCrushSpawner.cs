using UnityEngine;

public class MeteorCrushSpawner : MonoBehaviour
{
    [Header("陨石设置")]
    [Tooltip("陨石预制体")]
    public GameObject meteorPrefab;

    [Header("生成范围设置")]
    public Vector2 spawnXRange = new Vector2(0, 2f);
    public Vector2 spawnYRange = new Vector2(-5f, 5f);

    [Header("生成速率设置")]
    public float initialSpawnRate = 2f;
    public float minSpawnRate = 0.5f;
    public float spawnRateDecrease = 0.05f;
    public bool enableDifficultyIncrease = true;

    [Header("方向设置")]
    public Vector2 baseDirection = new Vector2(-1, 0);
    [Range(0, 90)]
    public float directionVariance = 30f;

    private float currentSpawnRate;
    private float spawnTimer;

    private void Start()
    {
        currentSpawnRate = initialSpawnRate;
        spawnTimer = initialSpawnRate;

        if (meteorPrefab == null)
        {
            Debug.LogError("请设置陨石预制体！", this);
            enabled = false;
        }
    }

    private void Update()
    {
        if (IsGameOver() || meteorPrefab == null)
            return;

        spawnTimer += Time.deltaTime;
        
        if (spawnTimer >= currentSpawnRate)
        {
            SpawnMeteor();
            spawnTimer = 0f;

            if (enableDifficultyIncrease)
            {
                currentSpawnRate = Mathf.Max(
                    currentSpawnRate - spawnRateDecrease * Time.deltaTime,
                    minSpawnRate
                );
            }
        }
    }

    private void SpawnMeteor()
    {
        Vector2 spawnPosition = new Vector2(
            transform.position.x + Random.Range(spawnXRange.x, spawnXRange.y),
            transform.position.y + Random.Range(spawnYRange.x, spawnYRange.y)
        );

        GameObject newMeteor = Instantiate(meteorPrefab, spawnPosition, Quaternion.identity);
        newMeteor.transform.parent = transform;
        MeteorCrush _neweMeteor= newMeteor.GetComponent<MeteorCrush>();
        _neweMeteor.SetMoveDirection(GetRandomDirection()); // 现在可以正常调用此方法
    }

    private Vector2 GetRandomDirection()
    {
        float angle = directionVariance * Mathf.Deg2Rad;
        float randomAngle = Random.Range(-angle, angle);

        float newX = baseDirection.x * Mathf.Cos(randomAngle) - baseDirection.y * Mathf.Sin(randomAngle);
        float newY = baseDirection.x * Mathf.Sin(randomAngle) + baseDirection.y * Mathf.Cos(randomAngle);

        return new Vector2(newX, newY);
    }

    private bool IsGameOver()
    {
        return AstronautManager.Instance != null && AstronautManager.Instance.IsGameOver;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Vector3 center = new Vector3(
            transform.position.x + (spawnXRange.x + spawnXRange.y) / 2,
            transform.position.y + (spawnYRange.x + spawnYRange.y) / 2,
            transform.position.z
        );
        Vector3 size = new Vector3(
            spawnXRange.y - spawnXRange.x,
            spawnYRange.y - spawnYRange.x,
            0.1f
        );
        Gizmos.DrawCube(center, size);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, (Vector3)baseDirection * 3f);
    }
}
