
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(Rigidbody2D))]
public class RocketControl : MonoBehaviour
{
    //火箭状态
    public enum RocketState { Ready, Launched, Exploded, InSpace }

    [Header("火箭参数")]
    public float maxThrust = 20f;   //最大推力
    public float minThrust = 5f;    //最小推力
    public float thrustIncrement = 0.5f;    //推力增减量
    public float rotationSpeed = 20f;   //旋转速度
    public float maxFuel = 100f;    //最大燃油量
    public float fuelConsumptionRate = 2f;  //燃油消耗率

    [Header("UI引用")]
    public CustomGUISlider thrustSlider; //推力显示滑块
    public CustomGUISlider fuelSlider;   //燃油显示滑块
    public CustomGUILabel altitudeText;   //高度显示文本
    public CustomGUILabel thrustText;     //推力显示文本

    [Header("动画控制")]
    public GameObject flameEffect; // 火焰动画对象（包含粒子系统或序列帧动画）
    public float flameStartDelay = 0.1f; // 火焰启动延迟（可选）

    [Header("其他引用")]
    //public GameObject explosionPrefab;  //爆炸效果预制体
    public GameObject explosionPrefab;   //爆炸动画
    public float explosionDuration = 2f;    //爆炸动画持续时间
    //public SteamAnimationTrigger steamAnimation;
    //public Animator transitionAnimator; //过场动画Animator
    public VideoPlayer videoPlayer;
    public GameManager gameManager;   //游戏管理器

    private Rigidbody2D rb;
    private RocketState currentState = RocketState.Ready;
    public float currentThrust;
    private float currentFuel;
    private float currentAltitude;
    private Vector2 launchPosition; //发射位置

    private const float spaceAltitudeThreshold = 2000f; //太空高度阈值

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;

        //初始化参数
        currentThrust = minThrust;
        currentFuel = maxFuel;
        launchPosition = transform.position;

                // 初始化视频播放器
        if (videoPlayer == null)
        {
            videoPlayer = gameObject.AddComponent<VideoPlayer>();
        }

        if (explosionPrefab != null)
        {
            explosionPrefab.SetActive(false);
        }

        // 初始化火焰状态（隐藏）
        if (flameEffect != null)
        {
            flameEffect.SetActive(false);
        }

        //初始化UI
        UpdateUI();
    }

    private void Update()
    {
        switch (currentState)
        {
            case RocketState.Ready:
                HandleReadyState();
                break;
            case RocketState.Launched:
                HandleLaunchedState();
                break;
            case RocketState.Exploded:
                break;
            case RocketState.InSpace:
                break;
        }
    }


    private void FixedUpdate()
    {
        if (currentState == RocketState.Launched)
        {
            //引用推力
            Vector2 thrustForce = transform.up * currentThrust;
            rb.AddForce(thrustForce);

            //计算当前高度
            currentAltitude = (transform.position.y - launchPosition.y);
            altitudeText.content.text = $"高度：{currentAltitude:F0} m";

            //检查是否到达太空
            if (currentAltitude >= spaceAltitudeThreshold && currentState != RocketState.InSpace)
            {
                EnterSpace();
            }
        }
    }

    // 处理就绪状态
    private void HandleReadyState()
    {
        // 调节推力
        AdjustThrust();

        // 按空格键发射
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Launch();
            StartFlameAnimation();
        }
    }

    // 处理发射后状态
    private void HandleLaunchedState()
    {
        // 调节推力
        AdjustThrust();

        // 控制方向（左右箭头或A/D键）
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.forward, -rotation);

        // 消耗燃油
        ConsumeFuel();
    }

    // 启动火焰动画
    private void StartFlameAnimation()
    {
        if (flameEffect != null)
        {
            // 如果需要延迟启动（模拟点火过程）
            if (flameStartDelay > 0)
            {
                Invoke(nameof(ActivateFlame), flameStartDelay);
            }
            else
            {
                ActivateFlame();
            }
        }
    }

    // 激活火焰显示
    private void ActivateFlame()
    {
        if (flameEffect != null)
        {
            flameEffect.SetActive(true);
            // 如果是粒子系统，确保播放
            var particle = flameEffect.GetComponent<ParticleSystem>();
            if (particle != null)
            {
                particle.Play();
            }
            // 如果是Animator控制的动画
            var animator = flameEffect.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("StartFlame");
            }
        }
    }

    // 停止火焰动画
    private void StopFlameAnimation()
    {
        if (flameEffect != null)
        {
            // 粒子系统停止
            var particle = flameEffect.GetComponent<ParticleSystem>();
            if (particle != null)
            {
                particle.Stop();
            }
            // Animator动画停止
            var animator = flameEffect.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("StopFlame");
            }
            // 延迟隐藏（如果有熄灭动画）
            Destroy(flameEffect, 0.5f);
        }
    }

    private void AdjustThrust()
    {
        if (Input.GetKey(KeyCode.W))
        {
            currentThrust = Mathf.Min(currentThrust + thrustIncrement * Time.deltaTime * 10, maxThrust);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            currentThrust = Mathf.Max(currentThrust - thrustIncrement * Time.deltaTime * 10, minThrust);
        }

        //更新UI
        thrustSlider.nowValue = currentThrust / maxThrust;
        thrustText.content.text = $"推力：{currentThrust:F1} kN";

    }

    private float GetConsumeFuelRate()
    {
        return currentThrust / maxThrust;
    }

    private void Launch()
    {
        currentState = RocketState.Launched;
        rb.isKinematic = false;
        gameManager.OnLaunch();
    }

    private void ConsumeFuel()
    {
        currentFuel -= fuelConsumptionRate * Time.deltaTime * GetConsumeFuelRate();
        currentFuel = Mathf.Max(currentFuel, 0);

        fuelSlider.nowValue = currentFuel / maxFuel;

        //燃油耗尽
        if (currentFuel <= 0)
        {
            Explode();
            gameManager.GameOver("燃油耗尽！");
        }

    }

    private void EnterSpace()
    {
        currentState = RocketState.InSpace;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        // 播放进入太空的视频
        PlayVideo();

        //触发过场动画
        //transitionAnimator.SetTrigger("ToSpace");
        gameManager.OnEnterSpace();

        // 进入太空后停止火焰
        StopFlameAnimation();
    }

    public void Explode()
    {
        if (currentState != RocketState.Exploded)
        {
            currentState = RocketState.Exploded;
            rb.isKinematic = true;

            // 爆炸时停止火焰
            StopFlameAnimation();

            // 正确的爆炸实例创建方式
            if (explosionPrefab != null)
            {

                explosionPrefab.SetActive(true);
                // 实例化预制体并获取引用
                GameObject explosionInstance = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                // 延迟销毁爆炸效果
                Destroy(explosionInstance, explosionDuration);
            }

            // 隐藏火箭
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;

            // 延迟销毁火箭
            Destroy(gameObject, explosionDuration);
        }
    }

    //更新UI
    private void UpdateUI()
    {
        thrustSlider.maxValue = 1;
        thrustSlider.nowValue = currentThrust / maxThrust;
        thrustText.content.text = $"推力：{currentThrust:F1} kN";

        fuelSlider.maxValue = 1;
        fuelSlider.nowValue = currentFuel / maxFuel;
        altitudeText.content.text = $"高度: 0 m";
    }

    public RocketState GetCurrentState()
    {
        return currentState;
    }

    // 播放视频的方法
private void PlayVideo()
{
    if (videoPlayer != null && videoPlayer.url != "")
    {
        // 确保视频准备就绪
        videoPlayer.Prepare();
        
        // 准备完成后播放
        videoPlayer.prepareCompleted += VideoPrepared;
        
        // 如果需要隐藏游戏UI
        if (gameManager != null)
        {
            //gameManager.HideGameUI();
        }
    }
    else
    {
        Debug.LogError("VideoPlayer未正确配置或未指定视频文件");
    }
}

// 视频准备完成回调
private void VideoPrepared(VideoPlayer vp)
{
    vp.Play();
    // 移除回调以避免重复调用
    vp.prepareCompleted -= VideoPrepared;
}

// 停止视频播放
private void StopVideo()
{
    if (videoPlayer != null && videoPlayer.isPlaying)
    {
        videoPlayer.Stop();
    }
}

}