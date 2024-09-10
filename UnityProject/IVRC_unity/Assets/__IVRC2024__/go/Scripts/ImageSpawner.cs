using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ImageSpawner : MonoBehaviour
{
    public string imagesFolderPath = "Assets/__IVRC2024__/go/Images/"; // 画像フォルダへのパス
    public GameObject centralObject; // VRカメラ (中央のオブジェクト)
    public float spawnRadius = 15f; // 中央オブジェクトからの半径
    public GameObject imagePrefab; // 各画像のプレハブ
    public int totalImagesToSpawn = 100; // 合計表示する画像の数
    public float verticalFOV = 60f; // 垂直方向の視野角 (VRカメラのFOVに合わせる)
    public float horizontalFOV = 90f; // 水平方向の視野角

    private List<GameObject> spawnedImages = new List<GameObject>(); // 生成した画像オブジェクトのリスト

    void Start()
    {
        SpawnImages();
    }

    void Update()
    {
        CheckVisibility(); // 毎フレーム、カメラの視界に入っているかを確認
    }

    void SpawnImages()
    {
        // 対応する画像ファイル拡張子を設定
        string[] validExtensions = { ".png", ".jpg", ".jpeg" };

        // フォルダ内およびサブフォルダ内のすべてのファイルを取得し、有効な画像拡張子のみ選別
        List<string> filePaths = new List<string>();
        string _imagesFolderPath = Path.Combine(Application.dataPath, imagesFolderPath);
        foreach (string file in Directory.GetFiles(_imagesFolderPath, "*.*", SearchOption.AllDirectories))
        {
            if (System.Array.Exists(validExtensions, ext => file.ToLower().EndsWith(ext)))
            {
                filePaths.Add(file);
                // ファイルパスをコンソールに表示して確認
                Debug.Log("Found image file: " + file);
            }
        }

        if (filePaths.Count == 0)
        {
            Debug.LogError("フォルダに画像が見つかりません。");
            return;
        }

        // 各画像を合計で表示する数を計算
        int imagesPerFile = totalImagesToSpawn / filePaths.Count;

        // 各ファイルごとにプレハブを生成し、複数回（imagesPerFile）表示する
        foreach (var filePath in filePaths)
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData); // 画像をロード

            for (int i = 0; i < imagesPerFile; i++) // 各画像を指定回数表示
            {
                // 新しい画像のプレハブをインスタンス化
                GameObject newImageObject = Instantiate(imagePrefab);

                // テクスチャを新しい画像のマテリアルに割り当てる（Rendererコンポーネントが必要）
                newImageObject.GetComponent<Renderer>().material.mainTexture = texture;

                // 中央オブジェクトの前方半球内にランダムに配置する
                Vector3 randomDirection = GetRandomDirectionWithinFOV();
                Vector3 randomPosition = randomDirection * spawnRadius + centralObject.transform.position;
                newImageObject.transform.position = randomPosition;

                // 画像を中央オブジェクトの方に向かせる
                newImageObject.transform.LookAt(centralObject.transform);

                // Quadを反転させて、画像の正しい面が中央を向くようにする
                newImageObject.transform.Rotate(0, 180, 0);

                // 生成したオブジェクトをリストに追加
                spawnedImages.Add(newImageObject);
            }
        }
    }


    // カメラの視野角内でランダムな方向を生成する
    Vector3 GetRandomDirectionWithinFOV()
    {
        // カメラの前方を基準とする
        Vector3 forward = centralObject.transform.forward;

        // ランダムな角度を計算する
        float randomHorizontalAngle = Random.Range(-horizontalFOV / 2f, horizontalFOV / 2f);
        float randomVerticalAngle = Random.Range(-verticalFOV / 2f, verticalFOV / 2f);

        // 回転行列を使って方向を調整する
        Quaternion horizontalRotation = Quaternion.AngleAxis(randomHorizontalAngle, Vector3.up);
        Quaternion verticalRotation = Quaternion.AngleAxis(randomVerticalAngle, centralObject.transform.right);

        // 水平方向と垂直方向の回転を適用して方向を計算
        Vector3 randomDirection = verticalRotation * horizontalRotation * forward;

        return randomDirection.normalized;
    }

    // カメラの視野に入っているか確認して、Rendererを有効/無効にする
    void CheckVisibility()
    {
        Camera cam = centralObject.GetComponent<Camera>();

        foreach (GameObject imageObject in spawnedImages)
        {
            Vector3 viewportPoint = cam.WorldToViewportPoint(imageObject.transform.position);

            // オブジェクトがカメラの視界内にあるかチェック
            bool isVisible = viewportPoint.z > 0 && viewportPoint.x > 0 && viewportPoint.x < 1 && viewportPoint.y > 0 && viewportPoint.y < 1;

            // Rendererの有効/無効を切り替える
            MeshRenderer renderer = imageObject.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.enabled = isVisible;
            }
        }
    }
}
