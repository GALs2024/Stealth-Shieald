using UnityEngine;
using System.Collections;

public static class AudioFader
{
    /// <summary>
    /// フェードイン
    /// </summary>
    public static IEnumerator FadeIn(AudioSource audioSource, AudioClip clip, float maxVolume, float fadeTime)
    {
        if (audioSource == null || clip == null) yield break;

        audioSource.volume = 0f;

        // PlayOneShotで指定されたclipを再生
        audioSource.PlayOneShot(clip);

        while (audioSource.volume < maxVolume)
        {
            audioSource.volume += Time.deltaTime / fadeTime;
            yield return null;
        }

        audioSource.volume = maxVolume;  // maxVolumeで固定
    }

    /// <summary>
    /// フェードアウト
    /// </summary>
    public static IEnumerator FadeOut(AudioSource audioSource, float fadeTime)
    {
        if (audioSource == null) yield break;

        float startVolume = audioSource.volume;

        while (audioSource.volume > 0f)
        {
            audioSource.volume -= Time.deltaTime / fadeTime;
            yield return null;
        }

        audioSource.Stop();  // 再生を停止
        audioSource.volume = startVolume;  // 元のボリュームに戻す
    }
}
