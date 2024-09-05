using UnityEngine;
using TMPro; // TextMeshProを使用するために必要
using System.Collections;

public class TextTyper : MonoBehaviour
{
    // Canvas上のTextMeshPro (TMP) テキストフィールドへの参照を設定
    public TMP_Text textMeshPro1;
    public TMP_Text textMeshPro2;

    // 文字を表示する間隔（秒）
    public float typingSpeed = 0.05f;

    // 表示するテキストの内容
    private string textToDisplay1 = "これはCanvas 1のテキストです";
    private string textToDisplay2 = "これはCanvas 1のテキストです";

    void Start()
    {
        // コルーチンを使ってテキストを一文字ずつ表示
        if (textMeshPro1 != null)
        {
            StartCoroutine(TypeText(textMeshPro1, textToDisplay1));
        }

        if (textMeshPro2 != null)
        {
            StartCoroutine(TypeText(textMeshPro2, textToDisplay2));
        }
    }

    // テキストを一文字ずつ表示するコルーチン
    IEnumerator TypeText(TMP_Text textMeshPro, string textToDisplay)
    {
        textMeshPro.text = ""; // 最初にテキストをクリア
        foreach (char letter in textToDisplay.ToCharArray())
        {
            textMeshPro.text += letter; // 一文字ずつ追加
            yield return new WaitForSeconds(typingSpeed); // 指定した時間待つ
        }
    }
}
