using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ImagePlacer : MonoBehaviour
{
    private string folderPath = "__IVRC2024__/Taichi/Assets/Textures/Mosaic/Originals"; // 画像があるフォルダのパス
    public float zPosition = 0f; // 画像を配置するz軸の値
    private Camera mainCamera; // メインカメラ

    public GameObject imagePrefab; // 画像を表示するためのプレハブ
    private List<Sprite> images = new List<Sprite>(); // 画像のスプライトリスト

    void Start()
    {
        this.mainCamera = Camera.main;
        LoadImagesFromFolder();
        ArrangeImages();
    }

    // 指定フォルダから画像をロードしてスプライトに変換
    void LoadImagesFromFolder()
    {
        // すべての画像ファイル (pngやjpgなど) を取得
        this.folderPath = Path.Combine(Application.dataPath, this.folderPath);
        string[] imagePaths = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly);
        
        foreach (string path in imagePaths)
        {
            if (path.EndsWith(".png") || path.EndsWith(".jpg") || path.EndsWith(".jpeg"))
            {
                byte[] imageBytes = File.ReadAllBytes(path);
                Texture2D texture = new Texture2D(2, 2);
                
                // テクスチャをロードし、失敗した場合はログ出力
                if (texture.LoadImage(imageBytes))
                {
                    // テクスチャをスプライトに変換
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
                    images.Add(sprite);
                }
                else
                {
                    Debug.LogError("Failed to load image from: " + path);
                }
            }
        }
    }

    // 画像を並べる処理
    void ArrangeImages()
    {
        if (images.Count == 0)
        {
            Debug.LogError("No images found in the folder.");
            return;
        }

        float totalWidth = 0;
        List<GameObject> imageObjects = new List<GameObject>();

        // 画像の横幅を計算
        foreach (Sprite sprite in images)
        {
            totalWidth += sprite.bounds.size.x;
        }

        // 2番目の画像の中心をカメラの中心に配置
        Vector3 centerPosition = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, mainCamera.nearClipPlane + zPosition));

        // 並べるための開始位置を計算
        float startX = centerPosition.x - (totalWidth / 2) + (images[1].bounds.size.x / 2);

        // 画像を横に並べる
        float currentX = startX;
        for (int i = 0; i < images.Count; i++)
        {
            Sprite sprite = images[i];

            // プレハブをインスタンス化
            GameObject imageObject = Instantiate(imagePrefab);
            imageObject.name = sprite.name;

            // スプライトを設定
            SpriteRenderer renderer = imageObject.GetComponent<SpriteRenderer>();
            renderer.sprite = sprite;

            // 位置を設定
            float imageWidth = sprite.bounds.size.x;
            imageObject.transform.position = new Vector3(currentX + 10, centerPosition.y + 10, zPosition);

            currentX += imageWidth; // 次の画像の位置へ移動
        }
    }
}
