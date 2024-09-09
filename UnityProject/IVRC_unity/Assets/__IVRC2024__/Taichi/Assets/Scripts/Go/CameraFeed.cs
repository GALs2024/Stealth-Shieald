using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFeed : MonoBehaviour
{
    public Material displayMaterial; // Webカメラ映像を表示するためのマテリアル
    private WebCamTexture webCamTexture;
    private WebCamDevice[] devices; // 接続されているWebカメラのリスト
    public int cameraIndex = 0; // 使用するカメラのインデックス

    void Start()
    {
        // 接続されているWebカメラのリストを取得
        devices = WebCamTexture.devices;

        // カメラが存在するか確認
        if (devices.Length > 0)
        {
            // デフォルトではcameraIndexのカメラを使用
            StartCamera(cameraIndex);
        }
        else
        {
            Debug.LogWarning("Webカメラが接続されていません。");
        }
    }

    // 特定のカメラを開始する
    public void StartCamera(int index)
    {
        if (webCamTexture != null)
        {
            // 既にカメラが起動している場合は停止
            webCamTexture.Stop();
        }

        if (index >= 0 && index < devices.Length)
        {
            // 指定されたカメラデバイスを選択
            webCamTexture = new WebCamTexture(devices[index].name);

            // マテリアルにWebカメラ映像を設定
            displayMaterial.mainTexture = webCamTexture;

            // Webカメラの映像を開始
            webCamTexture.Play();
        }
        else
        {
            Debug.LogWarning("指定されたインデックスのカメラは存在しません。");
        }
    }

    // 次のカメラに切り替える関数
    public void SwitchCamera()
    {
        cameraIndex = (cameraIndex + 1) % devices.Length;
        StartCamera(cameraIndex);
    }

    void OnDestroy()
    {
        // Webカメラの映像を停止
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
        }
    }
}
