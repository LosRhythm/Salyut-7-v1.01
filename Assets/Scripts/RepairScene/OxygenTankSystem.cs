using UnityEngine;
using UnityEngine.UI;

public class OxygenTankSystem : MonoBehaviour
{
    [Header("氧气状态设置")]
    [Tooltip("氧气状态图片（5个状态，从满到空排序）")]
    public Sprite[] oxygenStateSprites; // 索引0:满 索引1:80% 索引2:60% 索引3:40% 索引4:空
    [Tooltip("氧气消耗时间（秒）- 从满到空的总时间")]
    public float totalOxygenTime = 60f;

    [Header("引用设置")]
    [Tooltip("显示氧气状态的图片组件")]
    public Image oxygenDisplayImage;
    [Tooltip("安全区中心点")]
    public Transform safeZoneCenter;
    [Tooltip("安全区半径")]
    public float safeZoneRadius = 5f;
    [Tooltip("宇航员角色")]
    public Transform astronaut;

    [Header("事件设置")]
    [Tooltip("氧气耗尽时触发（如角色死亡）")]
    public UnityEngine.Events.UnityEvent onOxygenDepleted;

    private float currentOxygen; // 当前氧气值（0-1范围）
    private int currentStateIndex = 0; // 当前状态索引
    private bool isInSafeZone = false;
    private float oxygenDepletionRate; // 氧气消耗速率


    private void Start()
    {
        // 初始化氧气值为满
        currentOxygen = 1f;
        // 计算每秒氧气消耗速率
        oxygenDepletionRate = 1f / totalOxygenTime;

        // 检查必要引用
        CheckReferences();
        // 初始显示满状态
        UpdateOxygenDisplay();
    }


    private void Update()
    {
        // 引用不全则不执行逻辑
        if (oxygenDisplayImage == null || safeZoneCenter == null || astronaut == null)
            return;

        // 检测是否在安全区
        CheckSafeZoneStatus();

        if (!isInSafeZone)
        {
            // 不在安全区则消耗氧气
            ConsumeOxygen();
        }
        else
        {
            // 在安全区则补充氧气到满
            if (currentOxygen < 1f)
            {
                currentOxygen = Mathf.Min(currentOxygen + (oxygenDepletionRate * 2), 1f);
                UpdateOxygenDisplay();
            }
        }
    }


    // 消耗氧气
    private void ConsumeOxygen()
    {
        currentOxygen -= oxygenDepletionRate * Time.deltaTime;
        currentOxygen = Mathf.Max(currentOxygen, 0f);

        // 更新显示
        UpdateOxygenDisplay();

        // 氧气耗尽触发事件
        if (currentOxygen <= 0f)
        {
            onOxygenDepleted?.Invoke();
            enabled = false; // 停止脚本
        }
    }


    // 更新氧气显示状态
    private void UpdateOxygenDisplay()
    {
        if (oxygenStateSprites == null || oxygenStateSprites.Length != 5)
        {
            Debug.LogError("请设置5个氧气状态图片！", this);
            return;
        }

        // 根据当前氧气值计算状态索引
        int newStateIndex = Mathf.Clamp(4 - Mathf.FloorToInt(currentOxygen * 5), 0, 4);

        // 状态变化时才更新图片
        if (newStateIndex != currentStateIndex)
        {
            currentStateIndex = newStateIndex;
            oxygenDisplayImage.sprite = oxygenStateSprites[currentStateIndex];
            Debug.Log($"氧气状态变更: {currentStateIndex + 1}/5");
        }
    }


    // 检查是否在安全区
    private void CheckSafeZoneStatus()
    {
        float distance = Vector2.Distance(
            new Vector2(astronaut.position.x, astronaut.position.y),
            new Vector2(safeZoneCenter.position.x, safeZoneCenter.position.y)
        );

        isInSafeZone = distance <= safeZoneRadius;
    }


    // 检查引用
    private void CheckReferences()
    {
        if (oxygenDisplayImage == null)
            Debug.LogError("请赋值氧气显示图片组件！", this);

        if (oxygenStateSprites == null || oxygenStateSprites.Length != 5)
            Debug.LogError("请设置包含5个元素的氧气状态图片数组！", this);

        if (safeZoneCenter == null)
            Debug.LogError("请设置安全区中心点！", this);

        if (astronaut == null)
            Debug.LogError("请设置宇航员角色！", this);
    }


    // 可视化安全区
    private void OnDrawGizmosSelected()
    {
        if (safeZoneCenter == null) return;

        // 绘制安全区范围
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawSphere(safeZoneCenter.position, safeZoneRadius);

        // 绘制安全区中心点
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(safeZoneCenter.position, 0.3f);

        // 绘制宇航员到安全区的距离线
        if (astronaut != null)
        {
            Gizmos.color = isInSafeZone ? Color.green : Color.red;
            Gizmos.DrawLine(astronaut.position, safeZoneCenter.position);
        }
    }
}
