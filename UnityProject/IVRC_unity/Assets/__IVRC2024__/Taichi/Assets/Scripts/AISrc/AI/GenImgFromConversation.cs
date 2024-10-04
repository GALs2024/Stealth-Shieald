using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GenImgFromConversation : MonoBehaviour
{
    private string prompt; // 画像生成のプロンプトを設定
    private OpenAIDalle openAIDalle; // OpenAIDalleクラスのインスタンス
    private ChatHistoryLoader chatHistoryLoader; // ChatHistoryLoaderクラスのインスタンス
    private string outputPath = @"__IVRC2024__/Taichi/Assets/Textures"; // 画像の保存先のパス

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        string apiKey = ApiKeyLoader.LoadApiKey();
        // OpenAIDalleクラスのインスタンスを初期化
        this.openAIDalle = new OpenAIDalle(apiKey, "dall-e-2", 1024);

        this.chatHistoryLoader = new ChatHistoryLoader();
    }

    public void Generate()
    {
        this.prompt = "このAIとの会話からキーワードだけ抜き出して画像を生成して: " + this.chatHistoryLoader.GetChatHistory();
        Debug.Log("Img generating prompt: " + this.prompt);

        string savePath = Path.Combine(Application.dataPath, this.outputPath, "generated_img.jpg"); // 保存先のパスを設定

        // 画像生成プロセスを開始
        StartCoroutine(openAIDalle.RequestDalleImage(this.prompt, savePath, OnImageReceived, OnError));
    }

    private void OnImageReceived(string message)
    {
        Debug.Log("Success: " + message);
    }

    private void OnError(string error)
    {
        Debug.LogError("Error: " + error);
    }
}
