using System.Collections;
using UnityEngine;

public class CaptureWebCam : MonoBehaviour
{
    [SerializeField]
    private Material displayMaterial; // Webカメラ映像を表示するためのマテリアル

    private WebCamTexture _webCamTexture;

    private string selectedCameraName;

    public string SelectedCameraName
    {
        get { return selectedCameraName; }
        set { selectedCameraName = value; }
    }

    void Start()
    {
        if (selectedCameraName != null)
        {
            StartCamera(selectedCameraName);
        }
        else
        {
            Debug.LogWarning("Webカメラが接続されていません。");
        }
    }

    public void StartCamera(string cameraName)
    {
        if (_webCamTexture != null)
        {
            _webCamTexture.Stop();
        }

        try
        {
            _webCamTexture = new WebCamTexture(cameraName);
            displayMaterial.mainTexture = _webCamTexture;
            _webCamTexture.Play();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"カメラの起動に失敗しました: {ex.Message}");
        }
    }

    void OnDestroy()
    {
        if (_webCamTexture != null)
        {
            _webCamTexture.Stop();
        }
    }
}
