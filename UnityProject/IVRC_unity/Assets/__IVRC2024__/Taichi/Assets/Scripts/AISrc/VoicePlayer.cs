using System.Collections;
using UnityEngine;

public class VoicePlayer : MonoBehaviour
{
    public AudioClip[] audioClips; // 再生するオーディオクリップの配列
    public float[] waitTimes; // 各クリップの再生開始までの待機時間（秒）
    public AudioSource audioSource; // 使用するAudioSource

    private void Start()
    {
        // 待機時間配列がクリップ数と同じ長さかチェック
        if (waitTimes.Length != audioClips.Length)
        {
            Debug.LogError("待機時間の配列とオーディオクリップの配列の長さが一致しません");
            return;
        }

        // 各オーディオクリップの再生を独立して開始
        for (int i = 0; i < audioClips.Length; i++)
        {
            StartCoroutine(PlayAudioClipWithWait(i));
        }
    }

    private IEnumerator PlayAudioClipWithWait(int index)
    {
        AudioClip clip = audioClips[index];

        if (clip != null)
        {
            // 待機時間の経過を待つ
            yield return new WaitForSeconds(waitTimes[index]);

            // AudioSourceを使ってoneshotでクリップを再生
            audioSource.PlayOneShot(clip);
        }
    }
}
