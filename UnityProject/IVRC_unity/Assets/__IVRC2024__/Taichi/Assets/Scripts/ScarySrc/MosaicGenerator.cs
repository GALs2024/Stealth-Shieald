using UnityEngine;
using System.IO;
using System.Collections;

public class MosaicGenerator : MonoBehaviour
{
    private string filePath = @"__IVRC2024__/Taichi/Assets/Textures/Mosaic/background_removed.png";
    private Texture2D targetImage;
    private string imagesFolderPath = @"__IVRC2024__/Taichi/Assets/Textures/Mosaic/Tiles";
    public Vector2 gridTable = new Vector2(10, 10);
    public int tileResolution = 50;
    public GameObject tilePrefab;
    public float pixelsPerUnit = 100f;
    public float gapSize = 0.05f;

    private Texture2D[] tileImages;
    private Transform tileParent;

    public bool DontDestroyEnabled = true;
    public static MosaicGenerator instance = null;

    // AwakeはStartより先に呼ばれる
    void Awake () {
        if (instance == null) {
            instance = this;
            if (DontDestroyEnabled) {
                // シーンを遷移してもオブジェクトが消えないようにする
                DontDestroyOnLoad (this.gameObject);
            }
        } else {
            // すでに存在するインスタンスがあればこのオブジェクトを破棄
            Destroy (this.gameObject);
        }
    }

    // void Start()
    // {
    //     StartCoroutine(GenerateMosaicAsync());
    // }
    
    public IEnumerator GenerateMosaicAsync()
    {
        string targetImgPath = Path.Combine(Application.dataPath, this.filePath);
        targetImage = LoadTextureFromFile(targetImgPath);
        tileParent = new GameObject("TileParent").transform;

        this.imagesFolderPath = Path.Combine(Application.dataPath, this.imagesFolderPath);

        if (targetImage != null && Directory.Exists(imagesFolderPath))
        {
            PreprocessTileImages();
            this.tileResolution = tileImages[0].width;

            // タイル生成を非同期で実行
            yield return StartCoroutine(PlaceGridTilesAsync(targetImage, (int)gridTable.x, (int)gridTable.y));
        }
        else
        {
            Debug.LogError("ターゲット画像が存在しないか、フォルダが見つかりません。");
        }
    }

    // void Update()
    // {
    //     ManageTileLoading();
    // }

    public Texture2D LoadTextureFromFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            byte[] fileData = File.ReadAllBytes(filePath);
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

    void PreprocessTileImages()
    {
        string[] filePaths = Directory.GetFiles(imagesFolderPath, "*.png"); // ここではPNG画像を対象とする
        tileImages = new Texture2D[filePaths.Length];

        for (int i = 0; i < filePaths.Length; i++)
        {
            byte[] fileData = File.ReadAllBytes(filePaths[i]);
            Texture2D texture = new Texture2D(2, 2);
            if (texture.LoadImage(fileData))
            {
                tileImages[i] = texture;
                Debug.Log($"Tile image loaded: {filePaths[i]}");
            }
            else
            {
                Debug.LogError($"Failed to load tile image: {filePaths[i]}");
            }
        }
    }


    IEnumerator PlaceGridTilesAsync(Texture2D image, int columns, int rows)
    {
        int gridWidth = image.width / columns;
        int gridHeight = image.height / rows;
        float worldTileWidth = tileResolution / pixelsPerUnit + gapSize;
        float worldTileHeight = tileResolution / pixelsPerUnit + gapSize;
        float totalWidth = columns * worldTileWidth;
        float totalHeight = rows * worldTileHeight;
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 startPosition = new Vector3(
            cameraPosition.x - totalWidth / 2f + worldTileWidth / 2f,
            cameraPosition.y - totalHeight / 2f + worldTileHeight / 2f,
            0
        );

        int tilesPerFrame = 5;  // 1フレームごとに生成するタイルの数
        int tileCount = 0;      // 現在のフレームで生成したタイルの数

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                // 平均色を取得
                Color avgColor = GetAverageColor(image, x, y, gridWidth, gridHeight);

                // 透明なタイルはスキップ
                if (IsTransparentTile(image, x, y, gridWidth, gridHeight))
                {
                    continue;
                }

                // タイルの生成
                Vector3 position = new Vector3(x * worldTileWidth, y * worldTileHeight, 0) + startPosition;
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity, tileParent);
                SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();

                if (renderer == null)
                {
                    Debug.LogError("SpriteRenderer not found on tilePrefab.");
                    continue;
                }

                // ランダムなタイル画像の取得とリサイズ
                Texture2D tileTexture = tileImages[Random.Range(0, tileImages.Length)];
                Texture2D resizedTileTexture = ResizeTexture(tileTexture, tileResolution, tileResolution);
                ApplyColorToTexture(resizedTileTexture, avgColor);

                // スプライトを作成し、SpriteRendererに設定
                Sprite sprite = Sprite.Create(resizedTileTexture, new Rect(0, 0, tileResolution, tileResolution), new Vector2(0.5f, 0.5f), pixelsPerUnit);
                renderer.sprite = sprite;

                // タイルを表示
                renderer.enabled = true;

                // タイルを生成した数をカウント
                tileCount++;

                // 指定した数のタイルを生成したら、次のフレームまで待機
                if (tileCount >= tilesPerFrame)
                {
                    tileCount = 0;  // タイル数をリセット
                    yield return null;  // 次のフレームまで待機
                }
            }
        }
    }


    bool IsTransparentTile(Texture2D image, int gridX, int gridY, int gridWidth, int gridHeight)
    {
        Color[] pixels = image.GetPixels(gridX * gridWidth, gridY * gridHeight, gridWidth, gridHeight);
        foreach (Color pixel in pixels)
        {
            if (pixel.a > 0f)
            {
                return false;
            }
        }
        return true;
    }

    void ConvertToGrayscale(Texture2D texture)
    {
        Color[] pixels = texture.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            Color pixel = pixels[i];
            float gray = pixel.grayscale;
            pixels[i] = new Color(gray, gray, gray, pixel.a);
        }
        texture.SetPixels(pixels);
        texture.Apply();
    }

    Color GetAverageColor(Texture2D image, int gridX, int gridY, int gridWidth, int gridHeight)
    {
        int startX = gridX * gridWidth;
        int startY = gridY * gridHeight;
        Color[] pixels = image.GetPixels(startX, startY, gridWidth, gridHeight);
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
            pixels[i] *= color;
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
                if (GeometryUtility.TestPlanesAABB(planes, renderer.bounds))
                {
                    renderer.enabled = true;
                }
                else
                {
                    renderer.enabled = false;
                }
            }
        }
    }
}
