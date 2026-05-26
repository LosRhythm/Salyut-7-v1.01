using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RepairPoint : MonoBehaviour
{
    [Header("修理设置")]
    public float repairRange = 2f;
    public float repairTime = 5f;
    public bool isRepaired = false;

    [Header("独立UI设置（每个修理点单独配置）")]
    [Tooltip("此修理点专用的UI面板（在场景中创建并关联）")]
    public GameObject repairUI; // 拖入场景中为该修理点创建的UI
    public Image stageImage; // 拖入UI中的阶段图片组件
    public TMP_Text promptText; // 拖入UI中的提示文本
    public Sprite[] stageSprites; // 拖入4个阶段的图片（0-3对应四个阶段）
    public GameObject brokenHole;   //破洞贴图

    [Header("调试")]
    public bool drawGizmos = true;

    private PlayerController astronaut;
    private bool isInRange = false;
    private float repairProgress = 0f;
    private bool isRepairing = false;

    private void Start()
    {
        // 查找宇航员
        astronaut = FindAnyObjectByType<PlayerController>();
        if (astronaut == null)
        {
            Debug.LogWarning("场景中未找到Astronaut组件", this);
        }

        if (brokenHole != null)
        {
            brokenHole.SetActive(true);
        }

        // 初始化UI状态
        InitializeUI();
    }

    private void Update()
    {
        if (isRepaired)
        {
            HideUI();
            return;
        }

        // 检查UI引用是否完整
        if (!CheckUIReferences())
        {
            return;
        }

        // 检查与宇航员的距离
        CheckDistanceToAstronaut();

        // 处理修理逻辑
        if (isInRange)
        {
            ShowUI();
            HandleRepair();
        }
        else
        {
            HideUI();
            repairProgress = 0;
            isRepairing = false;
        }
    }

    // 初始化UI
    private void InitializeUI()
    {
        // 确保UI初始隐藏
        if (repairUI != null)
        {
            repairUI.SetActive(false);
        }

        // 初始化阶段图片
        if (stageImage != null && stageSprites != null && stageSprites.Length >= 4)
        {
            stageImage.sprite = stageSprites[0];
        }

        // 初始化提示文本
        if (promptText != null)
        {
            promptText.text = "按住X键进行修理";
        }
    }

    // 检查UI引用是否完整
    private bool CheckUIReferences()
    {
        if (repairUI == null)
        {
            Debug.LogError($"{gameObject.name} 未配置repairUI！", this);
            return false;
        }

        if (stageImage == null)
        {
            Debug.LogError($"{gameObject.name} 未配置stageImage！", this);
            return false;
        }

        if (promptText == null)
        {
            Debug.LogError($"{gameObject.name} 未配置promptText！", this);
            return false;
        }

        if (stageSprites == null || stageSprites.Length < 4)
        {
            Debug.LogError($"{gameObject.name} 未正确配置stageSprites（需要4张图片）！", this);
            return false;
        }

        return true;
    }

    // 检查与宇航员的距离
    private void CheckDistanceToAstronaut()
    {
        if (astronaut == null) return;

        float distance = Vector3.Distance(transform.position, astronaut.transform.position);
        isInRange = distance <= repairRange;
    }

    // 处理修理逻辑
    private void HandleRepair()
    {
        if (Input.GetKey(KeyCode.X))
        {
            SoundManager.instance.Play("Fixing");

            isRepairing = true;
            repairProgress += Time.deltaTime;
            float progress = Mathf.Clamp01(repairProgress / repairTime);

            // 更新阶段图片
            UpdateStageImage(progress);

            // 更新提示文本
            promptText.text = $"正在修理... 按住X键 ({Mathf.Round(progress * 100)}%)";

            // 检查是否修理完成
            if (repairProgress >= repairTime)
            {
                RepairComplete();
            }
        }
        else
        {
            isRepairing = false;
            promptText.text = "按住X键进行修理";
        }
    }

    // 更新阶段图片
    private void UpdateStageImage(float progress)
    {
        if (stageImage == null || stageSprites == null || stageSprites.Length < 4)
            return;

        Sprite targetSprite;

        if (progress < 0.25f)
        {
            targetSprite = stageSprites[0]; // 阶段1 (0-24%)
        }
        else if (progress < 0.5f)
        {
            targetSprite = stageSprites[1]; // 阶段2 (25-49%)
        }
        else if (progress < 0.75f)
        {
            targetSprite = stageSprites[2]; // 阶段3 (50-74%)
        }
        else
        {
            targetSprite = stageSprites[3]; // 阶段4 (75-100%)
        }

        // 确保目标图片有效
        if (targetSprite != null)
        {
            stageImage.sprite = targetSprite;
        }
        else
        {
            Debug.LogError($"{gameObject.name} 的阶段图片为空！", this);
        }
    }

    // 修理完成
    private void RepairComplete()
    {
        isRepaired = true;

        brokenHole.SetActive(false);
        HideUI();

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowSuccessMessage("修理完成！");
        }
    }

    // 显示UI
    private void ShowUI()
    {
        if (repairUI != null && !repairUI.activeSelf)
        {
            repairUI.SetActive(true);
        }
    }

    // 隐藏UI
    private void HideUI()
    {
        if (repairUI != null && repairUI.activeSelf)
        {
            repairUI.SetActive(false);
        }
    }

    // 绘制Gizmos
    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;

        // 绘制修理范围
        Gizmos.color = isInRange ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, repairRange);

        // 绘制修理点位置
        Gizmos.color = isRepaired ? Color.blue : Color.red;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }

    // 供跟踪器使用的属性
    public bool IsRepaired => isRepaired;
}
