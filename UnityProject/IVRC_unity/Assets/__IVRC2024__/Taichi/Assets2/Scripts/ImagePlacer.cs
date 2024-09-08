using UnityEngine;
using System.IO;

public class ImagePlacer : MonoBehaviour
{
    private string imagesFolderPath = @"Assets/__IVRC2024__/Taichi/Assets2/Textures/Mosaic/Originals"; // ターゲット画像のファイルパス

    private Texture2D[] imageTextures;  // 設定する画像配列
    public float zPosition = 0f;    // 奥行きの位置
    public float imageScale = 1f;   // 画像のスケール
    public float appearDistance = 10f; // 表示される距離
    public int gridRows = 2; // グリッドの行数
    public int gridColumns = 3; // グリッドの列数

    private GameObject[] imagePlanes;  // 生成された画像オブジェクトの参照

    // 画像を配置するメソッド
    public void PlaceImagesInGrid()
    {
        // カメラの中央座標を取得
        Camera cam = Camera.main;
        Vector3 cameraCenter = cam.transform.position;

        // 画像ファイルをすべて取得
        string[] filePaths = Directory.GetFiles(imagesFolderPath, "*.png");
        imageTextures = new Texture2D[filePaths.Length];
        imagePlanes = new GameObject[filePaths.Length];

        // カメラの画角から必要な範囲を計算
        float camHeight = 2f * zPosition * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float camWidth = camHeight * cam.aspect;

        // 画像1枚あたりのサイズを計算 (カメラの幅と高さに基づいて均等に配置)
        float imageWidth = camWidth / gridColumns;
        float imageHeight = camHeight / gridRows;

        // 各画像の座標を計算して配置
        for (int i = 0; i < filePaths.Length; i++)
        {
            int row = i / gridColumns;
            int col = i % gridColumns;

            // X, Y の座標を計算
            float xPosition = cameraCenter.x - camWidth / 2f + imageWidth / 2f + col * imageWidth;
            float yPosition = cameraCenter.y + camHeight / 2f - imageHeight / 2f - row * imageHeight;

            // 画像を表示するための平面オブジェクトを作成
            imagePlanes[i] = GameObject.CreatePrimitive(PrimitiveType.Quad);
            imagePlanes[i].transform.position = new Vector3(xPosition, yPosition, zPosition);

            // テクスチャを読み込む
            imageTextures[i] = LoadTextureFromFile(filePaths[i]);

            // 画像のスケールを設定
            float scaledWidth = imageTextures[i].width / 100f * imageScale;
            float scaledHeight = imageTextures[i].height / 100f * imageScale;
            imagePlanes[i].transform.localScale = new Vector3(scaledWidth, scaledHeight, 1f);

            // 画像をテクスチャとして適用
            Material material = new Material(Shader.Find("Unlit/Texture"));
            material.mainTexture = imageTextures[i];
            imagePlanes[i].GetComponent<Renderer>().material = material;

            // 初期状態では画像を非表示にする
            imagePlanes[i].SetActive(false);
        }
    }

    public Texture2D LoadTextureFromFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            // ファイルをバイト配列として読み込む
            byte[] fileData = File.ReadAllBytes(filePath);
            
            // 新しいTexture2Dオブジェクトを作成し、画像データをロード
            Texture2D texture = new Texture2D(2, 2);
            if (texture.LoadImage(fileData))
            {
                return texture;
            }
            else
            {
                Debug.LogError("Failed to load image data.");
                return null;
            }
        }
        else
        {
            Debug.LogError("File not found at: " + filePath);
            return null;
        }
    }

    void Update()
    {
        // カメラと画像の距離をチェック
        if (imagePlanes != null)
        {
            foreach (GameObject imagePlane in imagePlanes)
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
    }

    void Start()
    {
        // グリッドに画像を配置
        PlaceImagesInGrid();
    }
}
