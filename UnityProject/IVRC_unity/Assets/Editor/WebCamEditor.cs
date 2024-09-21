using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CustomEditor(typeof(CaptureWebCam))]
public class WebCamEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CaptureWebCam captureWebCam = (CaptureWebCam)target;

        // 接続されているカメラデバイスのリストを取得
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            EditorGUILayout.HelpBox("Webカメラが接続されていません。", MessageType.Warning);
            return;
        }

        // カメラデバイス名のリストを作成
        List<string> cameraNames = new List<string>();
        foreach (var device in devices)
        {
            cameraNames.Add(device.name);
        }

        // 選択中のカメラのインデックスを取得
        int selectedIndex = cameraNames.IndexOf(captureWebCam.SelectedCameraName);
        if (selectedIndex == -1) selectedIndex = 0; // デフォルトで最初のカメラを選択

        // プルダウンメニューを表示してカメラを選択
        selectedIndex = EditorGUILayout.Popup("Select Camera", selectedIndex, cameraNames.ToArray());

        // 選択されたカメラの名前を更新
        captureWebCam.SelectedCameraName = cameraNames[selectedIndex];

        // 通常のInspectorの描画
        DrawDefaultInspector();
    }
}
