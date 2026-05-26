using UnityEngine;

[RequireComponent(typeof(Collider))] // 改为3D碰撞器
public class DockingPort : MonoBehaviour
{
    [Header("基础设置")]
    public bool isStationPort; // 标记是否是空间站的对接端口
    public Transform parentVessel; // 对接端口所属的航天器/空间站根节点

    [Header("对接条件")]
    [Tooltip("对接成功需要的重叠比例（0-1）")]
    [Range(0f, 1f)]
    public float requiredOverlapRatio = 0.5f;

    [Tooltip("允许的最大距离（米）")]
    public float positionTolerance = 0.5f;

    [Tooltip("允许的最大角度偏差（度）")]
    [Range(0f, 45f)]
    public float angleTolerance = 15f;

    [Header("状态标记")]
    public bool isDocked = false;
    public bool isDetectingTarget = false;
    public DockingPort targetDockingPort;

    // 事件
    public System.Action OnDockingSuccess;
    public System.Action OnUndock;

    private Collider portCollider; // 3D碰撞器
    private Rigidbody vesselRigidbody;

    private void Awake()
    {
        // 获取3D碰撞器并设置为触发器
        portCollider = GetComponent<Collider>();
        portCollider.isTrigger = true;

        // 获取所属物体的刚体组件
        if (parentVessel != null)
        {
            vesselRigidbody = parentVessel.GetComponent<Rigidbody>();
        }
    }

    // 3D触发器检测：进入碰撞器
    private void OnTriggerEnter(Collider other)
    {
        CheckAndSetTargetPort(other);
    }

    // 3D触发器检测：停留在碰撞器中
    private void OnTriggerStay(Collider other)
    {
        if (!isDetectingTarget)
        {
            CheckAndSetTargetPort(other);
        }
        else if (!isDocked && targetDockingPort != null)
        {
            // 持续检测对接条件
            if (CheckDockingConditions())
            {
                CompleteDocking();
            }
        }
    }

    // 3D触发器检测：离开碰撞器
    private void OnTriggerExit(Collider other)
    {
        DockingPort otherPort = other.GetComponent<DockingPort>();
        if (otherPort != null && otherPort == targetDockingPort)
        {
            targetDockingPort = null;
            isDetectingTarget = false;
        }
    }

    // 检查并设置目标对接端口
    private void CheckAndSetTargetPort(Collider other)
    {
        DockingPort otherPort = other.GetComponent<DockingPort>();
        if (otherPort != null && !isDocked && !otherPort.isDocked &&
            isStationPort != otherPort.isStationPort)
        {
            targetDockingPort = otherPort;
            isDetectingTarget = true;
        }
    }

    // 检查是否满足所有对接条件
    public bool CheckDockingConditions()
    {
        if (targetDockingPort == null) return false;

        // 1. 检查距离
        float distance = Vector3.Distance(transform.position, targetDockingPort.transform.position);
        bool positionOk = distance <= positionTolerance;

        // 2. 检查角度
        float angle = Vector3.Angle(transform.forward, targetDockingPort.transform.forward);
        bool angleOk = angle <= angleTolerance;

        // 3. 检查重叠
        float overlap = CalculateOverlapRatio();
        bool overlapOk = overlap >= requiredOverlapRatio;

        return positionOk && angleOk && overlapOk;
    }

    // 计算3D碰撞器的重叠比例
    public float CalculateOverlapRatio()
    {
        if (portCollider == null || targetDockingPort.portCollider == null) return 0;

        Bounds currentBounds = portCollider.bounds;
        Bounds targetBounds = targetDockingPort.portCollider.bounds;

        if (!currentBounds.Intersects(targetBounds)) return 0;

        // 计算重叠区域
        Bounds overlapBounds = new Bounds();
        overlapBounds.SetMinMax(
            Vector3.Max(currentBounds.min, targetBounds.min),
            Vector3.Min(currentBounds.max, targetBounds.max)
        );

        // 计算体积比例（3D）
        float currentVolume = currentBounds.size.x * currentBounds.size.y * currentBounds.size.z;
        float overlapVolume = overlapBounds.size.x * overlapBounds.size.y * overlapBounds.size.z;

        return currentVolume > 0 ? overlapVolume / currentVolume : 0;
    }

    // 完成对接
    private void CompleteDocking()
    {
        isDocked = true;
        targetDockingPort.isDocked = true;

        // 锁定位置
        if (!isStationPort && parentVessel != null)
        {
            parentVessel.SetParent(targetDockingPort.parentVessel);

            // 精确对齐位置
            Vector3 offset = targetDockingPort.transform.position - transform.position;
            parentVessel.position += offset;

            // 冻结物理运动
            if (vesselRigidbody != null)
            {
                vesselRigidbody.velocity = Vector3.zero;
                vesselRigidbody.angularVelocity = Vector3.zero;
                vesselRigidbody.isKinematic = true;
            }
        }

        OnDockingSuccess?.Invoke();
        targetDockingPort.OnDockingSuccess?.Invoke();
    }

    // 解除对接
    public void Undock()
    {
        if (!isDocked || targetDockingPort == null) return;

        isDocked = false;
        targetDockingPort.isDocked = false;

        // 解除父物体关联
        if (!isStationPort && parentVessel != null)
        {
            parentVessel.SetParent(null);

            // 恢复物理控制
            if (vesselRigidbody != null)
            {
                vesselRigidbody.isKinematic = false;
            }
        }

        OnUndock?.Invoke();
        targetDockingPort.OnUndock?.Invoke();

        targetDockingPort = null;
        isDetectingTarget = false;
    }

    // 绘制Gizmos辅助调试
    private void OnDrawGizmos()
    {
        if (isDocked)
        {
            Gizmos.color = Color.green; // 已对接：绿色
        }
        else if (isDetectingTarget)
        {
            Gizmos.color = Color.yellow; // 检测到目标：黄色
        }
        else
        {
            Gizmos.color = Color.blue; // 未检测到目标：蓝色
        }

        // 绘制碰撞器范围
        if (portCollider != null)
        {
            Gizmos.DrawWireCube(portCollider.bounds.center, portCollider.bounds.size);
        }
    }
}
