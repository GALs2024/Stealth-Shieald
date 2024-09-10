using System.Collections;
using UnityEngine;

public class VoicePlayer : MonoBehaviour
{
    public AudioClip[] audioClips; // 再生するオーディオクリップの配列
    public float[] beforeWaitTimes;
    public float[] afterwaitTimes;
    public float[] volumes;
    public AudioSource audioSource; // 使用するAudioSource

    private void Start()
    {
        // 待機時間配列がクリップ数と同じ長さかチェック
        if (this.beforeWaitTimes.Length != this.audioClips.Length)
        {
            Debug.LogError("待機時間の配列とオーディオクリップの配列の長さが一致しません");
            return;
        }

        // 各オーディオクリップの再生を独立して開始
        for (int i = 0; i < this.audioClips.Length; i++)
        {
            AudioClip clip = audioClips[i];
            if (clip != null) {
                this.audioSource.volume = this.volumes[i];
                StartCoroutine(PlayAudioClipWithWait(i, clip));
                StartCoroutine(StopAudioClipWait(i));
            }
        }
    }

    private IEnumerator PlayAudioClipWithWait(int index, AudioClip clip)
    {
        // 待機時間の経過を待つ
        yield return new WaitForSeconds(this.beforeWaitTimes[index]);

        // AudioSourceを使ってoneshotでクリップを再生
        this.audioSource.PlayOneShot(clip);
    }

    private IEnumerator StopAudioClipWait(int index)
    {
        yield return new WaitForSeconds(this.afterwaitTimes[index]);

        this.audioSource.Stop();
    }
}
