using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class ConversationalAI_self : MonoBehaviour
{
    private OpenAIChat chatService;
    private OpenAIWhisper whisperService;
    private OpenAITTS ttsService;
    private MicrophoneRecorder _microphoneRecorder;
    private AIConversationManager_self aiConversationManager;
    private ConversationHistoryManager conversationHistoryManager;
    public GameObject avatarObject;
    private AudioSource targetAudioSource;
    public ThinkingDotsTyper thinkingDotsTyper;

    [SerializeField]
    private string systemMessage = "Please answer shortly with a question.";
    [SerializeField]
    private int maxTokens = 20;
    [SerializeField]
    private int maxMicInputs = 1; // 指定された回数のマイク入力
    private int currentMicInputCount = 0; // 現在のマイク入力回数
    public float mic_threshold = 0.001f;

    private bool isWaitingForUserResponse = false;

    private string lastUserInput;

    private string AIOutputFile = @"__IVRC2024__/Taichi/Assets/Audio/AIOutput/output.mp3";

    public bool englishmode = false;

    public void Initialize()
    {
        var apiKey = ApiKeyLoader.LoadApiKey();

        this.aiConversationManager = FindObjectOfType<AIConversationManager_self>();

        this.targetAudioSource = avatarObject.GetComponent<AudioSource>();

        this.whisperService = new OpenAIWhisper(apiKey, "whisper-1");
        this.chatService = new OpenAIChat(apiKey, "gpt-4o-mini", systemMessage, maxTokens);
        this.ttsService = new OpenAITTS(apiKey, "tts-1", "nova");

        this.conversationHistoryManager = new ConversationHistoryManager();

        this._microphoneRecorder = GetComponent<MicrophoneRecorder>();
        this._microphoneRecorder.OnRecordingStopped += OnRecordingStopped;
    }

    void Update()
    {
        if (this.isWaitingForUserResponse)
        {
            this.thinkingDotsTyper.StartTyping();
        }
        else
        {
            this.thinkingDotsTyper.StopTyping();
        }
    }

    void OnDestroy()
    {
        this._microphoneRecorder.OnRecordingStopped -= OnRecordingStopped;
    }

    public void StartConversation(string _systemMessage, string userInput)
    {
        // userとAIで保存方法を変えるここで直接保存するようにしたほうが良いかも
        this.lastUserInput = userInput;

        if (_systemMessage == null)
        {
            string first_reply = "質問しながら返答してください";
            string delightful_reply = "キーワードを含んで楽しく返答してください";
            if (englishmode)
            {
                first_reply = "Ask one question that digs deeper into the input.";
                delightful_reply = "Do not ask questions. Just put in key words and phase them in.";
            }
            this.systemMessage = first_reply;
            Debug.Log("currentMicInputCount: " + this.currentMicInputCount);
            if (this.currentMicInputCount == this.maxMicInputs)
            {
                this.systemMessage = delightful_reply;
            }
        }
        else
        {
            this.systemMessage = _systemMessage;
            Debug.Log("systemMessage: " + this.systemMessage);
        }

        // 会話履歴の内容をすべて結合し、現在の入力を追加
        // string fullConversation = string.Join(, this.conversationHistoryManager.GetConversationHistoryAsString());
        string fullConversation = "";
        fullConversation += userInput;

        Debug.Log("Full Conversation: " + fullConversation);

        this.thinkingDotsTyper.StartTyping();
        StartCoroutine(chatService.RequestChatResponse(this.systemMessage, fullConversation, OnChatResponseSuccess, OnError));
    }

    public void ResetMicInputCount()
    {
        this.currentMicInputCount = 0;
    }

    private void OnChatResponseSuccess(string chatResponse)
    {
        Debug.Log("Chat Response: " + chatResponse);
        this.aiConversationManager.Set3DText(true, chatResponse);

        // this.conversationHistoryManager.SaveConversationHistory(this.lastUserInput, chatResponse);

        string ouputPath = Path.Combine(Application.dataPath, this.AIOutputFile);
        StartCoroutine(this.ttsService.ConvertTextToSpeech(
            chatResponse, ouputPath, (audioData, filePath) => OnTTSSuccess(audioData, filePath), OnError
        ));
    }

    private void OnTTSSuccess(byte[] audioData, string filePath)
    {
        StartCoroutine(PlayAndWaitForAudio(filePath));
    }

    private IEnumerator PlayAndWaitForAudio(string filePath)
    {
        yield return StartCoroutine(LoadAndPlayMP3(filePath));

        StartMicrophoneInput(); // TTS再生後に次のマイク入力を開始
    }

    private IEnumerator LoadAndPlayMP3(string filePath)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(filePath, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("MP3ファイルの読み込みに失敗しました: " + www.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);

                if (clip != null)
                {
                    this.targetAudioSource.clip = clip;
                    this.targetAudioSource.Play();
                    this.aiConversationManager.Display3DText(true);

                    while (this.targetAudioSource.isPlaying)
                    {
                        yield return null;
                    }

                    this.targetAudioSource.clip = null;
                }
                else
                {
                    Debug.LogError("Failed to convert the downloaded data into an AudioClip.");
                }
            }
        }
    }

    private void StartMicrophoneInput()
    {
        if (this.currentMicInputCount < this.maxMicInputs)
        {
            this.isWaitingForUserResponse = true;
            Debug.Log("あなたの回答を待っています...");
            this._microphoneRecorder.StartMonitoring(threshold: mic_threshold, silenceDuration: 2f, frequency: 44100);
            // this.currentMicInputCount++;
            // OnTranscriptionSuccess("これはサンプルスクリプトです。こんな感じでマイク入力を開始します。");
        }
        else
        {
            Debug.Log("Maximum number of microphone inputs reached. Ending conversation.");
            // プログラムを終了させる処理をここに追加（例えば、シーンのリロードや終了）
            if (this.aiConversationManager != null)
            {
                this.aiConversationManager.NextAction();
            }
        }
    }

    private void OnRecordingStopped(byte[] audioData, AudioClip recordedClip)
    {
        if (this.isWaitingForUserResponse && audioData != null)
        {
            this.isWaitingForUserResponse = false;
            this.currentMicInputCount++; // マイク入力回数をカウント

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string filename = "RecordedAudio_" + timestamp;

            WavUtility.SaveAsWav(filename, "__IVRC2024__/Taichi/Assets/Audio/User", recordedClip);

            Debug.Log("Sending audio data to Whisper service...");

            StartCoroutine(this.whisperService.RequestTranscription(audioData, OnTranscriptionSuccess, OnError));
        }
    }

    private void OnTranscriptionSuccess(string transcription)
    {
        // transcription が空白の場合
        if (string.IsNullOrWhiteSpace(transcription))
        {
            Debug.Log("Transcription is empty. Please try again.");
            this.currentMicInputCount--; // マイク入力回数を減らす
            // this.aiConversationManager.Reset3DText(false);

            string cannotGetText = "聞き取れませんでした。";
            if (englishmode)
            {
                cannotGetText = "I could not hear you.";
            }
            OnChatResponseSuccess(cannotGetText);
            return;
        }
        Debug.Log("Transcription: " + transcription);
        this.aiConversationManager.Set3DText(false, transcription);
        this.aiConversationManager.Display3DText(false);
        StartConversation(null, transcription);
    }

    private void OnError(string error)
    {
        Debug.LogError("Error: " + error);
    }

    public void ResetConversationHistory()
    {
        this.conversationHistoryManager.ClearConversationHistory();
    }
}
