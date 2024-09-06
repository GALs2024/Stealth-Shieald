using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenFakeNews : MonoBehaviour
{
    private OpenAIChat openAIChat;
    private string apiKey;
    private string systemMessage;

    // Start is called before the first frame update
    void Start()
    {
        this.apiKey = ApiKeyLoader.LoadApiKey();
        this.systemMessage = "# 命令\nUser特徴の含めて、架空のテロニュースの原稿を3種類（100文字以内ずつ）作成してください。\n# 出力json形式\n[\n  {\n    \"details\": \"\"\n  },\n   {\n    \"details\": \"\"\n  },\n  {\n    \"details\": \"\"\n  }\n]";
        this.openAIChat = new OpenAIChat(this.apiKey, "gpt-4o-mini", this.systemMessage, 100);
    }

    public void Generate(string targetContents)
    {
        string userInput = "# 会話内容\n" + targetContents;
        StartCoroutine(this.openAIChat.RequestChatResponse(this.systemMessage, userInput, OnSuccess, OnError));
    }

    private void OnSuccess(string message)
    {
        Debug.Log(message);
    }

    private void OnError(string error)
    {
        Debug.LogError(error);
    }
}
