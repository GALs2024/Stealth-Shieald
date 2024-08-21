using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonControl : MonoBehaviour
{
    public Button stopButton; // UIボタンの参照
    public VibratingObject vibratingObject; // 振動するオブジェクトの参照

    void Start()
    {
        // ボタンにクリックイベントを登録
        stopButton.onClick.AddListener(StopVibration);
    }

    void StopVibration()
    {
        // 振動を停止する
        if (vibratingObject != null)
        {
            vibratingObject.StopVibration();
        }
    }
}
