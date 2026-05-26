using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager instance; // 单例实例

    [Header("背景音乐设置")]
    public AudioClip mainTheme; // 主背景音乐
    [Range(0f, 1f)] public float defaultVolume = 0.5f; // 默认音量
    public bool playOnStart = true; // 是否在游戏开始时播放

    private AudioSource audioSource;

    private void Awake()
    {
        // 确保整个游戏中只有一个背景音乐管理器
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 场景切换时不销毁
        }
        else
        {
            Destroy(gameObject); // 销毁重复的实例
        }

        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        // 初始化音频设置
        audioSource.loop = true; // 循环播放
        audioSource.volume = defaultVolume;
        audioSource.playOnAwake = false;

        // 如果指定了音乐且需要自动播放
        if (mainTheme != null && playOnStart)
        {
            PlayMusic(mainTheme);
        }
    }

    // 播放指定的背景音乐
    public void PlayMusic(AudioClip music)
    {
        if (music == null) return;

        // 如果播放的是新音乐，切换音乐
        if (audioSource.clip != music)
        {
            audioSource.clip = music;
        }

        // 如果没有在播放，开始播放
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    // 继续播放音乐
    public void ResumeMusic()
    {
        if (audioSource.clip != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    // 暂停音乐
    public void PauseMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    // 停止音乐
    public void StopMusic()
    {
        audioSource.Stop();
    }

    // 调整音量
    public void SetVolume(float volume)
    {
        audioSource.volume = Mathf.Clamp01(volume);
    }

    // 切换音乐播放状态（播放/暂停）
    public void ToggleMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
        else if (audioSource.clip != null)
        {
            audioSource.Play();
        }
    }
}
