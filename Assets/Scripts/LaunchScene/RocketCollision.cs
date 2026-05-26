using UnityEngine;

public class RocketCollision : MonoBehaviour
{
    public RocketControl rocketControl;
    public GameManager gameManager;

    // 地面层标签
    public string groundTag = "Land";

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 检查是否碰撞到地面，且火箭已发射但未爆炸
        if (collision.collider.CompareTag(groundTag) &&
            rocketControl.GetCurrentState() == RocketControl.RocketState.Launched)
        {
            // 触发爆炸
            rocketControl.Explode();
            gameManager.GameOver("火箭坠毁!");
        }
    }
}
