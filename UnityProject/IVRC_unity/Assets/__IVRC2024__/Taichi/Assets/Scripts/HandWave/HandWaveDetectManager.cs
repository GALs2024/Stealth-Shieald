using System.Collections;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class HandWaveDetectManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clip1;  // 音声ファイル1
    public AudioClip clip2;  // 音声ファイル2

    private string pythonPath = @"Assets/__IVRC2024__/Taichi/Assets/PythonScripts/for_unity.py";  // Pythonのパスを指定
    public AIConversationManager_self aiConversationManager; 

    private Process pythonProcess;
    private bool waveDetected = false;
    private bool shouldPlayClip2 = false;  // 追加: クリップ2を再生するためのフラグ

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            UnityEngine.Debug.LogError("AudioSource コンポーネントが見つかりません。この GameObject に AudioSource をアタッチしてください。");
            return;
        }

        if (this.aiConversationManager == null)
        {
            UnityEngine.Debug.LogError("AIConversationManager がアタッチされていません。");
            return;
        }

        // 音声ファイル1を再生しながらPythonを呼び出す
        StartCoroutine(PlayAudioAndStartPython(clip1));
    }

    IEnumerator PlayAudioAndStartPython(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();

            yield return new WaitForSeconds(0.0f);

            // Pythonプロセスを開始
            StartPythonProcessAsync();

            // 音声が再生中である間は待機
            while (audioSource.isPlaying)
            {
                yield return null;  // 次のフレームまで待機
            }

            // 音声が終了したら何か追加の処理が必要であればここに記述
        }
        else
        {
            UnityEngine.Debug.LogError("AudioSourceまたはAudioClipが設定されていません。");
        }
    }

    async void StartPythonProcessAsync()
    {
        await Task.Run(() => 
        {
            StartPythonProcess();
        });
    }

    void StartPythonProcess()
    {
        pythonProcess = new Process();
        pythonProcess.StartInfo.FileName = @"python";  // Pythonのパスを指定
        pythonProcess.StartInfo.Arguments = this.pythonPath;  // スクリプトのパスを指定
        pythonProcess.StartInfo.UseShellExecute = false;
        pythonProcess.StartInfo.RedirectStandardOutput = true;
        pythonProcess.StartInfo.RedirectStandardError = true;  // エラーメッセージもリダイレクト
        pythonProcess.StartInfo.CreateNoWindow = true;
        pythonProcess.OutputDataReceived += OnPythonMessageReceived;
        pythonProcess.ErrorDataReceived += OnPythonMessageReceived;  // 標準エラーも同じハンドラで処理
        pythonProcess.EnableRaisingEvents = true;
        pythonProcess.Exited += (sender, e) =>
        {
            // Pythonプロセスが終了した後にクリップ2を再生するためのフラグを立てる
            shouldPlayClip2 = true;
        };

        pythonProcess.Start();
        pythonProcess.BeginOutputReadLine();
        pythonProcess.BeginErrorReadLine();  // 標準エラーの読み取り開始

        // 優先度を下げてUnityへの影響を最小化
        pythonProcess.PriorityClass = ProcessPriorityClass.BelowNormal;
    }

    void OnPythonMessageReceived(object sender, DataReceivedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.Data) && e.Data.StartsWith("UNITY:"))
        {
            string filteredMessage = e.Data.Substring(6);  // "UNITY:" を削除してメッセージを取得
            UnityEngine.Debug.Log("Received message from Python: " + filteredMessage);

            if (filteredMessage.Contains("Waving motion detected!"))
            {
                waveDetected = true;
                UnityEngine.Debug.Log("Unity: 手の振りを検出しました。音声ファイル2を再生します。");

                // Pythonプロセスを終了
                if (pythonProcess != null && !pythonProcess.HasExited)
                {
                    pythonProcess.Kill();
                    UnityEngine.Debug.Log("Pythonプロセスを終了しました。");
                }
            }

            // 他のメッセージを受信した場合の処理をここに追加
            if (filteredMessage.Contains("Timeout!"))
            {
                UnityEngine.Debug.Log("Unity: タイムアウトしました。");
                if (pythonProcess != null && !pythonProcess.HasExited)
                {
                    pythonProcess.Kill();
                    UnityEngine.Debug.Log("Pythonプロセスを終了しました。");
                }
            }
        }
    }

    void PlayAudioClip2()
    {
        UnityEngine.Debug.Log("PlayClip2を呼び出しました。");
        if (audioSource != null && clip2 != null)
        {
            audioSource.clip = clip2;
            audioSource.Play();
            UnityEngine.Debug.Log("clip2 を再生しています。");
        }
        shouldPlayClip2 = false;  // フラグをリセット
        this.aiConversationManager.StartConversation();  // AIとの会話を開始
    }

    void OnApplicationQuit()
    {
        if (pythonProcess != null && !pythonProcess.HasExited)
        {
            pythonProcess.Kill();
        }
    }

    void Update()
    {
        // Pythonプロセスが終了した後にクリップ2を再生
        if (shouldPlayClip2)
        {
            PlayAudioClip2();
        }

        // 手の振りを検出した後に追加の処理を行う場合
        if (waveDetected)
        {
            // 追加の処理をここに記述
        }
    }
}
