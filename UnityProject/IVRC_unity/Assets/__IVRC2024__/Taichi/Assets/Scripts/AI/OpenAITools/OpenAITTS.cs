using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class OpenAITTS
{
    private string apiKey;
    private string ttsModel;
    private string voice;
    private string saveDirectory;
    private string ttsUrl = "https://api.openai.com/v1/audio/speech";

    public OpenAITTS(string apiKey, string ttsModel = "tts-1", string voice = "alloy", string saveDirectory = "Audios/AIOutput")
    {
        this.apiKey = apiKey;
        this.ttsModel = ttsModel;
        this.voice = voice;
        this.saveDirectory = saveDirectory;
    }

    public IEnumerator ConvertTextToSpeech(string text, Action<byte[], string> onSuccess, Action<string> onError)
    {
        string jsonBody = $"{{\"model\": \"{ttsModel}\", \"voice\": \"{voice}\", \"input\": \"{text}\", \"response_format\": \"mp3\"}}";

        UnityWebRequest request = new UnityWebRequest(ttsUrl, "POST");
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
            byte[] audioData = request.downloadHandler.data;

            string filePath = Path.Combine(Application.dataPath, saveDirectory, "output.mp3");
            SaveWavFile(audioData, filePath);
            onSuccess?.Invoke(audioData, filePath);
        }
    }

    private void SaveWavFile(byte[] audioData, string filePath)
    {
        try
        {
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllBytes(filePath, audioData);
            // Debug.Log("Audio data saved successfully at: " + filePath);
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving WAV file: " + e.Message);
        }
    }
}
