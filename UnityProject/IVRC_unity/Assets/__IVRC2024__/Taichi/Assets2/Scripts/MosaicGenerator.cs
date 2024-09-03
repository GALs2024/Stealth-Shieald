using UnityEngine;
using System.IO;

public class MosaicGenerator : MonoBehaviour
{
    public Texture2D targetImage;  // モザイクアートにする対象画像
    public string imagesFolderPath;  // 画像が格納されているフォルダのパス
    public Vector2 outputResolution = new Vector2(1024, 1024); // リサイズ後のターゲット画像の幅と高さ 
    public Vector2 gridTable = new Vector2(10, 10); // モザイクの横方向のタイル数と縦方向のタイル数
    public int tileResolution = 50; // 各タイルの幅と高さ（ピクセル）
    public GameObject tilePrefab;  // タイルを表現するプレハブ
    public float pixelsPerUnit = 100f; // 1ユニットあたりのピクセル数
    public float gapSize = 0.05f; // タイル間の隙間のサイズ

    private Texture2D[] tileImages; // フォルダ内の画像を格納する配列
    private Transform tileParent; // タイルを整理するための親オブジェクト

    void Start()
    {
        tileParent = new GameObject("TileParent").transform;

        if (targetImage != null)
        {
            if (Directory.Exists(imagesFolderPath))
            {
                // フォルダ内の画像を事前に処理
                PreprocessTileImages();

                // targetImageを指定された解像度にリサイズ
                Texture2D resizedImage = ResizeTexture(targetImage, (int)outputResolution.x, (int)outputResolution.y);

                Debug.Log("resized");

                // 各タイルの色を変更し、モザイクアートを作成
                PlaceGridTiles(resizedImage, (int)gridTable.x, (int)gridTable.y);
            }
            else
            {
                Debug.LogError("指定されたフォルダが存在しません: " + imagesFolderPath);
            }
        }
        else
        {
            Debug.LogError("ターゲット画像が設定されていません。");
        }
    }

    void PreprocessTileImages()
    {
        string[] filePaths = Directory.GetFiles(imagesFolderPath, "*.png"); // ここではPNG画像を対象とする
        tileImages = new Texture2D[filePaths.Length];

        for (int i = 0; i < filePaths.Length; i++)
        {
            byte[] fileData = File.ReadAllBytes(filePaths[i]);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);

            // 画像をグレースケールに変換
            ConvertToGrayscale(texture);

            // タイルに使用する画像をリサイズ
            Texture2D resizedTileTexture = ResizeTexture(texture, tileResolution, tileResolution);

            // 事前処理済みの画像を保存
            tileImages[i] = resizedTileTexture;
        }
    }

    void PlaceGridTiles(Texture2D image, int columns, int rows)
    {
        // 各グリッドの幅と高さを計算
        int gridWidth = image.width / columns;
        int gridHeight = image.height / rows;

        // 各タイルのサイズをワールド単位に変換し、隙間を考慮
        float worldTileWidth = tileResolution / pixelsPerUnit + gapSize;
        float worldTileHeight = tileResolution / pixelsPerUnit + gapSize;

        // グリッド全体の幅と高さを計算
        float totalWidth = columns * worldTileWidth;
        float totalHeight = rows * worldTileHeight;

        // カメラのワールド座標を取得
        Vector3 cameraPosition = Camera.main.transform.position;

        // 中心に配置するためのオフセットを計算
        Vector3 startPosition = new Vector3(cameraPosition.x - totalWidth / 2f + worldTileWidth / 2f, 
                                            cameraPosition.y - totalHeight / 2f + worldTileHeight / 2f, 
                                            0);

        // 各グリッドごとにタイルを作成し配置
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                // グリッド領域の平均色を取得
                Color avgColor = GetAverageColor(image, x, y, gridWidth, gridHeight);

                // もしグリッド領域が透明（アルファ値が0）なら、そのタイルをスキップ
                if (IsTransparentTile(image, x, y, gridWidth, gridHeight))
                {
                    continue; // 透明なら処理をスキップ
                }

                // プレハブをインスタンス化し、スプライトを設定
                Vector3 position = new Vector3(x * worldTileWidth, y * worldTileHeight, 0) + startPosition;
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity, tileParent);
                SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();

                // フォルダからランダムに画像を選択
                Texture2D tileTexture = tileImages[Random.Range(0, tileImages.Length)];

                // タイルに使用する画像をリサイズ
                Texture2D resizedTileTexture = ResizeTexture(tileTexture, tileResolution, tileResolution);

                // 平均色を使ってタイル画像を着色
                ApplyColorToTexture(resizedTileTexture, avgColor);

                // スプライトにテクスチャを設定
                Sprite sprite = Sprite.Create(resizedTileTexture, new Rect(0, 0, tileResolution, tileResolution), new Vector2(0.5f, 0.5f), pixelsPerUnit);
                renderer.sprite = sprite;

                // タイルを表示
                renderer.enabled = true;
            }
        }
    }

    // 透明な領域かどうかを判定する関数
    bool IsTransparentTile(Texture2D image, int gridX, int gridY, int gridWidth, int gridHeight)
    {
        // グリッド領域内のピクセルを取得
        Color[] pixels = image.GetPixels(gridX * gridWidth, gridY * gridHeight, gridWidth, gridHeight);

        // ピクセルの中でアルファ値が0のものが1つでもあれば、そのタイルは透明とみなす
        foreach (Color pixel in pixels)
        {
            if (pixel.a > 0f)
            {
                return false; // 完全に透明でない場合
            }
        }

        return true; // 全てのピクセルが透明ならtrueを返す
    }

    void ConvertToGrayscale(Texture2D texture)
    {
        Color[] pixels = texture.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            Color pixel = pixels[i];
            float gray = pixel.grayscale; // グレースケールの値を計算
            pixels[i] = new Color(gray, gray, gray, pixel.a); // アルファ値を保持しつつグレースケールに変換
        }
        texture.SetPixels(pixels);
        texture.Apply();
    }

    Color GetAverageColor(Texture2D image, int gridX, int gridY, int gridWidth, int gridHeight)
    {
        // グリッドの左上のピクセル位置を計算
        int startX = gridX * gridWidth;
        int startY = gridY * gridHeight;

        // グリッドのピクセルを取得
        Color[] pixels = image.GetPixels(startX, startY, gridWidth, gridHeight);

        // 平均色を計算
        Color avgColor = new Color(0, 0, 0);
        foreach (Color pixel in pixels)
        {
            avgColor += pixel;
        }
        avgColor /= pixels.Length;

        return avgColor;
    }

    void ApplyColorToTexture(Texture2D texture, Color color)
    {
        Color[] pixels = texture.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] *= color; // 元の画像の色を保ちながら色調を変更
        }
        texture.SetPixels(pixels);
        texture.Apply();
    }

    Texture2D ResizeTexture(Texture2D source, int width, int height)
    {
        RenderTexture rt = RenderTexture.GetTemporary(width, height);
        Graphics.Blit(source, rt);

        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D resizedTexture = new Texture2D(width, height);
        resizedTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        resizedTexture.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);

        return resizedTexture;
    }

    void ManageTileLoading()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        foreach (Transform tile in tileParent)
        {
            Renderer renderer = tile.GetComponent<Renderer>();
            if (renderer != null)
            {
                // カメラのフラスタム内にある場合は表示
                if (GeometryUtility.TestPlanesAABB(planes, renderer.bounds))
                {
                    renderer.enabled = true;
                }
                // フラスタム外にある場合は非表示
                else
                {
                    renderer.enabled = false;
                }
            }
        }
    }
}
