using UnityEngine;

public class MoveObjectWithOffset : MonoBehaviour
{
    // 移動させたいオブジェクト
    public GameObject targetObject;

    // カメラの中央からの相対的なオフセット（X, Y, Z）
    public Vector3 offset;

    void Start()
    {
        // MainCameraタグがついているカメラを取得
        Camera mainCamera = Camera.main;

        if (mainCamera != null && targetObject != null)
        {
            // カメラのスクリーンの中央座標を取得
            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, mainCamera.nearClipPlane);

            // スクリーン座標をワールド座標に変換
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenCenter);

            // オフセットを適用
            worldPosition += offset;

            // オブジェクトの位置を設定
            targetObject.transform.position = worldPosition;
        }
        else
        {
            Debug.LogError("MainCamera or targetObject is missing.");
        }
    }
}
