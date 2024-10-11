using System.Collections;
using UnityEngine;
using TMPro;

public class ThinkingDotsTyper : MonoBehaviour
{
    // 3Dオブジェクト用のTextMeshPro
    public TextMeshPro textObject;

    // 文字を表示する間隔（秒）
    public float typingSpeed = 0.5f;

    public bool isEnglishMode = false;
    private string baseText = "もう一度話してください。";

    private int dotCount = 0;  // 現在のドットの数

    // 現在のコルーチンを保持するための変数
    private Coroutine typingCoroutine;

    // ドットが増えるコルーチンを開始
    public void StartTyping()
    {
        if (this.isEnglishMode)
        {
            this.baseText = "Please speak into the microphone.";
        }
        if (typingCoroutine == null)
        {
            typingCoroutine = StartCoroutine(UpdateDots());
        }
    }

    // ドットが増えるコルーチン
    private IEnumerator UpdateDots()
    {
        while (true)
        {
            dotCount = (dotCount + 1) % 4;  // ドットの数を0〜3でループ
            string currentText = baseText + new string('.', dotCount);  // ドットを追加
            textObject.text = currentText;  // テキストを即座に表示（アニメーションなしの場合）
            yield return new WaitForSeconds(typingSpeed);  // 指定時間待つ
        }
    }

    // 表示を停止してテキストをクリア
    public void StopTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);  // コルーチンを停止
            typingCoroutine = null;
        }

        ResetText();  // テキストをクリア
    }

    // テキストをクリア
    public void ResetText()
    {
        textObject.text = "";  // テキストを消す
    }
}
