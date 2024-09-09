using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ConversationHistoryManager
{
    [Serializable]
    public class ConversationEntry
    {
        public string Timestamp;
        public string UserInput;
        public string AIResponse;
    }

    [Serializable]
    public class ConversationHistory
    {
        public List<ConversationEntry> Entries = new List<ConversationEntry>();
    }

    private ConversationHistory conversationHistory = new ConversationHistory();
    private string conversationHistoryPath = @"Assets/__IVRC2024__/Taichi/Assets/Data/conversation_history.json";

    public ConversationHistoryManager()
    {
        LoadConversationHistory();
    }

    public void SaveConversationHistory(string userInput, string aiResponse)
    {
        var conversation = new ConversationEntry
        {
            Timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss"),
            UserInput = userInput,
            AIResponse = aiResponse
        };

        // 会話履歴に新しいエントリを追加
        conversationHistory.Entries.Add(conversation);

        // 会話履歴全体をJSONとして保存
        string conversationJson = JsonUtility.ToJson(conversationHistory, true);

        try
        {
            File.WriteAllText(conversationHistoryPath, conversationJson);
            Debug.Log("Conversation saved to: " + conversationHistoryPath);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save conversation: " + e.Message);
        }
    }

    public void LoadConversationHistory()
    {
        try
        {
            if (File.Exists(conversationHistoryPath))
            {
                string json = File.ReadAllText(conversationHistoryPath);
                conversationHistory = JsonUtility.FromJson<ConversationHistory>(json);
                
                if (conversationHistory.Entries == null)
                {
                    conversationHistory.Entries = new List<ConversationEntry>();
                }
                
                Debug.Log("Conversation history loaded.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load conversation history: " + e.Message);
        }
    }

    public void ClearConversationHistory()
    {
        // 会話履歴を空にする
        conversationHistory.Entries.Clear();

        // 空の会話履歴をJSONとして保存
        string conversationJson = JsonUtility.ToJson(conversationHistory, true);

        try
        {
            File.WriteAllText(conversationHistoryPath, conversationJson);
            Debug.Log("Conversation history cleared.");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to clear conversation history: " + e.Message);
        }
    }

    public List<string> GetConversationHistoryAsString()
    {
        List<string> history = new List<string>();

        if (conversationHistory != null && conversationHistory.Entries != null)
        {
            foreach (var entry in conversationHistory.Entries)
            {
                history.Add($"User: {entry.UserInput}");
                history.Add($"AI: {entry.AIResponse}");
            }
        }

        return history;
    }
}
