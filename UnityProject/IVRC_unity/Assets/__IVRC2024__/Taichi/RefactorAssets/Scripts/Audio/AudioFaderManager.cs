using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFaderManager : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        FindBGMAudioSource();
    }

    public void FadeOut(float fadeTime)
    {
        StartCoroutine(AudioFader.FadeOut(audioSource, fadeTime));
    }

    void FindBGMAudioSource()
    {
        // シーン内からAudioManagerオブジェクトを探す
        GameObject audioManager = GameObject.Find("AudioManager");

        if (audioManager != null)
        {
            // AudioManagerの子オブジェクトであるBGMを探す
            Transform bgmTransform = audioManager.transform.Find("BGM");

            if (bgmTransform != null)
            {
                // BGMオブジェクトにアタッチされているAudioSourceを取得
                audioSource = bgmTransform.GetComponent<AudioSource>();

                if (audioSource != null)
                {
                    Debug.Log("BGMのAudioSourceを見つけました: " + audioSource.name);
                }
                else
                {
                    Debug.LogError("BGMオブジェクトにはAudioSourceがアタッチされていません。");
                }
            }
            else
            {
                Debug.LogError("BGMオブジェクトが見つかりません。");
            }
        }
        else
        {
            Debug.LogError("AudioManagerオブジェクトが見つかりません。");
        }
    }
}
