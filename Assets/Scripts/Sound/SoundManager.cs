using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Sound
{
    public string soundName;      // 音效名称，用于查找和播放
    public AudioClip clip;        // 音效资源

    [Range(0f, 1f)]
    public float volume = 1f;     // 音量

    [Range(-3f, 3f)]
    public float pitch = 1f;      // 音调

    public bool loop = false;     // 是否循环播放

    [HideInInspector]
    public AudioSource source;    // 音频源组件
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;  // 单例实例

    [SerializeField]
    private Sound[] sounds;               // 音效数组，可在Inspector中配置

    private void Awake()
    {
        // 确保只有一个AudioManager实例
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 切换场景时不销毁
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 为每个音效创建AudioSource组件
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    /// <summary>
    /// 播放指定名称的音效
    /// </summary>
    /// <param name="soundName">音效名称</param>
    public void Play(string soundName)
    {
        Sound s = FindSound(soundName);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + soundName + " not found!");
            return;
        }

        s.source.Play();
    }

    /// <summary>
    /// 停止指定名称的音效
    /// </summary>
    /// <param name="soundName">音效名称</param>
    public void Stop(string soundName)
    {
        Sound s = FindSound(soundName);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + soundName + " not found!");
            return;
        }

        s.source.Stop();
    }

    /// <summary>
    /// 暂停指定名称的音效
    /// </summary>
    /// <param name="soundName">音效名称</param>
    public void Pause(string soundName)
    {
        Sound s = FindSound(soundName);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + soundName + " not found!");
            return;
        }

        s.source.Pause();
    }

    /// <summary>
    /// 继续播放指定名称的音效
    /// </summary>
    /// <param name="soundName">音效名称</param>
    public void UnPause(string soundName)
    {
        Sound s = FindSound(soundName);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + soundName + " not found!");
            return;
        }

        s.source.UnPause();
    }

    /// <summary>
    /// 设置指定音效的音量
    /// </summary>
    /// <param name="soundName">音效名称</param>
    /// <param name="volume">音量值(0-1)</param>
    public void SetVolume(string soundName, float volume)
    {
        Sound s = FindSound(soundName);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + soundName + " not found!");
            return;
        }

        s.volume = Mathf.Clamp01(volume);
        s.source.volume = s.volume;
    }

    /// <summary>
    /// 查找音效
    /// </summary>
    private Sound FindSound(string soundName)
    {
        return System.Array.Find(sounds, sound => sound.soundName == soundName);
    }
}
