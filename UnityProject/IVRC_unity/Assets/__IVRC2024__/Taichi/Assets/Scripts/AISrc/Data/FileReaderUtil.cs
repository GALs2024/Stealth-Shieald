using System.Collections;
using System.IO;
using UnityEngine;

public static class FileReaderUtil
{
    // ファイルを同期的に読み込むメソッド (StreamingAssets フォルダを使用)
    public static string ReadFileSync(string filePath)
    {
        // ファイルが存在するかを確認
        if (File.Exists(filePath))
        {
            // Androidプラットフォームの場合は特別な処理が必要なため、警告を出す
            if (Application.platform == RuntimePlatform.Android)
            {
                Debug.LogWarning("Androidでは非同期処理を推奨します。");
            }
            
            // ファイルを読み込んで返す
            return File.ReadAllText(filePath);
        }
        else
        {
            Debug.LogError("指定されたファイルが見つかりません: " + filePath);
            return null;
        }
    }

    // ファイルを非同期的に読み込むメソッド（Android向けも含む）
    public static IEnumerator ReadFileAsync(string fileName, System.Action<string> callback)
    {
        // StreamingAssets フォルダのパスを組み立てる
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        string result = null;

        // ファイルが存在するか確認
        if (File.Exists(filePath) || filePath.Contains("://") || filePath.Contains(":///"))
        {
            // AndroidやWebGLの場合は特殊な方法でアクセス
            if (filePath.Contains("://") || filePath.Contains(":///"))
            {
                using (WWW www = new WWW(filePath))
                {
                    yield return www;
                    result = www.text;
                }
            }
            else
            {
                result = File.ReadAllText(filePath);
            }
        }
        else
        {
            Debug.LogError("指定されたファイルが見つかりません: " + filePath);
        }

        // 結果をコールバックで返す
        callback?.Invoke(result);
    }
}
