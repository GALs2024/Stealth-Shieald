using System.Collections;
using UnityEngine;
using TMPro;

public class TextMeshTyper : MonoBehaviour
{
    // 3Dオブジェクト用のTextMeshPro
    public TextMeshPro textObject;

    // 文字を表示する間隔（秒）
    public float typingSpeed = 0.05f;

    // 表示するテキストの内容
    private string textToDisplay = "Sample Text";

    public void StartTyping()
    {
        StartCoroutine(TypeText(textObject, this.textToDisplay));
    }

    public void SetText(string text)
    {
        this.textToDisplay = text;
    }

    // テキストを一文字ずつ表示するコルーチン
    IEnumerator TypeText(TextMeshPro textObject, string textToDisplay)
    {
        textObject.text = ""; // 最初にテキストをクリア
        foreach (char letter in textToDisplay.ToCharArray())
        {
            textObject.text += letter; // 一文字ずつ追加
            yield return new WaitForSeconds(typingSpeed); // 指定した時間待つ
        }
    }

    public void ResetText()
    {
        this.textObject.text = "";
    }
}
