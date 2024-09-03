using UnityEngine;

public class ImagePlacer : MonoBehaviour
{
    public Texture2D imageTexture;  // 設定する画像
    public float zPosition = 0f;    // 奥行きの位置
    public float imageScale = 1f;   // 画像のスケール
    public float appearDistance = 10f; // 表示される距離

    private GameObject imagePlane;  // 生成された画像オブジェクトの参照

    // 画像を配置するメソッド
    public void PlaceImageAtCenter()
    {
        // カメラの中央座標を取得
        Camera cam = Camera.main;
        Vector3 cameraCenter = cam.transform.position;

        // 画像を表示するための平面オブジェクトを作成
        imagePlane = GameObject.CreatePrimitive(PrimitiveType.Quad);
        imagePlane.transform.position = new Vector3(cameraCenter.x, cameraCenter.y, zPosition);

        // 画像のスケールを設定
        float imageWidth = imageTexture.width / 100f * imageScale;
        float imageHeight = imageTexture.height / 100f * imageScale;
        imagePlane.transform.localScale = new Vector3(imageWidth, imageHeight, 1f);

        // 画像をテクスチャとして適用
        Material material = new Material(Shader.Find("Unlit/Texture"));
        material.mainTexture = imageTexture;
        imagePlane.GetComponent<Renderer>().material = material;

        // 初期状態では画像を非表示にする
        imagePlane.SetActive(false);
    }

    void Update()
    {
        // カメラと画像の距離をチェック
        if (imagePlane != null)
        {
            float distance = Vector3.Distance(Camera.main.transform.position, imagePlane.transform.position);

            // 指定した距離以内に近づいたら画像を表示、遠ざかったら非表示にする
            if (distance <= appearDistance)
            {
                imagePlane.SetActive(true);  // 画像を表示する
            }
            else
            {
                imagePlane.SetActive(false);  // 画像を非表示にする
            }
        }
    }

    void Start()
    {
        // カメラ中央に画像を配置
        PlaceImageAtCenter();
    }
}
