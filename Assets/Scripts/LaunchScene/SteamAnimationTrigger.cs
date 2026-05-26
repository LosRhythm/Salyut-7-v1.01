

using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SteamAnimationTrigger : MonoBehaviour
{
    [Tooltip("是否在游戏开始时隐藏蒸汽")]
    public bool hideOnStart = true;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool hasLaunched = false;

    void Start()
    {
        // 获取组件
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 初始隐藏蒸汽
        if (hideOnStart)
        {
            StopAnimation();
        }
        else
        {
            // 确保动画处于停止状态
            animator.enabled = false;
        }
    }

    void Update()
    {
        // 按下空格键且尚未发射时触发动画
        if (Input.GetKeyDown(KeyCode.Space) && !hasLaunched)
        {
            LaunchRocket();
        }
    }

    // 启动火箭并播放蒸汽动画
    public void LaunchRocket()
    {
        hasLaunched = true;
        PlayAnimation();
        Debug.Log("火箭发射，蒸汽动画开始播放");
    }

    // 播放蒸汽动画
    private void PlayAnimation()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true; // 显示蒸汽
        }

        if (animator != null)
        {
            animator.enabled = true;
            animator.Play("SteamAnim"); // 播放动画（确保与你的动画剪辑名称一致）
        }
    }

    // 停止并隐藏蒸汽动画
    private void StopAnimation()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false; // 隐藏蒸汽
        }

        if (animator != null)
        {
            animator.enabled = false;
        }
    }
}
