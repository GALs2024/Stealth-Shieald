using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class OpenAIChat
{
    private string apiKey;
    private string chatModel;
    private string systemMessage;
    private int maxTokens;
    private string chatUrl = "https://api.openai.com/v1/chat/completions";

    public OpenAIChat(string apiKey, string chatModel = "gpt-4o-mini", string systemMessage = "キーワードを含んで短く返しをしてください.", int maxTokens = 20)
    {
        this.apiKey = apiKey;
        this.chatModel = chatModel;
        this.systemMessage = systemMessage;
        this.maxTokens = maxTokens;
    }

    public IEnumerator RequestChatResponse(string systemMessage, string userInput, Action<string> onSuccess, Action<string> onError)
    {
        this.systemMessage = systemMessage;
        string jsonBody = ConstructJsonBody(userInput);

        UnityWebRequest request = new UnityWebRequest(chatUrl, "POST");
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
            string message = ExtractMessage(responseJson);
            onSuccess?.Invoke(message);
        }
    }

    private string ConstructJsonBody(string userInput)
    {
        string formattedUserInput = TextFomatter.SanitizeInput(userInput);

        return $"{{\"model\": \"{chatModel}\", \"messages\": [" +
               $"{{\"role\": \"system\", \"content\": \"{systemMessage}\"}}," +
               $"{{\"role\": \"user\", \"content\": \"{formattedUserInput}\"}}]," +
               $"\"max_tokens\": {maxTokens}}}";
    }

    private string ExtractMessage(string responseJson)
    {
        try
        {
            var responseObj = Newtonsoft.Json.Linq.JObject.Parse(responseJson);
            var message = responseObj["choices"]?[0]?["message"]?["content"]?.ToString();
            Debug.Log("message: " + message);
            return message ?? "No message found";
        }
        catch (Exception e)
        {
            Debug.LogError("Error parsing response JSON: " + e.Message);
            return "Error parsing message";
        }
    }
}
