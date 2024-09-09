using System.Collections;
using UnityEngine;

public class VoicePlayer : MonoBehaviour
{
    public AudioClip[] audioClips; // 再生するオーディオクリップの配列
    public float intervalBetweenClips = 1f; // クリップ間の待機時間（秒）
    public AudioSource audioSource; // 使用するAudioSource

    private void Start()
    {
        // オーディオクリップの再生を開始
        StartCoroutine(PlayAudioClipsWithInterval());
    }

    private IEnumerator PlayAudioClipsWithInterval()
    {
        foreach (AudioClip clip in audioClips)
        {
            if (clip != null)
            {
                // AudioSourceを使ってoneshotでクリップを再生
                audioSource.PlayOneShot(clip);
                
                // 次のクリップ再生まで指定された秒数待機
                yield return new WaitForSeconds(clip.length + intervalBetweenClips);
            }
        }
    }
}
