using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    // 引用音频源组件
    private AudioSource audioSource;

    // 要播放的背景音乐
    public AudioClip backgroundMusic;

    // 是否在场景加载时自动播放
    public bool playOnStart = true;

    private void Awake()
    {
        // 获取音频源组件
        audioSource = GetComponent<AudioSource>();

        // 配置音频源
        audioSource.clip = backgroundMusic;
        audioSource.loop = true; // 循环播放
        audioSource.playOnAwake = false; // 不在唤醒时自动播放
    }

    private void Start()
    {
        // 注册场景加载事件
        SceneManager.sceneLoaded += OnSceneLoaded;

        // 如果设置了自动播放，则开始播放
        if (playOnStart && backgroundMusic != null)
        {
            PlayMusic();
        }
    }

    // 播放音乐
    public void PlayMusic()
    {
        if (audioSource != null && !audioSource.isPlaying && backgroundMusic != null)
        {
            audioSource.Play();
        }
    }

    // 停止音乐
    public void StopMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    // 场景加载时的回调
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 当新场景加载时停止当前音乐
        StopMusic();

        // 注销事件，避免重复注册
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // 确保在对象销毁时注销事件
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
