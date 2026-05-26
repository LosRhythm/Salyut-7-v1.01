using UnityEngine;
using TMPro;

// 基于距离检测的倒计时系统：通过计算距离判断是否在安全区
public class CountdownSystem : MonoBehaviour
{
    [Header("倒计时基础设置")]
    [Tooltip("初始倒计时时间（秒）")]
    public float initialTime = 60f;
    [Tooltip("倒计时文本（拖入UI的TextMeshPro）")]
    public TextMeshProUGUI countdownText;

    [Header("安全区设置")]
    [Tooltip("安全区中心点（空对象标记位置）")]
    public Transform safeZoneCenter; // 安全区中心点
    [Tooltip("安全区半径（单位：米）")]
    public float safeZoneRadius = 5f; // 安全区范围半径
    [Tooltip("玩家角色")]
    public Transform player; // 宇航员

    private bool isInSafeZone; // 是否在安全区内
    private float currentTime; // 当前剩余时间


    private void Start()
    {
        currentTime = initialTime;
        CheckReferences();
        UpdateCountdownText();
    }


    private void Update()
    {
        // 检查必要引用是否存在
        if (countdownText == null || safeZoneCenter == null || player == null)
            return;

        // 更新倒计时
        currentTime -= Time.deltaTime;
        currentTime = Mathf.Max(currentTime, 0); // 防止出现负数
        UpdateCountdownText();

        // 检测是否在安全区内
        CheckSafeZoneDistance();
    }


    // 检查与安全区的距离
    private void CheckSafeZoneDistance()
    {
        // 计算宇航员到安全区中心的距离
        float distance = Vector2.Distance(
            new Vector2(player.position.x, player.position.y),
            new Vector2(safeZoneCenter.position.x, safeZoneCenter.position.y)
        );

        // 判断是否在安全区内（距离 <= 半径）
        bool wasInSafeZone = isInSafeZone;
        isInSafeZone = distance <= safeZoneRadius;

        // 刚进入安全区时重置倒计时
        if (isInSafeZone && !wasInSafeZone)
        {
            ResetCountdown();
        }
    }


    // 重置倒计时
    private void ResetCountdown()
    {
        currentTime = initialTime;
        Debug.Log($"进入安全区，倒计时已重置（当前时间：{initialTime}秒）");
    }


    // 更新倒计时文本显示
    private void UpdateCountdownText()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        countdownText.text = $"{minutes:00}:{seconds:00}";
    }


    // 检查必要引用
    private void CheckReferences()
    {
        if (countdownText == null)
            Debug.LogError("请赋值倒计时文本组件（countdownText）", this);

        if (safeZoneCenter == null)
            Debug.LogError("请设置安全区中心点（safeZoneCenter）", this);

        if (player == null)
            Debug.LogError("请赋值玩家角色（player）", this);
    }


    // 可视化安全区范围（Scene视图）
    private void OnDrawGizmosSelected()
    {
        if (safeZoneCenter == null) return;

        // 绘制安全区范围（半透明绿色球体）
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawSphere(safeZoneCenter.position, safeZoneRadius);

        // 绘制安全区中心点（黄色小球）
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(safeZoneCenter.position, 0.3f);

        // 如果已指定玩家，绘制玩家到安全区的距离线
        if (player != null)
        {
            Gizmos.color = isInSafeZone ? Color.green : Color.red;
            Gizmos.DrawLine(player.position, safeZoneCenter.position);
        }
    }
}
