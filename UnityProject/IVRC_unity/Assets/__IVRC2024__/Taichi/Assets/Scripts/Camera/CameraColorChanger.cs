using UnityEngine;

public class CameraColorChanger : MonoBehaviour
{
    public Camera targetCamera;  // 背景色を変更するカメラ
    public Color startColor = Color.white;  // 開始時の背景色
    public Color endColor = Color.black;    // 終了時の背景色
    public float duration = 2.0f;  // 色を変更する時間

    private float timeElapsed = 0.0f;
    private bool isChanging = false;

    void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;  // メインカメラをデフォルトに設定
        }
    }

    void Update()
    {
        if (isChanging)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / duration);
            targetCamera.backgroundColor = Color.Lerp(startColor, endColor, t);

            if (t >= 1.0f)
            {
                isChanging = false;  // 終了時には変更を停止
            }
        }
    }

    public void StartColorChange()
    {
        timeElapsed = 0.0f;
        isChanging = true;
    }
}
