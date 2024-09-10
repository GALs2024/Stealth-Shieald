using System.Collections;
using UnityEngine;

public class VoicePlayer : MonoBehaviour
{
    public AudioClip[] audioClips;        // 再生するオーディオクリップの配列
    public float[] beforeWaitTimes;       // 再生前の待機時間の配列
    public float[] afterWaitTimes;        // 再生後の待機時間の配列
    public float[] volumes;               // 各クリップの音量の配列
    public int continuePlayingClipIndex = -1;  // 次のシーンでも再生を続けたいクリップのインデックス (-1は指定なし)

    private static VoicePlayer instance;  // シングルトンのインスタンス
    private AudioSource persistentAudioSource; // 次のシーンでも再生されるAudioSource

    private void Awake()
    {
        // シングルトンパターンでこのオブジェクトが重複しないようにする
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // このオブジェクトを次のシーンでも破棄しない
        }
    }

    private void Start()
    {
        // 全ての配列が正しい長さを持っているかチェック
        if (audioClips == null || beforeWaitTimes == null || afterWaitTimes == null || volumes == null)
        {
            Debug.LogError("いずれかの配列が未設定です");
            return;
        }

        if (audioClips.Length != beforeWaitTimes.Length || audioClips.Length != afterWaitTimes.Length || audioClips.Length != volumes.Length)
        {
            Debug.LogError("配列の長さが一致しません");
            return;
        }

        // continuePlayingClipIndexが正しい範囲にあるか確認
        if (continuePlayingClipIndex >= audioClips.Length || continuePlayingClipIndex < -1)
        {
            Debug.LogError("continuePlayingClipIndexが無効です");
            return;
        }

        // 各オーディオクリップの再生を独立して開始
        for (int i = 0; i < audioClips.Length; i++)
        {
            AudioClip clip = audioClips[i];
            if (clip != null)
            {
                // 特定のクリップは次のシーンに移行しても再生し続ける
                if (i == continuePlayingClipIndex)
                {
                    StartCoroutine(PlayPersistentAudioClip(i, clip));
                }
                else
                {
                    StartCoroutine(PlayAudioClipWithWait(i, clip));
                }
            }
        }
    }

    private IEnumerator PlayAudioClipWithWait(int index, AudioClip clip)
    {
        // 再生前の待機時間
        yield return new WaitForSeconds(beforeWaitTimes[index]);

        // 新しいGameObjectを作成し、AudioSourceを追加
        GameObject audioObject = new GameObject("AudioSource_" + index);
        AudioSource newAudioSource = audioObject.AddComponent<AudioSource>();

        // AudioSourceの設定（ボリュームなど）
        newAudioSource.clip = clip;
        newAudioSource.volume = volumes[index];
        newAudioSource.loop = false; // ループしない
        newAudioSource.Play();

        // 再生後の待機時間を経過させる
        yield return new WaitForSeconds(clip.length + afterWaitTimes[index]);

        // 再生が終了したらAudioSourceを破棄
        Destroy(audioObject);
    }

    // 次のシーンに移行しても再生を続けるクリップの再生
    private IEnumerator PlayPersistentAudioClip(int index, AudioClip clip)
    {
        // 再生前の待機時間
        yield return new WaitForSeconds(beforeWaitTimes[index]);

        // PersistentAudioSourceがまだ存在しない場合のみ作成
        if (persistentAudioSource == null)
        {
            persistentAudioSource = gameObject.AddComponent<AudioSource>();

            // AudioSourceの設定（ボリュームなど）
            persistentAudioSource.clip = clip;
            persistentAudioSource.volume = volumes[index];
            persistentAudioSource.loop = false; // ループしない
            persistentAudioSource.Play();
        }

        // 再生終了後、指定した時間を待って停止する
        yield return new WaitForSeconds(clip.length + afterWaitTimes[index]);

        // 再生終了後、AudioSourceを停止
        persistentAudioSource.Stop();
    }
}
