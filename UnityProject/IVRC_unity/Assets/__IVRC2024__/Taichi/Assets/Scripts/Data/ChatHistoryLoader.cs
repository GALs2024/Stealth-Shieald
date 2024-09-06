using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json; // Newtonsoft.Json package をインポートしてください
using System.IO;

// JSON構造に対応するクラス
[System.Serializable]
public class ChatEntry
{
    public string Timestamp;
    public string UserInput;
    public string AIResponse;
}

[System.Serializable]
public class ChatData
{
    public List<ChatEntry> Entries;
}

public class ChatHistoryLoader : MonoBehaviour
{
    public string jsonFileName = "chatHistory.json";  // JSONファイル名
    private string fullChatHistory;  // 会話履歴を保持する変数

    private ChatData chatData;  // デシリアライズされたデータを保持する変数

    // Start is called before the first frame update
    void Start()
    {
        // JSONデータをロード
        LoadChatHistory();
        // 会話履歴を文字列にまとめる
        StoreChatHistory();
    }

    // JSONファイルを読み込み、デシリアライズする関数
    void LoadChatHistory()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, jsonFileName);

        if (File.Exists(filePath))
        {
            string jsonContent = File.ReadAllText(filePath);
            chatData = JsonConvert.DeserializeObject<ChatData>(jsonContent);
        }
        else
        {
            Debug.LogError("JSONファイルが見つかりません: " + filePath);
        }
    }

    // 会話履歴を1つの文字列にまとめて保持する関数
    void StoreChatHistory()
    {
        if (chatData == null)
        {
            fullChatHistory = "会話データがありません。";
            return;
        }

        fullChatHistory = "";  // 初期化

        foreach (ChatEntry entry in chatData.Entries)
        {
            fullChatHistory += $"{entry.UserInput} {entry.AIResponse} ";
        }

        // ここで `fullChatHistory` に全ての会話履歴が保存されています
        Debug.Log(fullChatHistory);  // デバッグログに出力（オプション）
    }

    // 会話履歴を取得する関数（外部からアクセス可能にする場合）
    public string GetChatHistory()
    {
        return fullChatHistory;
    }
}
