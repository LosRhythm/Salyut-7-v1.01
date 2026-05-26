using UnityEngine;

public class SolarPanelLocalAxisTracker : MonoBehaviour
{
    [Tooltip("指定太阳对象")]
    public Transform sunTransform;

    [Tooltip("太阳能板转向太阳的速度")]
    public float rotationSpeed = 5f;

    [Tooltip("允许旋转的本地轴向（基于太阳能板自身坐标系）")]
    public Vector3 localRotationAxis = Vector3.right; // 本地X轴

    [Tooltip("太阳能板工作面的本地朝向（基于自身坐标系）")]
    public Vector3 localFaceDirection = Vector3.forward; // 本地Z轴

    private Vector3 normalizedLocalAxis;
    private Vector3 normalizedLocalFace;

    void Start()
    {
        // 自动查找太阳
        if (sunTransform == null)
        {
            GameObject sun = GameObject.Find("Sun");
            if (sun != null)
            {
                sunTransform = sun.transform;
            }
        }

        // 标准化本地向量
        normalizedLocalAxis = localRotationAxis.normalized;
        normalizedLocalFace = localFaceDirection.normalized;
    }

    void Update()
    {
        if (sunTransform != null)
        {
            TrackSunWithLocalAxis();
        }
    }

    void TrackSunWithLocalAxis()
    {
        // 将太阳方向转换到太阳能板的本地坐标系
        Vector3 sunDirectionInLocal = transform.InverseTransformDirection(
            sunTransform.position - transform.position
        ).normalized;

        // 获取当前工作面在本地坐标系中的方向
        Vector3 currentLocalFace = normalizedLocalFace;

        // 计算在本地坐标系中需要旋转的角度
        float angle = Vector3.Angle(currentLocalFace, sunDirectionInLocal);

        if (angle > 0.5f) // 角度足够大时才旋转
        {
            // 计算本地旋转轴
            Vector3 localRotation = Vector3.Cross(currentLocalFace, sunDirectionInLocal).normalized;

            // 仅保留指定的本地旋转轴分量
            Vector3 constrainedLocalAxis = Vector3.Project(localRotation, normalizedLocalAxis).normalized;

            if (constrainedLocalAxis.sqrMagnitude > 0.001f)
            {
                // 计算旋转角度（受速度限制）
                float rotateAngle = Mathf.Min(angle, rotationSpeed * Time.deltaTime);

                // 应用本地旋转
                transform.Rotate(constrainedLocalAxis, rotateAngle, Space.Self);
            }
        }
    }

    // 可视化本地轴向和工作面方向
    void OnDrawGizmosSelected()
    {
        // 绘制本地旋转轴（红色）
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position,
                       transform.position + transform.TransformDirection(normalizedLocalAxis) * 1.5f);

        // 绘制工作面方向（蓝色）
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position,
                       transform.position + transform.TransformDirection(normalizedLocalFace) * 2f);
    }
}
