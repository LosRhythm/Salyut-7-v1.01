using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RocketAudioController : MonoBehaviour
{
    [Header("必要引用")]
    [Tooltip("必须指定RocketControl组件")]
    public RocketControl rocketControl; // 手动指定RocketControl组件

    [Header("音效引用")]
    public AudioClip launchSound; // 发射音效
    public AudioClip engineSound; // 引擎运行音效
    public AudioClip explosionSound; // 爆炸音效
    public AudioClip fuelDepletedSound; // 燃油耗尽音效

    [Header("音效设置")]
    [Range(0f, 1f)] public float launchVolume = 1f;
    [Range(0f, 1f)] public float engineVolume = 0.8f;
    [Range(0f, 1f)] public float explosionVolume = 0.9f;
    [Range(0f, 1f)] public float fuelDepletedVolume = 0.7f;
    public bool engineSoundFadesWithThrottle = true;

    private AudioSource audioSource;
    private bool isLaunchSoundPlayed = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // 强制检查是否设置了RocketControl
        if (rocketControl == null)
        {
            Debug.LogError("请在Inspector中为RocketAudioController指定RocketControl组件！");
        }
    }

    private void Start()
    {
        audioSource.playOnAwake = false;
        audioSource.loop = true;
    }

    private void Update()
    {
        // 如果没有指定RocketControl，不执行任何操作
        if (rocketControl == null) return;

        // 根据火箭状态控制音效
        switch (rocketControl.GetCurrentState())
        {
            case RocketControl.RocketState.Ready:
                HandleReadyState();
                break;
            case RocketControl.RocketState.Launched:
                HandleLaunchedState();
                break;
            case RocketControl.RocketState.Exploded:
                HandleExplodedState();
                break;
            case RocketControl.RocketState.InSpace:
                HandleInSpaceState();
                break;
        }
    }

    private void HandleReadyState()
    {
        // 停止所有音效
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        // 发射时播放发射音效
        if (Input.GetKeyDown(KeyCode.Space) && !isLaunchSoundPlayed)
        {
            PlayLaunchSound();
        }
    }

    private void HandleLaunchedState()
    {
        // 播放引擎音效
        if (!audioSource.isPlaying && engineSound != null)
        {
            audioSource.clip = engineSound;
            audioSource.volume = engineVolume;
            audioSource.Play();
        }

        // 随推力变化调整引擎音效
        if (engineSoundFadesWithThrottle)
        {
            // 从RocketControl获取当前推力比例
            float thrustRatio = rocketControl.currentThrust / rocketControl.maxThrust;
            audioSource.volume = Mathf.Lerp(0.2f, engineVolume, thrustRatio);
            audioSource.pitch = Mathf.Lerp(0.8f, 1.5f, thrustRatio);
        }
    }

    private void HandleExplodedState()
    {
        // 播放爆炸音效（只播放一次）
        if (explosionSound != null && !audioSource.isPlaying)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, explosionVolume);
        }
    }

    private void HandleInSpaceState()
    {
        // 进入太空后逐渐停止引擎音效
        if (audioSource.isPlaying && audioSource.clip == engineSound)
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, 0, Time.deltaTime * 2);
            if (audioSource.volume < 0.05f)
            {
                audioSource.Stop();
            }
        }
    }

    // 播放发射音效
    public void PlayLaunchSound()
    {
        if (launchSound != null)
        {
            AudioSource.PlayClipAtPoint(launchSound, transform.position, launchVolume);
            isLaunchSoundPlayed = true;
        }
    }

    // 播放燃油耗尽音效
    public void PlayFuelDepletedSound()
    {
        if (fuelDepletedSound != null)
        {
            AudioSource.PlayClipAtPoint(fuelDepletedSound, transform.position, fuelDepletedVolume);
        }
    }

    // 播放爆炸音效
    public void PlayExplosionSound()
    {
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, explosionVolume);
        }
    }
}
