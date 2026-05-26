using UnityEngine;

public class SalyutController : MonoBehaviour
{
    [Header("旋转设置")]
    public float rotationSpeed = 5f; // 缓慢旋转

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // 设置为运动学刚体，保持固定
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0;
    }

    void Update()
    {
        // 让礼炮7号缓慢旋转，增加真实感
        transform.Rotate(rotationSpeed * Time.deltaTime, 0, 0);
    }
}
