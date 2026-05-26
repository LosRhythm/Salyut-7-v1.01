using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DockingUIManager : MonoBehaviour
{
    [Tooltip("显示对接状态的文本")]
    public TextMeshProUGUI statusText;

    [Tooltip("航天器的对接端口")]
    public DockingPort shipDockingPort;

    [Tooltip("对接条件提示文本")]
    public TextMeshProUGUI hintText;

    [Header("延迟设置")]
    [Tooltip("对接成功后延迟几秒切换场景")]
    [Range(1f, 10f)]
    public float delaySeconds = 3f; // 默认延迟3秒
    private void Start()
    {
        UpdateStatus("未对接");
        UpdateHint("靠近空间站对接端口进行对接");

        if (shipDockingPort != null)
        {
            shipDockingPort.OnDockingSuccess += OnDockingSuccess;
            shipDockingPort.OnUndock += OnUndock;
        }
    }

    private void Update()
    {
        if (shipDockingPort == null)
            return;

        // 按U键解除对接
        if (shipDockingPort.isDocked && Input.GetKeyDown(KeyCode.U))
        {
            shipDockingPort.Undock();
        }
        // 显示对接状态和提示
        else if (!shipDockingPort.isDocked)
        {
            if (shipDockingPort.isDetectingTarget)
            {
                UpdateStatus("对接中...");
                ShowDockingHints();
            }
            else
            {
                UpdateStatus("未对接");
                UpdateHint("靠近空间站对接端口进行对接");
            }
        }
    }

    // 显示对接条件提示
    private void ShowDockingHints()
    {
        if (shipDockingPort.targetDockingPort == null || hintText == null)
            return;

        // 计算距离提示
        float distance = Vector2.Distance(shipDockingPort.transform.position,
                                         shipDockingPort.targetDockingPort.transform.position);
        string distanceHint = distance <= shipDockingPort.positionTolerance
            ? "位置: 良好"
            : $"靠近: {distance:F1}m";

        // 计算角度提示
        float angle = Vector2.Angle(shipDockingPort.transform.up,
                                   shipDockingPort.targetDockingPort.transform.up);
        string angleHint = angle <= shipDockingPort.angleTolerance
            ? "角度: 良好"
            : $"调整角度: {angle:F1}°";

        // 计算重叠提示
        float overlap = shipDockingPort.CalculateOverlapRatio();
        string overlapHint = overlap >= shipDockingPort.requiredOverlapRatio
            ? "重叠: 足够"
            : $"增加重叠: {Mathf.Round(overlap * 100)}%";

        UpdateHint($"{distanceHint} | {angleHint} | {overlapHint}");
    }

    private void OnDockingSuccess()
    {
        UpdateStatus("对接成功！");
        UpdateHint("对接已完成");

        SoundManager.instance.Play("Docking");

        Invoke(nameof(LoadNextScene), delaySeconds);

       
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene("Scene2-3");
    }

    private void OnUndock()
    {
        UpdateStatus("已解除对接");
    }

    private void UpdateStatus(string message)
    {
        if (statusText != null)
            statusText.text = $"对接状态: {message}";
    }

    private void UpdateHint(string message)
    {
        if (hintText != null)
            hintText.text = $"提示: {message}";
    }

    private void OnDestroy()
    {
        if (shipDockingPort != null)
        {
            shipDockingPort.OnDockingSuccess -= OnDockingSuccess;
            shipDockingPort.OnUndock -= OnUndock;
        }
    }
}
