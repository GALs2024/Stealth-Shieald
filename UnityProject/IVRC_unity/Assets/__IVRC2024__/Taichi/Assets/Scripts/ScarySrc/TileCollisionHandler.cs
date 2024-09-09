using UnityEngine;

public class TileCollisionHandler : MonoBehaviour
{
    private static bool hasPlayedAudio = false; // 全てのタイルで一度だけ再生するために静的変数を使用
    private AudioSource vabrationAudioSource;

    void Start()
    {
        // シーンの "Vabration" オブジェクトを探して AudioSource を取得
        GameObject vabrationObject = GameObject.Find("Vabration");
        if (vabrationObject != null)
        {
            vabrationAudioSource = vabrationObject.GetComponent<AudioSource>();
        }
        else
        {
            Debug.LogError("Vabration オブジェクトが見つかりません！");
        }
    }

    // 衝突時の処理
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name); // デバッグ用

        // AudioSourceが存在し、まだ再生されていない場合に音声を再生
        if (vabrationAudioSource != null && !hasPlayedAudio && !vabrationAudioSource.isPlaying)
        {
            vabrationAudioSource.Play();
            hasPlayedAudio = true; // 再生フラグを設定（静的にすることで全タイル共通で管理）
        }

        // 2秒後にタイルを削除
        Destroy(gameObject, 2f);
    }
}
