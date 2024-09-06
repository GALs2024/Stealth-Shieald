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

public class ChatHistoryLoader
{
    private string jsonFileName = "Assets/__IVRC2024__/Taichi/Assets/Data/conversation_history.json";  // JSONファイル名
    private string fullChatHistory;  // 会話履歴を保持する変数

    private ChatData chatData;  // デシリアライズされたデータを保持する変数

    // JSONファイルを読み込み、デシリアライズする関数
    public void LoadChatHistory()
    {
        if (File.Exists(this.jsonFileName))
        {
            string jsonContent = File.ReadAllText(this.jsonFileName);
            chatData = JsonConvert.DeserializeObject<ChatData>(jsonContent);
            StoreChatHistory(); // デシリアライズが成功した場合に履歴を格納
        }
        else
        {
            Debug.LogError("JSONファイルが見つかりません: " + this.jsonFileName);
            fullChatHistory = "会話データがありません。";  // エラーが発生した場合にデフォルトのメッセージ
        }
    }

    // 会話履歴を1つの文字列にまとめて保持する関数
    private void StoreChatHistory()
    {
        if (chatData == null)
        {
            fullChatHistory = "会話データがありません。";
            return;
        }

        fullChatHistory = "";  // 初期化

        foreach (ChatEntry entry in chatData.Entries)
        {
            fullChatHistory += $"User: {entry.UserInput} AI: {entry.AIResponse} ";
        }
    }

    // 会話履歴を取得する関数（外部からアクセス可能）
    public string GetChatHistory()
    {
        // 履歴がない場合はロードする
        if (string.IsNullOrEmpty(fullChatHistory))
        {
            LoadChatHistory();  // 会話履歴がまだロードされていない場合はロード
        }
        return fullChatHistory;
    }
}
