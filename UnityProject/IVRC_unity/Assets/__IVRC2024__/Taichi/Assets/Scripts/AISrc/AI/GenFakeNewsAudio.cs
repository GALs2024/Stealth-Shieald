using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GenFakeNews : MonoBehaviour
{
    private OpenAIChat openAIChat;
    private OpenAITTS ttsService;
    private ChatHistoryLoader chatHistoryLoader;
    private string targetInfoPath = @"__IVRC2024__/Taichi/Assets/Data/target_output.txt";
    private string apiKey;
    private string systemMessage;
    private string targetContent;

    private string outputPath = @"__IVRC2024__/Taichi/Assets/Audio/FakeNews";

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        this.apiKey = ApiKeyLoader.LoadApiKey();
        this.chatHistoryLoader = new ChatHistoryLoader();
        string _targetInfoPath = Path.Combine(Application.dataPath, this.targetInfoPath);
        this.targetContent = FileReaderUtil.ReadFileSync(_targetInfoPath);
        this.systemMessage = "# 命令  [人物像]の内容を含めて、架空のテロニュースの原稿を3種類（100文字以内ずつ）作成してください。  # 出力json形式  [{'details': ''},{'details': ''},{'details': ''}]";
        this.openAIChat = new OpenAIChat(this.apiKey, "gpt-4o-mini", this.systemMessage, 500);
        this.ttsService = new OpenAITTS(this.apiKey, "tts-1", "alloy");
    }

    public void Generate()
    {
        string conversationHistory = this.chatHistoryLoader.GetChatHistory();
        // string userInput = "# 会話内容  " + conversationHistory + "  # 人物像  " + this.targetContent;
        string userInput = "  # 人物像  " + this.targetContent;
        Debug.Log(userInput);
        StartCoroutine(this.openAIChat.RequestChatResponse(this.systemMessage, userInput, OnSuccess, OnError));
    }

    private void OnSuccess(string message)
    {
        List<Dictionary<string, string>> json_text = DeserializeJson(message);
        Debug.Log(json_text);

        for (int i = 0; i < json_text.Count; i++)
        {
            Dictionary<string, string> item = json_text[i];
            
            if (item.ContainsKey("details"))
            {
                string details = item["details"];
                Debug.Log($"Index: {i}, Details: {details}");
                string ouputFileName = "details" + i + ".mp3";
                string outputFilePath = Path.Combine(Application.dataPath, this.outputPath, ouputFileName);
                StartCoroutine(this.ttsService.ConvertTextToSpeech(
                    details, outputFilePath, (audioData, filePath) => OnTTSSuccess(audioData, filePath), OnError
                ));
            }
        }
    }

    private List<Dictionary<string, string>> DeserializeJson(string content)
    {
        return Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(content);
    }

    private void OnTTSSuccess(byte[] audioData, string filePath)
    {
        Debug.Log("Audio file saved at: " + filePath);
    }

    private void OnError(string error)
    {
        Debug.LogError(error);
    }
}
