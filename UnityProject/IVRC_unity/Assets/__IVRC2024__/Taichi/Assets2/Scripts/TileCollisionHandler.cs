using UnityEngine;

public class TileCollisionHandler : MonoBehaviour
{
    // 衝突時の処理
    private void OnCollisionEnter(Collision collision)
    {
        // 2秒後にタイルを削除
        Debug.Log("Collision detected with: " + collision.gameObject.name); // デバッグ用
        Destroy(gameObject, 2f);
    }
}
