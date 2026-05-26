using UnityEngine;

public class CameraAxisController : MonoBehaviour
{
    [Tooltip("要跟随的目标（空间站）")]
    public Transform target;

    [Tooltip("基于目标本地轴向的位置偏移")]
    public Vector3 localAxisOffset = new Vector3(0, 10, -20);

    [Tooltip("相机看向的本地轴向（相对于目标）")]
    public Vector3 lookAtLocalAxis = Vector3.forward;

    [Tooltip("相机自身上方向的本地轴向")]
    public Vector3 cameraUpAxis = Vector3.up;

    [Tooltip("位置跟随平滑度")]
    [Range(0.1f, 20f)] public float positionSmooth = 10f;

    [Tooltip("旋转跟随平滑度")]
    [Range(0.1f, 20f)] public float rotationSmooth = 15f;

    private void LateUpdate()
    {
        if (target == null) return;

        // 计算相机目标位置（基于目标的本地轴向）
        Vector3 targetPosition = CalculateTargetPosition();

        // 计算相机目标旋转（基于看向的轴向）
        Quaternion targetRotation = CalculateTargetRotation();

        // 平滑移动到目标位置
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            positionSmooth * Time.deltaTime
        );

        // 平滑旋转到目标朝向
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            rotationSmooth * Time.deltaTime
        );
    }

    // 计算基于目标本地轴向的相机位置
    private Vector3 CalculateTargetPosition()
    {
        // 将本地轴向偏移转换为世界坐标
        Vector3 offset =
            localAxisOffset.x * target.right +
            localAxisOffset.y * target.up +
            localAxisOffset.z * target.forward;

        return target.position + offset;
    }

    // 计算相机应该朝向的方向
    private Quaternion CalculateTargetRotation()
    {
        // 计算看向的目标方向（基于目标的本地轴向）
        Vector3 lookDirection =
            lookAtLocalAxis.x * target.right +
            lookAtLocalAxis.y * target.up +
            lookAtLocalAxis.z * target.forward;

        // 计算相机上方向（基于指定的轴向）
        Vector3 upDirection =
            cameraUpAxis.x * target.right +
            cameraUpAxis.y * target.up +
            cameraUpAxis.z * target.forward;

        // 生成目标旋转
        return Quaternion.LookRotation(lookDirection.normalized, upDirection.normalized);
    }

    // 场景视图可视化
    private void OnDrawGizmosSelected()
    {
        if (target == null) return;

        // 绘制相机到目标的连接线
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, target.position);

        // 绘制相机看向的方向
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 15f);

        // 绘制相机上方向
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * 5f);
    }
}
