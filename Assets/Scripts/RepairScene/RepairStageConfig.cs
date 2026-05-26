using UnityEngine;
using UnityEngine.UI;

// 挂载在修理点UI预制体上，存储4个阶段的图片
[RequireComponent(typeof(Image))] // 强制要求有Image组件
public class RepairStageConfig : MonoBehaviour
{
    [Header("4个修理阶段的图片（按进度顺序）")]
    [Tooltip("索引0：0%~24%，索引1：25%~49%，索引2：50%~74%，索引3：75%~100%")]
    public Sprite[] stageSprites; // 需手动拖入4张阶段图

    private Image stageImage; // 显示阶段图的Image组件

    private void Awake()
    {
        // 获取自身的Image组件（用于显示阶段图）
        stageImage = GetComponent<Image>();

        // 初始化：默认显示第1阶段图（防止空引用）
        if (stageSprites != null && stageSprites.Length > 0)
        {
            stageImage.sprite = stageSprites[0];
        }
        else
        {
            Debug.LogError("请为RepairStageConfig赋值4个阶段的图片！", this);
        }
    }

    // 外部调用：根据进度百分比切换阶段图
    public void UpdateStageImage(float repairProgress)
    {
        // 进度安全校验（确保在0~1之间）
        repairProgress = Mathf.Clamp01(repairProgress);

        // 二次检查：数组是否为空或长度不足
        if (stageSprites == null)
        {
            Debug.LogError("stageSprites数组为空！请在预制体中赋值", this);
            return;
        }
        if (stageSprites.Length < 4)
        {
            Debug.LogError($"stageSprites数组长度为{stageSprites.Length}，需要4个元素！", this);
            return;
        }

        // 检查每个元素是否为空
        for (int i = 0; i < 4; i++)
        {
            if (stageSprites[i] == null)
            {
                Debug.LogError($"stageSprites[{i}]为空！请重新赋值第{i + 1}阶段图片", this);
                return;
            }
        }

        // 校验图片数组是否有效
        if (stageSprites == null || stageSprites.Length < 4)
        {
            Debug.LogWarning("阶段图片数组未配置完整（需4张图）", this);
            return;
        }

        Sprite targetSprite = null;
        // 记录当前阶段和对应的图片
        Debug.Log("进入判断阶段");
        // 根据进度判断当前阶段，切换图片
        if (repairProgress < 0.25f) // 0%~24%：阶段1
        {
            stageImage.sprite = stageSprites[0];
            Debug.Log($"进入阶段1（0%~24%），准备显示图片：{targetSprite?.name ?? "空图片"}", this);
        }
        else if (repairProgress < 0.5f) // 25%~49%：阶段2
        {
            stageImage.sprite = stageSprites[1];
            Debug.Log($"进入阶段2（25%~49%），准备显示图片：{targetSprite?.name ?? "空图片"}", this);
        }
        else if (repairProgress < 0.75f) // 50%~74%：阶段3
        {
            stageImage.sprite = stageSprites[2];
            Debug.Log($"进入阶段3（50%~74%），准备显示图片：{targetSprite?.name ?? "空图片"}", this);
        }
        else // 75%~100%：阶段4
        {
            stageImage.sprite = stageSprites[3];
            Debug.Log($"进入阶段4（75%~100%），准备显示图片：{targetSprite?.name ?? "空图片"}", this);
        }

        // 验证图片是否有效，再设置
        if (targetSprite != null)
        {
            stageImage.sprite = targetSprite;
            Debug.Log("图片设置成功！", this);
        }
        else
        {
            Debug.LogError("目标图片为空，无法设置！", this);
        }
    }
}