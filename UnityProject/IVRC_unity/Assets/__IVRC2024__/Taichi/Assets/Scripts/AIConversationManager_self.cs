using System.Collections;
using UnityEngine;

public class AIConversationManager_self : MonoBehaviour
{
    [SerializeField]
    private ConversationalAI_self conversationalAI;
    [SerializeField]
    public string[] AISystemMessage;
    private int currentMassageIndex = 0;
    public TextMeshTyper AITextMeshTyper;
    public TextMeshTyper UserTextMeshTyper; 

    void Start()
    {
        if (this.conversationalAI == null)
        {
            Debug.LogError("ConversationalAI script is not assigned.");
            return;
        }

        this.conversationalAI.Initialize();
        NextConversation();
    }

    private void NextConversation()
    {
        if (this.currentMassageIndex < this.AISystemMessage.Length)
        {   
            this.conversationalAI.StartConversation(this.AISystemMessage[this.currentMassageIndex], "");
            this.currentMassageIndex++;
        }
        else
        {
            Debug.Log("All audio clips have been played. Ending conversation.");
            // 必要なら会話終了後の処理をここに追加
            // sample.play();
        }
    }

    // このメソッドはConversationalAIから呼び出されます
    public void NextAction()
    {
        NextConversation(); // 次の音声ファイルを再生
    }

    public void Display3DText(bool is_ai, string text)
    {
        if (is_ai)
        {
            this.AITextMeshTyper.StartTyping(text);
        }
        else
        {
            this.UserTextMeshTyper.StartTyping(text);
        }
    }
}
