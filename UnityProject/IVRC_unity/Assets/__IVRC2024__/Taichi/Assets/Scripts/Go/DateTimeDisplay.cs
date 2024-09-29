using System;
using UnityEngine;
using TMPro; // TextMeshProを使用するために必要
using System.Collections; // IEnumeratorを使用するために必要

public class DateTimeDisplay : MonoBehaviour
{
    public TextMeshProUGUI dateTimeText;  // TextMeshProのUIオブジェクト
    private string dateTimeFormat = "yyyy/MM/dd HH:mm:ss";  // フォーマットを指定

    void Start()
    {
        if (dateTimeText == null)
        {
            Debug.LogError("TextMeshProUGUIオブジェクトが設定されていません。");
            return;
        }

        // コルーチンで1秒毎に時間を更新
        StartCoroutine(UpdateDateTime());
    }

    IEnumerator UpdateDateTime()
    {
        while (true)
        {
            // 現在の時間と日付を取得し、フォーマット
            dateTimeText.text = DateTime.Now.ToString(dateTimeFormat);

            // 1秒待ってから次に更新
            yield return new WaitForSeconds(1f);
        }
    }
}
