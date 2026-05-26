using UnityEngine;
using UnityEngine.SceneManagement;

public class DockingSystem : MonoBehaviour
{
    [Header("对接设置")]
    public Transform soyuzDockingPort; // 联盟号对接器
    public Transform salyutDockingPort; // 礼炮7号对接器
    public float maxAllowedSpeed = 1f; // 最大允许对接速度
    public float dockingDistanceThreshold = 0.5f; // 对接距离阈值
    public string repairSceneName = "RepairScene"; // 修理场景名称

    private SoyuzController soyuzController;
    private bool isDockingInProgress = false;

    void Start()
    {
        soyuzController = FindObjectOfType<SoyuzController>();
    }

    void Update()
    {
        CheckDockingStatus();
    }

    void CheckDockingStatus()
    {
        // 计算两个对接器之间的距离
        float distance = Vector2.Distance(soyuzDockingPort.position, salyutDockingPort.position);

        // 检测是否在对接范围内
        if (distance < dockingDistanceThreshold)
        {
            if (!isDockingInProgress)
            {
                isDockingInProgress = true;
                CheckDockingResult();
            }
        }
        else
        {
            isDockingInProgress = false;
        }
    }

    void CheckDockingResult()
    {
        // 计算相对速度
        float relativeSpeed = soyuzController.GetRelativeSpeed(salyutDockingPort.parent);

        // 检查对接是否成功
        if (relativeSpeed <= maxAllowedSpeed)
        {
            Debug.Log("对接成功！");
            DockingSuccess();
        }
        else
        {
            Debug.Log($"对接失败！相对速度 {relativeSpeed:F2} 超过阈值 {maxAllowedSpeed}");
            DockingFailed();
        }
    }

    void DockingSuccess()
    {
        // 对接成功，切换到修理场景
        SceneManager.LoadScene(repairSceneName);
    }

    void DockingFailed()
    {
        // 对接失败，重置场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
