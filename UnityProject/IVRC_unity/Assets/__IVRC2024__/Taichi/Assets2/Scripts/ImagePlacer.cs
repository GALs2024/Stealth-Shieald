using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ImagePlacer : MonoBehaviour
{
    private string folderPath = "Assets/__IVRC2024__/Taichi/Assets/Textures/Mosaic/Originals"; // 画像があるフォルダのパス
    public float zPosition = 0f; // 画像を配置するz軸の値
    private Camera mainCamera; // メインカメラ

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

        // 画像をGameObjectとして生成し、そのサイズを取得
        foreach (Sprite sprite in images)
        {
            GameObject imageObject = new GameObject(sprite.name);
            SpriteRenderer renderer = imageObject.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;

            imageObjects.Add(imageObject);

            totalWidth += sprite.bounds.size.x; // 横幅を計算
        }

        // 2番目の画像の中心をカメラの中心に配置
        Vector3 centerPosition = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, mainCamera.nearClipPlane + zPosition));

        // 並べるための開始位置を計算
        float startX = centerPosition.x - (totalWidth / 2) + (images[1].bounds.size.x / 2);

        // 画像を横に並べる
        float currentX = startX;
        for (int i = 0; i < images.Count; i++)
        {
            GameObject imageObject = imageObjects[i];
            float imageWidth = images[i].bounds.size.x;
            imageObject.transform.position = new Vector3(currentX, centerPosition.y, zPosition);
            currentX += imageWidth; // 次の画像の位置へ移動
        }
    }
}
