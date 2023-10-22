using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != null)
            {
                Destroy(gameObject);
            }
        }
        DontDestroyOnLoad(gameObject);
    }

    public AudioSource audioSource;
    public AudioSource audioSource_02;

    public AudioClip[] hit;
    public AudioClip skill_DaShu; // 大树地刺和树枝
    public AudioClip erorr;         // 警告
    public AudioClip longFire;      // 黑龙火焰喷出时
    public AudioClip renmaATK;      // 人马准备冲刺
    public AudioClip longAtk;       // 龙准备俯冲
    public AudioClip longTalk;      // 黑龙日常用语

    private void Start()
    {

    }

    public void PlaySound_02(AudioClip sound)
    {
        // 停止当前正在播放的声音
        audioSource.Stop();

        // 分配新的声音文件
        audioSource.clip = sound;

        // 播放新的声音
        audioSource.Play();
    }


    public void PlaySound(AudioClip sound)
    {
        // 停止当前正在播放的声音
        audioSource.Stop();

        // 分配新的声音文件
        audioSource.clip = sound;

        // 播放新的声音
        audioSource.Play();
    }
}
