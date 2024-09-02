using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class ConversationalAI : MonoBehaviour
{
    private string lastUserInput;

    private OpenAIChat chatService;
    private OpenAIWhisper whisperService;
    private OpenAITTS ttsService;
    private MicrophoneRecorder _microphoneRecorder;

    [SerializeField]
    private string systemMessage = "質問しながら短く答えてください";
    [SerializeField]
    private int maxTokens = 20;
    [SerializeField]
    private AudioClip startAudioClip; // 最初に再生する音声ファイル
    [SerializeField]
    private int maxMicInputs = 1; // 指定された回数のマイク入力
    private int currentMicInputCount = 0; // 現在のマイク入力回数
    public float mic_threshold = 0.001f;

    private bool isWaitingForUserResponse = false;

    // private AudioSource audioSource;

    private string savedTranscriptionsPath = @"Assets/__IVRC2024__/Taichi/Assets/Data/speak_contents.txt";
    private AIConversationManager aiConversationManager;
    private ConversationHistoryManager conversationHistoryManager;
    public GameObject avatarObject;
    private AudioSource targetAudioSource;

    public void Initialize()
    {
        var apiKey = ApiKeyLoader.LoadApiKey();

        // if (audioSource == null)
        // {
        //     audioSource = gameObject.AddComponent<AudioSource>();
        // }

        // AIConversationManagerを探して設定
        aiConversationManager = FindObjectOfType<AIConversationManager>();

        this.targetAudioSource = avatarObject.GetComponent<AudioSource>();

        whisperService = new OpenAIWhisper(apiKey, "whisper-1");
        chatService = new OpenAIChat(apiKey, "gpt-4o-mini", systemMessage, maxTokens);
        ttsService = new OpenAITTS(apiKey, "tts-1", "alloy", "__IVRC2024__/Taichi/Assets/Audio/AIOutput");
        
        _microphoneRecorder = GetComponent<MicrophoneRecorder>();
        _microphoneRecorder.OnRecordingStopped += OnRecordingStopped;

        // savedTranscriptionsPath = Path.Combine(Application.persistentDataPath, "transcriptions.txt");

        conversationHistoryManager = new ConversationHistoryManager();
    }

    void OnDestroy()
    {
        _microphoneRecorder.OnRecordingStopped -= OnRecordingStopped;
    }

    public void SetStartAudioClip(AudioClip clip)
    {
        currentMicInputCount = 0; // マイク入力回数をリセット
        startAudioClip = clip;
    }

    public void PlayStartAudio()
    {
        if (startAudioClip != null)
        {
            this.targetAudioSource.clip = startAudioClip;
            this.targetAudioSource.Play();
            StartCoroutine(WaitForAudioToEnd()); // 音声が終わるのを待つ
        }
        else
        {
            Debug.LogError("Start audio clip is not assigned.");
        }
    }

    private IEnumerator WaitForAudioToEnd()
    {
        while (this.targetAudioSource.isPlaying)
        {
            yield return null;
        }

        StartMicrophoneInput(); // 音声再生後にマイク入力を開始
    }

    private void StartMicrophoneInput()
    {
        if (currentMicInputCount < maxMicInputs)
        {
            isWaitingForUserResponse = true;
            Debug.Log("あなたの回答を待っています...");
            _microphoneRecorder.StartMonitoring(threshold: mic_threshold, silenceDuration: 2f, frequency: 44100);
        }
        else
        {
            Debug.Log("Maximum number of microphone inputs reached. Ending conversation.");
            // プログラムを終了させる処理をここに追加（例えば、シーンのリロードや終了）
            if (aiConversationManager != null)
            {
                aiConversationManager.OnAudioFinished(); // 次のオーディオクリップに移行
            }
        }
    }

    private void StartConversation(string userInput)
    {
        // ユーザー入力を保持
        lastUserInput = userInput;

        // 会話履歴の内容をすべて結合し、現在の入力を追加
        string fullConversation = string.Join(" ", conversationHistoryManager.GetConversationHistoryAsString());
        fullConversation += " User: " + userInput;

        Debug.Log("Full Conversation: " + fullConversation);

        // unityの実行を終了する
        // return;
        this.systemMessage = "質問しながら短く答えてください";
        Debug.Log("currentMicInputCount: " + this.currentMicInputCount);
        if (this.currentMicInputCount == this.maxMicInputs) {
            this.systemMessage = "楽しくオウム返しをしてください";
        }

        Debug.Log("systemMessage: " + this.systemMessage);
        StartCoroutine(chatService.RequestChatResponse(this.systemMessage, fullConversation, OnChatResponseSuccess, OnError));
    }

    private void OnChatResponseSuccess(string chatResponse)
    {
        Debug.Log("Chat Response: " + chatResponse);

        conversationHistoryManager.SaveConversationHistory(lastUserInput, chatResponse);

        StartCoroutine(ttsService.ConvertTextToSpeech(
            chatResponse, (audioData, filePath) => OnTTSSuccess(audioData, filePath), OnError
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

    private void OnRecordingStopped(byte[] audioData, AudioClip recordedClip)
    {
        if (isWaitingForUserResponse && audioData != null)
        {
            isWaitingForUserResponse = false;
            currentMicInputCount++; // マイク入力回数をカウント

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string filename = "RecordedAudio_" + timestamp;

            WavUtility.SaveAsWav(filename, recordedClip);

            StartCoroutine(whisperService.RequestTranscription(audioData, OnTranscriptionSuccess, OnError));
        }
    }

    private void OnTranscriptionSuccess(string transcription)
    {
        Debug.Log("Transcription: " + transcription);
        SaveTranscription(transcription);
        StartConversation(transcription);
    }

    private void SaveTranscription(string transcription)
    {
        try
        {
            File.AppendAllText(savedTranscriptionsPath, transcription + Environment.NewLine);
            Debug.Log("Transcription saved to: " + savedTranscriptionsPath);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save transcription: " + e.Message);
        }
    }

    private void OnError(string error)
    {
        Debug.LogError("Error: " + error);
    }
}
