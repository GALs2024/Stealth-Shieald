using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NamedTextController : MonoBehaviour
{
    private Dictionary<string, TMP_Text> _textMeshProDictionary;

    [System.Serializable]
    public struct TextEntry
    {
        public string Key;
        public TMP_Text TextObject;
    }

    public List<TextEntry> TextEntries;

    private void Awake()
    {
        this._textMeshProDictionary = new Dictionary<string, TMP_Text>();

        foreach (TextEntry entry in this.TextEntries)
        {
            if (!this._textMeshProDictionary.ContainsKey(entry.Key))
            {
                this._textMeshProDictionary.Add(entry.Key, entry.TextObject);
            }
        }
    }

    public void SetText(string key, string newText)
    {
        if (this._textMeshProDictionary.ContainsKey(key))
        {
            TMPUtil.SetText(this._textMeshProDictionary[key], newText);
        }
        else
        {
            Debug.LogError($"Text key '{key}' not found.");
        }
    }

    public void ClearText(string key)
    {
        if (this._textMeshProDictionary.ContainsKey(key))
        {
            TMPUtil.SetText(this._textMeshProDictionary[key], string.Empty);
        }
    }

    public void SetTextWithDelay(string key, string newText, float delay)
    {
        if (this._textMeshProDictionary.ContainsKey(key))
        {
            StartCoroutine(TMPUtil.TypeText(this._textMeshProDictionary[key], newText, delay));
        }
    }

    public void SetTextColor(string key, Color newColor)
    {
        if (this._textMeshProDictionary.ContainsKey(key))
        {
            TMPUtil.SetTextColor(this._textMeshProDictionary[key], newColor);
        }
    }

    public void SetTextSize(string key, float newSize)
    {
        if (this._textMeshProDictionary.ContainsKey(key))
        {
            TMPUtil.SetTextSize(this._textMeshProDictionary[key], newSize);
        }
    }
}
