using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// オブジェクトに触れたときに振動を停止するスクリプト
public class StopVibrationOnTouch : MonoBehaviour

{
    // コライダーに触れたときに呼ばれるメソッド
    private void OnTriggerEnter(Collider collider)
    {
        // 触れたオブジェクトにVibratingObjectスクリプトがアタッチされているか確認
        VibratingObject vibratingObject = collider.GetComponent<VibratingObject>();
        if (vibratingObject != null)
        {
            // 振動を停止する
            vibratingObject.StopVibration();
        }
    }
}
