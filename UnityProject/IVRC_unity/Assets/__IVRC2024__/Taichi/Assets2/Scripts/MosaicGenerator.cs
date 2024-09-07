using UnityEngine;
using System.IO;

public class MosaicGenerator : MonoBehaviour
{
    private string filePath = @"Assets/__IVRC2024__/Taichi/Assets2/Textures/Mosaic/background_removed.png"; // ターゲット画像のファイルパス
    private Texture2D targetImage;  // モザイクアートにする対象画像
    private string imagesFolderPath = @"Assets/__IVRC2024__/Taichi/Assets2/Textures/Mosaic/Tiles";  // 画像が格納されているフォルダのパス
    private Vector2 outputResolution = new Vector2(1024, 1024); // リサイズ後のターゲット画像の幅と高さ 
    public Vector2 gridTable = new Vector2(10, 10); // モザイクの横方向のタイル数と縦方向のタイル数
    public int tileResolution = 50; // 各タイルの幅と高さ（ピクセル）
    public GameObject tilePrefab;  // タイルを表現するプレハブ
    public float pixelsPerUnit = 100f; // 1ユニットあたりのピクセル数
    public float gapSize = 0.05f; // タイル間の隙間のサイズ

    private Texture2D[] tileImages; // フォルダ内の画像を格納する配列
    private Transform tileParent; // タイルを整理するための親オブジェクト

    void Start()
    {
        targetImage = LoadTextureFromFile(filePath);
        tileParent = new GameObject("TileParent").transform;

        if (targetImage != null)
        {
            if (Directory.Exists(imagesFolderPath))
            {
                PreprocessTileImages();
                this.tileResolution = tileImages[0].width;
                // Texture2D resizedImage = ResizeTexture(targetImage, (int)outputResolution.x, (int)outputResolution.y);
                // Debug.Log("resized");
                // PlaceGridTiles(resizedImage, (int)gridTable.x, (int)gridTable.y);
                PlaceGridTiles(this.targetImage, (int)gridTable.x, (int)gridTable.y);
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

    void Update()
    {
        ManageTileLoading();
    }

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
            texture.LoadImage(fileData);
            tileImages[i] = texture;
        }
    }

    void PlaceGridTiles(Texture2D image, int columns, int rows)
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

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Color avgColor = GetAverageColor(image, x, y, gridWidth, gridHeight);

                if (IsTransparentTile(image, x, y, gridWidth, gridHeight))
                {
                    continue;
                }

                Vector3 position = new Vector3(x * worldTileWidth, y * worldTileHeight, 0) + startPosition;
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity, tileParent);
                SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();

                Texture2D tileTexture = tileImages[Random.Range(0, tileImages.Length)];
                Texture2D resizedTileTexture = ResizeTexture(tileTexture, tileResolution, tileResolution);
                ApplyColorToTexture(resizedTileTexture, avgColor);
                Sprite sprite = Sprite.Create(resizedTileTexture, new Rect(0, 0, tileResolution, tileResolution), new Vector2(0.5f, 0.5f), pixelsPerUnit);
                renderer.sprite = sprite;
                renderer.enabled = true;
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
