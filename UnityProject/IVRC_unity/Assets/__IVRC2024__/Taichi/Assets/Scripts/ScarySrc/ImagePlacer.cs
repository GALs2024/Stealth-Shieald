using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq; // これを追加
using UnityEngine;

public class ImagePlacer : MonoBehaviour
{
    private string folderPath = "__IVRC2024__/Taichi/Assets/Textures/Mosaic/Originals"; // 画像があるフォルダのパス
    public float zPosition = 0f; // 画像を配置するz軸の値
    private Camera mainCamera; // メインカメラ
    public GameObject imagePrefab; // 画像を表示するためのプレハブ
    private List<Sprite> images = new List<Sprite>(); // 画像のスプライトリスト
    public float maxDistance = 10f; // カメラからの最大距離

    private List<GameObject> imageObjects = new List<GameObject>(); // 生成された画像オブジェクトのリスト

    void Start()
    {
        this.mainCamera = Camera.main;
        LoadImagesFromFolder();
        ArrangeImages();
    }

    void Update()
    {
        CheckDistanceAndHideImages();
    }

    // 指定フォルダから画像をロードしてスプライトに変換
    void LoadImagesFromFolder()
    {
        this.folderPath = Path.Combine(Application.dataPath, this.folderPath);
        string[] imagePaths = { Path.Combine(this.folderPath, "2_magazine_style_output.png") };
        // string[] imagePaths = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly)
        //     .Where(path => path.EndsWith(".png") || path.EndsWith(".jpg") || path.EndsWith(".jpeg")).ToArray();

        foreach (string path in imagePaths)
        {
            if (path.EndsWith(".png") || path.EndsWith(".jpg") || path.EndsWith(".jpeg"))
            {
                byte[] imageBytes = File.ReadAllBytes(path);
                Texture2D texture = new Texture2D(2, 2);

                if (texture.LoadImage(imageBytes))
                {
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

        foreach (Sprite sprite in images)
        {
            totalWidth += sprite.bounds.size.x;
        }

        Vector3 centerPosition = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, mainCamera.nearClipPlane));
        float startX = centerPosition.x - (totalWidth / 2) + (images[0].bounds.size.x / 2);

        float currentX = startX;
        for (int i = 0; i < images.Count; i++)
        {
            Sprite sprite = images[i];
            GameObject imageObject = Instantiate(imagePrefab);
            imageObject.name = sprite.name;

            SpriteRenderer renderer = imageObject.GetComponent<SpriteRenderer>();
            renderer.sprite = sprite;

            float imageWidth = sprite.bounds.size.x;
            imageObject.transform.position = new Vector3(currentX, centerPosition.y + 20, zPosition);
            currentX += imageWidth;

            imageObjects.Add(imageObject); // 生成された画像オブジェクトをリストに追加
        }
    }

    // 各フレームでカメラとの距離をチェックし、画像を非表示にする
    void CheckDistanceAndHideImages()
    {
        foreach (GameObject imageObject in imageObjects)
        {
            float distance = Vector3.Distance(mainCamera.transform.position, imageObject.transform.position);

            // 距離が最大距離を超えたら非表示にする
            if (distance > maxDistance)
            {
                imageObject.SetActive(false);
            }
            else
            {
                imageObject.SetActive(true);
            }
        }
    }
}
