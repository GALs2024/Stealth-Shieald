using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFader : MonoBehaviour
{
    private AudioSource audioSource;
    private GameObject bgmManager;

    void Start()
    {
        FindBGMAudioSource();
    }

    public void FadeOut(float fadeTime)
    {
        if (bgmManager != null)
        {
            StartCoroutine(AudioFaderUtils.FadeOut(audioSource, fadeTime));
        }
    }

    void FindBGMAudioSource()
    {
        bgmManager = GameObject.Find("BGMManager");

        if (bgmManager != null)
        {
            audioSource = bgmManager.GetComponent<AudioSource>();

            if (audioSource != null)
            {
                Debug.Log("BGMManagerのAudioSourceを見つけました: " + audioSource.name);
            }
            else
            {
                Debug.LogError("BGMManagerオブジェクトにはAudioSourceがアタッチされていません。");
            }
        }
        else
        {
            Debug.LogError("BGMManagerオブジェクトが見つかりません。");
        }
    }
}
