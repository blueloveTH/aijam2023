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
    public AudioClip skill_DaShu; // �����ش̺���֦
    public AudioClip erorr;         // ����
    public AudioClip longFire;      // �����������ʱ
    public AudioClip renmaATK;      // ����׼�����
    public AudioClip longAtk;       // ��׼������
    public AudioClip longTalk;      // �����ճ�����

    private void Start()
    {

    }

    public void PlaySound_02(AudioClip sound)
    {
        // ֹͣ��ǰ���ڲ��ŵ�����
        audioSource.Stop();

        // �����µ������ļ�
        audioSource.clip = sound;

        // �����µ�����
        audioSource.Play();
    }


    public void PlaySound(AudioClip sound)
    {
        // ֹͣ��ǰ���ڲ��ŵ�����
        audioSource.Stop();

        // �����µ������ļ�
        audioSource.clip = sound;

        // �����µ�����
        audioSource.Play();
    }
}
