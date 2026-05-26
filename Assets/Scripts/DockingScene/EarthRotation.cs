using UnityEngine;

public class EarthRotation : MonoBehaviour
{
    // 自转速度，可以在Inspector面板调整
    [Tooltip("地球自转速度，单位是度/秒")]
    public float rotationSpeed = 10f;

    // 自转轴向，默认绕Y轴旋转
    [Tooltip("地球自转的轴向")]
    public Vector3 rotationAxis = Vector3.up;

    void Update()
    {
        // 每一帧都让物体绕指定轴旋转
        // Time.deltaTime确保旋转速度不受帧率影响
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
    }
}
