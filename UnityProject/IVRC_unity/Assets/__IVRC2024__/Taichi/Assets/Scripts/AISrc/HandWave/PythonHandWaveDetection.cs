using System.Diagnostics;
using UnityEngine;

public class PythonHandWaveDetection : MonoBehaviour
{
    private Process pythonProcess;

    void Start()
    {
        UnityEngine.Debug.Log("Start PythonHandWaveDetection");
        StartPythonProcess();
    }

    void StartPythonProcess()
    {
        pythonProcess = new Process();
        pythonProcess.StartInfo.FileName = @"python";  // Pythonのパスを指定
        pythonProcess.StartInfo.Arguments = @"Assets/PythonScripts/for_unity.py";  // スクリプトのパスを指定
        pythonProcess.StartInfo.UseShellExecute = false;
        pythonProcess.StartInfo.RedirectStandardOutput = true;
        pythonProcess.StartInfo.RedirectStandardError = true;  // エラーメッセージもリダイレクト
        pythonProcess.StartInfo.CreateNoWindow = true;
        pythonProcess.OutputDataReceived += OnPythonMessageReceived;
        pythonProcess.ErrorDataReceived += OnPythonMessageReceived;  // 標準エラーも同じハンドラで処理
        pythonProcess.Start();
        pythonProcess.BeginOutputReadLine();
        pythonProcess.BeginErrorReadLine();  // 標準エラーの読み取り開始
    }

    void OnPythonMessageReceived(object sender, DataReceivedEventArgs e)
    {
         if (!string.IsNullOrEmpty(e.Data) && e.Data.StartsWith("UNITY:"))
        {
            string filteredMessage = e.Data.Substring(6);  // "UNITY:" を削除してメッセージを取得
            UnityEngine.Debug.Log(filteredMessage);

            if (filteredMessage.Contains("Unity: Hand wave detected."))
            {
                UnityEngine.Debug.Log("手の振りを検出しました。Pythonプログラムを終了し、追加の処理を実行します。");
                pythonProcess.Kill();

                // ExecuteAdditionalCode();
            }
        }
    }

    void OnApplicationQuit()
    {
        if (pythonProcess != null && !pythonProcess.HasExited)
        {
            pythonProcess.Kill();
        }
    }
}