using System.Collections;
using UnityEngine;

public class AIConversationManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] audioClips; // 複数の音声ファイルを設定
    [SerializeField]
    private ConversationalAI conversationalAI;
    [SerializeField]
    private Sample sample;

    private int currentClipIndex = 0;

    void Start()
    {
        if (audioClips.Length == 0)
        {
            Debug.LogError("No audio clips assigned.");
            return;
        }

        if (conversationalAI == null)
        {
            Debug.LogError("ConversationalAI script is not assigned.");
            return;
        }

        conversationalAI.Initialize();
        
        PlayNextAudioClip();
    }

    private void PlayNextAudioClip()
    {
        if (currentClipIndex < audioClips.Length)
        {
            StartCoroutine(DelayedPlayNextAudioClip());

            IEnumerator DelayedPlayNextAudioClip()
            {
                yield return new WaitForSeconds(1f);
                conversationalAI.SetStartAudioClip(audioClips[currentClipIndex]);
                conversationalAI.PlayStartAudio(); // ConversationalAIの音声再生メソッドを呼び出し
                currentClipIndex++;
            }
        }
        else
        {
            Debug.Log("All audio clips have been played. Ending conversation.");
            // 必要なら会話終了後の処理をここに追加
            sample.play();
        }
    }

    // このメソッドはConversationalAIから呼び出されます
    public void OnAudioFinished()
    {
        PlayNextAudioClip(); // 次の音声ファイルを再生
    }
}
