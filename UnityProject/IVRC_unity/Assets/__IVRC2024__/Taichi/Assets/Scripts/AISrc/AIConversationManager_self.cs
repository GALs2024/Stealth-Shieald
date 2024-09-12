using System.Collections;
using System.IO;
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
    public AIMovieManager aiMovieManager;
    public GenFakeNews genFakeNews;
    public GenImgFromConversation genImgFromConversation;

    void Start()
    {
        string path = Path.Combine(Application.dataPath, @"__IVRC2024__/Taichi/Assets/Audio/User");
        WavUtility.DeleteAllFilesInDirectory(path);
    }

    public void StartConversation()
    {
        if (this.conversationalAI == null)
        {
            Debug.LogError("ConversationalAI script is not assigned.");
            return;
        }

        this.conversationalAI.Initialize();
        this.conversationalAI.ResetConversationHistory();
        NextConversation();
    }

    private void NextConversation()
    {
        if (this.currentMassageIndex < this.AISystemMessage.Length)
        {   
            this.conversationalAI.StartConversation(this.AISystemMessage[this.currentMassageIndex], "");
            this.conversationalAI.ResetMicInputCount();
            this.currentMassageIndex++;
        }
        else
        {
            Debug.Log("All audio clips have been played. Ending conversation.");
            // 必要なら会話終了後の処理をここに追加
            this.genFakeNews.Generate();
            this.genImgFromConversation.Generate();
            this.aiMovieManager.StartMovie();
        }
    }

    // このメソッドはConversationalAIから呼び出されます
    public void NextAction()
    {
        NextConversation(); // 次の音声ファイルを再生
    }

    public void Reset3DText(bool is_ai)
    {
        if (is_ai)
        {
            this.AITextMeshTyper.ResetText();
        }
        else
        {
            this.UserTextMeshTyper.ResetText();
        }
    }

    public void Set3DText(bool is_ai, string text)
    {
        if (is_ai)
        {
            this.AITextMeshTyper.SetText(text);
        }
        else
        {
            this.UserTextMeshTyper.SetText(text);
        }
    }

    public void Display3DText(bool is_ai)
    {
        if (is_ai)
        {
            this.AITextMeshTyper.StartTyping();
        }
        else
        {
            this.UserTextMeshTyper.StartTyping();
        }
    }
}
