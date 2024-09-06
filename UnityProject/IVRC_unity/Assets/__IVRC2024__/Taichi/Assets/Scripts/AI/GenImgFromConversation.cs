using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenImgFromConversation : MonoBehaviour
{
    private string prompt; // 画像生成のプロンプトを設定
    private OpenAIDalle openAIDalle; // OpenAIDalleクラスのインスタンス
    private ChatHistoryLoader chatHistoryLoader; // ChatHistoryLoaderクラスのインスタンス
    public RawImage displayImage; // UIで表示するためのRawImageコンポーネント

    void Start()
    {
        string apiKey = ApiKeyLoader.LoadApiKey();
        
        // OpenAIDalleクラスのインスタンスを初期化
        openAIDalle = new OpenAIDalle(apiKey, "dall-e-2", 1024);
        
        chatHistoryLoader = new ChatHistoryLoader();

        prompt = "この文章に関連した画像を生成して: " + chatHistoryLoader.GetChatHistory();
        Debug.Log("Prompt: " + prompt);

        string savePath = @"C:\Users\EM\Desktop\Stealth-Shieald\UnityProject\IVRC_unity\Assets\__IVRC2024__\Taichi\Assets\" + "generated_img.jpg"; // 保存先のパスを設定

        // 画像生成プロセスを開始
        StartCoroutine(openAIDalle.RequestDalleImage(prompt, savePath, OnImageReceived, OnError));
    }

    private void OnImageReceived(string message) // string型の引数を取るようにする
    {
        Debug.Log("Success: " + message);
    }

    private void OnError(string error) // string型の引数を取るようにする
    {
        Debug.LogError("Error: " + error);
    }
}
