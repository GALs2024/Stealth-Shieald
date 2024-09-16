using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NamedTextController : MonoBehaviour
{
    public Dictionary<string, TMP_Text> textMeshProDictionary;

    [System.Serializable]
    public struct TextEntry
    {
        public string key;
        public TMP_Text textObject;
    }

    public List<TextEntry> textEntries;

    void Awake()
    {
        textMeshProDictionary = new Dictionary<string, TMP_Text>();

        // リストを基にDictionaryにデータを登録
        foreach (var entry in textEntries)
        {
            if (!textMeshProDictionary.ContainsKey(entry.key))
            {
                textMeshProDictionary.Add(entry.key, entry.textObject);
            }
        }
    }

    // テキストを設定（名前で指定）
    public void SetText(string key, string newText)
    {
        if (textMeshProDictionary.ContainsKey(key))
        {
            TMPUtil.SetText(textMeshProDictionary[key], newText);
        }
        else
        {
            Debug.LogError($"テキストキー '{key}' が見つかりませんでした。");
        }
    }

    // 何も表示しない
    public void ClearText(string key)
    {
        if (textMeshProDictionary.ContainsKey(key))
        {
            TMPUtil.SetText(textMeshProDictionary[key], "");
        }
    }

    // 指定した時間で一文字ずつ表示
    public void SetTextWithDelay(string key, string newText, float delay)
    {
        if (textMeshProDictionary.ContainsKey(key))
        {
            StartCoroutine(TMPUtil.TypeText(textMeshProDictionary[key], newText, delay));
        }
    }

    // テキストの色を設定
    public void SetTextColor(string key, Color newColor)
    {
        if (textMeshProDictionary.ContainsKey(key))
        {
            TMPUtil.SetTextColor(textMeshProDictionary[key], newColor);
        }
    }

    // テキストのサイズを設定
    public void SetTextSize(string key, float newSize)
    {
        if (textMeshProDictionary.ContainsKey(key))
        {
            TMPUtil.SetTextSize(textMeshProDictionary[key], newSize);
        }
    }
}
