using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AIMovieManager : MonoBehaviour
{
    public string sceneName;

    // 2つのAudioSourceを用意
    public AudioSource newBGMAudioSource;
    public AudioSource audioSource;
    public AudioSource lastBGMAudioSource;

    // 2つの音声クリップ
    public AudioClip newBGMClip;
    public AudioClip audioClip;
    public float delay = 2.0f; // audioClipの再生を待機する時間
    public float sceneChangeDelay = 5.0f; // シーン切り替えの待機時間

    public void StartMovie()
    {
        Debug.Log("StartMovie");

        StopAudioClips();

        // 指定した待機時間後に音声クリップを再生
        StartCoroutine(PlayAudioClipsWithDelay(newBGMAudioSource, newBGMClip, 0.0f));
        StartCoroutine(PlayAudioClipsWithDelay(audioSource, audioClip, delay));

        // シーンを指定した時間後に切り替える
        StartCoroutine(ChangeSceneAfterDelay(sceneChangeDelay));
    }

    // 指定時間後にシーンを切り替えるコルーチン
    IEnumerator ChangeSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ChangeScene();
    }

    void ChangeScene()
    {
        SceneManager.LoadScene(this.sceneName);
    }

    // 待機後に2つの音声を同時に再生するメソッド
    IEnumerator PlayAudioClipsWithDelay(AudioSource audioSource, AudioClip clip, float delay)
    {
        // delayの時間待機
        yield return new WaitForSeconds(delay);
        if (audioSource != null && clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    // 2つのAudioSourceを停止するメソッド
    public void StopAudioClips()
    {
        if (lastBGMAudioSource != null && lastBGMAudioSource.isPlaying)
        {
            lastBGMAudioSource.Stop();
        }
    }
}
