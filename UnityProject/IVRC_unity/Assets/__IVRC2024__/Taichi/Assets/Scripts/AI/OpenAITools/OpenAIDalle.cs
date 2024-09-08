using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class OpenAIDalle
{
    private string apiKey;
    private string model;
    private int imageSize;
    private string dalleUrl = "https://api.openai.com/v1/images/generations";

    // 通常のコンストラクタを使って初期化
    public OpenAIDalle(string apiKey, string model = "dall-e-2", int imageSize = 1024)
    {
        this.apiKey = apiKey;
        this.model = model;
        this.imageSize = imageSize;
    }

    public IEnumerator RequestDalleImage(string prompt, string savePath, Action<string> onSuccess, Action<string> onError)
    {
        string jsonBody = ConstructJsonBody(prompt);

        UnityWebRequest request = new UnityWebRequest(dalleUrl, "POST");
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            onError?.Invoke(request.error);
        }
        else
        {
            string responseJson = request.downloadHandler.text;
            string imageUrl = ExtractImageUrl(responseJson);
            if (!string.IsNullOrEmpty(imageUrl))
            {
                yield return DownloadImage(imageUrl, savePath, onSuccess, onError);
            }
            else
            {
                onError?.Invoke("No image URL found");
            }
        }
    }

    private string ConstructJsonBody(string prompt)
    {
        return $"{{\"model\": \"{this.model}\", \"prompt\": \"{prompt}\", \"n\": 1, \"size\": \"{imageSize}x{imageSize}\"}}";
    }

    private string ExtractImageUrl(string responseJson)
    {
        try
        {
            var responseObj = Newtonsoft.Json.Linq.JObject.Parse(responseJson);
            var imageUrl = responseObj["data"]?[0]?["url"]?.ToString();
            Debug.Log("Generated Image URL: " + imageUrl);
            return imageUrl;
        }
        catch (Exception e)
        {
            Debug.LogError("Error parsing response JSON: " + e.Message);
            return null;
        }
    }

    private IEnumerator DownloadImage(string imageUrl, string savePath, Action<string> onSuccess, Action<string> onError)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            onError?.Invoke(request.error);
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;

            try
            {
                byte[] imageBytes = texture.EncodeToPNG();
                File.WriteAllBytes(savePath, imageBytes);  // 画像を保存

                onSuccess?.Invoke("Image saved to: " + savePath);
            }
            catch (Exception e)
            {
                onError?.Invoke("Failed to save image: " + e.Message);
            }
        }
    }
}
