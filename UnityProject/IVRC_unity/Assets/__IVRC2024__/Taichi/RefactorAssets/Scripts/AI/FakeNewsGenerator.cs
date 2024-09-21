using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// This class is responsible for generating fake news scripts using OpenAI API and converting them to audio.
public class FakeNewsGenerator : MonoBehaviour
{
    private OpenAIChat _openAIChat;
    private OpenAITTS _ttsService;
    private ChatHistoryLoader _chatHistoryLoader;
    private string _apiKey;
    private string _systemMessage;
    private string _targetContent;

    private const string TargetInfoPath = @"__IVRC2024__/Taichi/Assets/Data/target_output.txt";
    private const string OutputPath = @"__IVRC2024__/Taichi/Assets/Audio/FakeNews";

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        _apiKey = ApiKeyLoader.LoadApiKey();
        _chatHistoryLoader = new ChatHistoryLoader();

        string targetInfoFilePath = Path.Combine(Application.dataPath, TargetInfoPath);
        _targetContent = FileReaderUtil.ReadFileSync(targetInfoFilePath);

        _systemMessage = "# 命令  [人物像]の内容を含めて、架空のテロニュースの原稿を1つ（50文字以内）作成してください。  # 出力json形式  [{'details': ''}]";
        _openAIChat = new OpenAIChat(_apiKey, "gpt-4o-mini", _systemMessage, 500);
        _ttsService = new OpenAITTS(_apiKey, "tts-1", "alloy");
    }

    // Initiates the process of generating fake news content and converting it to speech.
    public void Generate()
    {
        string conversationHistory = _chatHistoryLoader.GetChatHistory();
        string userInput = "  # 人物像  " + _targetContent;

        Debug.Log(userInput);
        StartCoroutine(_openAIChat.RequestChatResponse(_systemMessage, userInput, OnSuccess, OnError));
    }

    // Callback when the OpenAI chat response is successful.
    // It processes the JSON response and converts the generated content to speech.
    private void OnSuccess(string message)
    {
        List<Dictionary<string, string>> jsonText = DeserializeJson(message);
        Debug.Log(jsonText);

        for (int i = 0; i < jsonText.Count; i++)
        {
            Dictionary<string, string> item = jsonText[i];

            if (item.ContainsKey("details"))
            {
                string details = item["details"];
                Debug.Log($"Index: {i}, Details: {details}");

                string outputFileName = $"details{i}.mp3";
                string outputFilePath = Path.Combine(Application.dataPath, OutputPath, outputFileName);

                StartCoroutine(_ttsService.ConvertTextToSpeech(
                    details, outputFilePath, OnTTSSuccess, OnError
                ));
            }
        }
    }

    // Deserializes a JSON string into a list of dictionaries.
    private List<Dictionary<string, string>> DeserializeJson(string content)
    {
        return Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(content);
    }

    // Callback when the text-to-speech conversion is successful.
    private void OnTTSSuccess(byte[] audioData, string filePath)
    {
        Debug.Log("Audio file saved at: " + filePath);
    }

    // Callback for handling errors in API calls.
    private void OnError(string error)
    {
        Debug.LogError(error);
    }
}
