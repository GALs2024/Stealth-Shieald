using System.Collections;
using TMPro;
using UnityEngine;

public static class TMPUtil
{
    // テキストの内容を設定するユーティリティ関数
    public static void SetText(TMP_Text textMeshPro, string newText)
    {
        if (textMeshPro == null) return;
        textMeshPro.text = newText;
    }

    // テキストの色を設定するユーティリティ関数
    public static void SetTextColor(TMP_Text textMeshPro, Color newColor)
    {
        if (textMeshPro == null) return;
        textMeshPro.color = newColor;
    }

    // テキストのサイズを設定するユーティリティ関数
    public static void SetTextSize(TMP_Text textMeshPro, float newSize)
    {
        if (textMeshPro == null) return;
        textMeshPro.fontSize = newSize;
    }

    // 指定した時間ごとに一文字ずつ表示するコルーチン
    public static IEnumerator TypeText(TMP_Text textMeshPro, string newText, float delay)
    {
        if (textMeshPro == null) yield break;

        textMeshPro.text = "";

        // 一文字ずつ表示
        foreach (char letter in newText.ToCharArray())
        {
            textMeshPro.text += letter;
            yield return new WaitForSeconds(delay);
        }
    }
}
