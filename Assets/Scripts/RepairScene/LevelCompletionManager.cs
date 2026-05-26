using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelCompletionManager : MonoBehaviour
{
    [Header("通关设置")]
    [Tooltip("修理点的Tag名称（需与场景中修理点的Tag一致）")]
    public string repairPointTag = "RepairPoint";

    [Tooltip("通关后显示的提示文本")]
    public TMP_Text completionText;

    [Tooltip("通关延迟时间（秒），用于显示提示后再执行通关逻辑")]
    public float completionDelay = 2f;

    [Header("调试")]
    [Tooltip("是否在控制台输出调试信息")]
    public bool debugMode = true;

    private bool isLevelCompleted = false;

    private void Start()
    {
        // 初始化通关提示文本（隐藏）
        if (completionText != null)
        {
            completionText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("未设置通关提示文本组件，请在Inspector中赋值", this);
        }
    }

    private void Update()
    {
        // 如果已经通关，则不再检测
        if (isLevelCompleted) return;

        // 检测所有修理点是否都已修复
        if (AreAllRepairPointsCompleted())
        {
            CompleteLevel();
        }
    }

    // 检测所有带指定Tag的修理点是否都已修复
    private bool AreAllRepairPointsCompleted()
    {
        // 查找场景中所有带指定Tag的修理点
        GameObject[] repairPoints = GameObject.FindGameObjectsWithTag(repairPointTag);

        // 如果没有找到修理点，根据需求处理（这里视为已完成）
        if (repairPoints.Length == 0)
        {
            if (debugMode)
                Debug.LogWarning($"未找到Tag为{repairPointTag}的修理点，默认视为已完成", this);
            return true;
        }

        // 检查每个修理点是否已修复
        foreach (GameObject rp in repairPoints)
        {
            RepairPoint repairPoint = rp.GetComponent<RepairPoint>();

            // 如果找不到RepairPoint组件，或未修复，则返回false
            if (repairPoint == null)
            {
                if (debugMode)
                    Debug.LogWarning($"Tag为{repairPointTag}的对象{rp.name}上未挂载RepairPoint组件", this);
                return false;
            }

            if (!repairPoint.IsRepaired)
            {
                // 找到未修复的修理点，返回false
                if (debugMode)
                    //Debug.Log($"发现未修复的修理点：{rp.name}", this);
                return false;
            }
        }

        // 所有修理点都已修复
        return true;
    }

    // 通关处理
    private void CompleteLevel()
    {
        isLevelCompleted = true;

        if (debugMode)
            Debug.Log("所有修理点已修复，准备通关！", this);

        // 显示通关提示
        if (completionText != null)
        {
            completionText.gameObject.SetActive(true);
            completionText.text = "所有修理点已修复，通关成功！";
            SceneManager.LoadScene("Ending");
        }

        // 延迟执行通关逻辑（如加载下一关、返回菜单等）
        Invoke(nameof(ExecuteCompletionLogic), completionDelay);
    }

    // 实际的通关逻辑（可根据项目需求修改）
    private void ExecuteCompletionLogic()
    {
        // 示例：加载下一关（需在Build Settings中设置场景索引）
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        // 示例：返回主菜单（需替换为实际的菜单场景名称）
        // SceneManager.LoadScene("MainMenu");

        // 示例：暂停游戏（仅用于演示）
        Time.timeScale = 0;

        if (debugMode)
            Debug.Log("已执行通关逻辑", this);
    }
}
