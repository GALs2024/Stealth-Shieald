using UnityEngine;

public static class AudioUtils
{
    /// <summary>
    /// SFX（効果音）を再生するユーティリティ
    /// </summary>
    public static void PlaySFX(AudioSource audioSource, AudioClip clip, float volume = 1.0f)
    {
        if (audioSource == null || clip == null) return;
        audioSource.PlayOneShot(clip, volume);
    }

    /// <summary>
    /// BGMを再生するユーティリティ
    /// </summary>
    public static void PlayBGM(AudioSource audioSource, AudioClip clip, float volume = 1.0f, bool loop = true)
    {
        if (audioSource == null || clip == null) return;
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.Play();
    }

    /// <summary>
    /// 再生中のオーディオを停止する
    /// </summary>
    public static void StopAudio(AudioSource audioSource)
    {
        if (audioSource == null) return;
        audioSource.Stop();
    }

    /// <summary>
    /// 一時停止
    /// </summary>
    public static void PauseAudio(AudioSource audioSource)
    {
        if (audioSource == null) return;
        audioSource.Pause();
    }

    /// <summary>
    /// 再生を再開
    /// </summary>
    public static void ResumeAudio(AudioSource audioSource)
    {
        if (audioSource == null) return;
        audioSource.UnPause();
    }
}
