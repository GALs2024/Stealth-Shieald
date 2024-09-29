using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class stopobject : MonoBehaviour
{
    // 手に触れたときに振動を停止するためのメソッド
    private void OnTriggerEnter(Collider collider)
    {
        // ハンドトラッキングされた手かどうかを確認
        if (collider.CompareTag("Hand"))
        {
            // 振動オブジェクトのスクリプトを取得
            VibratingObject vibratingObject = GetComponent<VibratingObject>();
            if (vibratingObject != null)
            {
                // 振動を停止する
                vibratingObject.StopVibration();
            }
        }
    }
}