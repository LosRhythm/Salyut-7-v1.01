using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RepairPointCount : MonoBehaviour
{
    // 公开的数量属性，可在UI中绑定显示
    public int repairPointCount { get; private set; }

    public TextMeshProUGUI count;

    private int _previousCount;
    private string unit = "个";

    // Update is called once per frame
    void Update()
    {
        // 每帧获取所有标签为"RepairPoint"的物体
        GameObject[] repairPoints = GameObject.FindGameObjectsWithTag("RepairPointUI");

        // 更新当前数量
        repairPointCount = repairPoints.Length;

        // 当数量发生变化时在控制台输出
        if (repairPointCount != _previousCount)
        {
            Debug.Log($"RepairPoint数量变化: {_previousCount} -> {repairPointCount}");
            _previousCount = repairPointCount;


            count.text = repairPointCount.ToString()+unit;

            // 可以在这里添加数量变化时的其他逻辑
            // 例如：OnRepairPointCountChanged(repairPointCount);
        }
    }
    
        // 可选：获取当前数量的方法
    public int GetCurrentRepairPointCount()
    {
        return repairPointCount;
    }
}
