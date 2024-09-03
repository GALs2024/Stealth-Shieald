using UnityEngine;

public class CameraController : MonoBehaviour
{
    // カメラの移動速度
    public float speed = -1.0f;

    // 初期位置
    private Vector3 startPosition;

    void Start()
    {
        // カメラの初期位置を記録
        startPosition = transform.position;
    }

    void Update()
    {
        // Z軸のみを徐々に増加させる
        float newZ = transform.position.z + speed * Time.deltaTime;
        
        // xとyはそのまま、zのみを変更
        transform.position = new Vector3(startPosition.x, startPosition.y, newZ);
    }
}
