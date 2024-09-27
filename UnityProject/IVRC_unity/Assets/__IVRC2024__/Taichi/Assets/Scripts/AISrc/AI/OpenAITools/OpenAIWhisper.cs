using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Collections.Generic;

public class OpenAIWhisper
{
    private string apiKey;
    private string whisperModel;
    private string whisperUrl = "https://api.openai.com/v1/audio/transcriptions";

    public OpenAIWhisper(string apiKey, string whisperModel = "whisper-1")
    {
        this.apiKey = apiKey;
        this.whisperModel = whisperModel;
    }

    public IEnumerator RequestTranscription(byte[] audioData, Action<string> onSuccess, Action<string> onError)
    {
        if (WavUtility.IsSilent(audioData))
        {
            onSuccess?.Invoke("");
            yield break;
        }
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("model", whisperModel));
        formData.Add(new MultipartFormDataSection("language", "en"));
        formData.Add(new MultipartFormFileSection("file", audioData, "audio.wav", "audio/wav"));

        UnityWebRequest request = UnityWebRequest.Post(whisperUrl, formData);
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            onError?.Invoke(request.error);
        }
        else
        {
            string responseJson = request.downloadHandler.text;
            string transcription = ExtractTranscription(responseJson);
            onSuccess?.Invoke(transcription);
        }
    }

    private string ExtractTranscription(string responseJson)
    {
        try
        {
            var responseObj = Newtonsoft.Json.Linq.JObject.Parse(responseJson);
            var transcription = responseObj["text"]?.ToString();
            return transcription ?? "No transcription found";
        }
        catch (Exception e)
        {
            Debug.LogError("Error parsing response JSON: " + e.Message);
            return "Error parsing transcription";
        }
    }
}