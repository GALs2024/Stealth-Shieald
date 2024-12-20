using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;
using System.Linq;

public class RandomAudioPlayer : MonoBehaviour
{
    public string inputDirPath = @"__IVRC2024__/Taichi/Assets/Audio/User"; // WAVファイルが格納されているフォルダのパス
    public float maxRandomDelay = 3.0f; // ランダムなディレイの最大値（秒）
    public float initDelay = 5.0f; // Init()を実行するまでの遅延時間（秒）

    private List<AudioClip> audioClips = new List<AudioClip>();
    private AudioSource audioSource;

    public float audioClipsVolume = 1.0f;
    public float fadeDuration = 5.0f; // フェードアウトの持続時間
    public float delayBeforeFade = 10.0f; // フェードアウトを開始する前の遅延時間

    public bool playStart = false;
    public bool fadeOutEnabled = true; // フェードアウトの有無を設定するフラグ

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        if (playStart)
        {
            // 指定された時間後にInit()を実行
            StartCoroutine(DelayedInit(initDelay));
        }
    }

    // 指定時間の遅延後にInitを呼び出すコルーチン
    public IEnumerator DelayedInit(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(LoadAudioClips());
    }

    IEnumerator LoadAudioClips()
    {
        // フォルダ内のすべてのWAVファイルパスを取得
        string _inputDirPath = Path.Combine(Application.dataPath, this.inputDirPath);
        string[] wavFiles = Directory.GetFiles(_inputDirPath, "*.wav");
        string[] mp3Files = Directory.GetFiles(_inputDirPath, "*.mp3");

        // 2つの配列を結合
        string[] allFiles = wavFiles.Concat(mp3Files).ToArray();

        foreach (string file in allFiles)
        {
            // ファイルパスのバックスラッシュをスラッシュに変換
            string filePath = "file:///" + file.Replace("\\", "/");
            yield return StartCoroutine(LoadClip(filePath));
        }

        PlayAudioClipsWithRandomDelay();
    }

    IEnumerator LoadClip(string path)
    {
        // ファイルの拡張子を取得し、小文字に変換
        string extension = Path.GetExtension(path).ToLower();

        // .meta ファイルは無視する
        if (extension == ".meta")
        {
            yield break; // .meta ファイルなら処理を中断
        }

        // AudioTypeを動的に設定
        AudioType audioType;
        if (extension == ".wav")
        {
            audioType = AudioType.WAV;
        }
        else if (extension == ".mp3")
        {
            audioType = AudioType.MPEG;
        }
        else
        {
            Debug.LogError("Unsupported audio format: " + path);
            yield break; // 対応していない形式の場合は処理を中断
        }

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, audioType))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Failed to load audio clip: " + path);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);

                // オーディオクリップの正規化
                NormalizeAudioClip(clip);

                audioClips.Add(clip);
            }
        }
    }

    // AudioClipを正規化するメソッド
    void NormalizeAudioClip(AudioClip clip)
    {
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        float maxSampleValue = samples.Max(Mathf.Abs); // すべてのサンプルの絶対値の中で最大のものを取得

        // 最大値が0なら何もしない（無音ファイル）
        if (maxSampleValue == 0)
        {
            return;
        }

        // サンプル値を最大値に基づいて正規化
        float normalizationFactor = 1.0f / maxSampleValue;
        for (int i = 0; i < samples.Length; i++)
        {
            samples[i] *= normalizationFactor;
        }

        clip.SetData(samples, 0);
    }

    void PlayAudioClipsWithRandomDelay()
    {
        foreach (var clip in audioClips)
        {
            StartCoroutine(PlayClipWithDelay(clip));
        }
    }

    IEnumerator PlayClipWithDelay(AudioClip clip)
    {
        // ランダムなディレイを計算
        float delay = Random.Range(0f, maxRandomDelay);
        yield return new WaitForSeconds(delay);

        // AudioSource.PlayOneShot()でクリップを再生
        audioSource.PlayOneShot(clip, audioClipsVolume);

        // フェードアウトが有効な場合のみ、指定した時間待機してからフェードアウトを開始
        if (fadeOutEnabled)
        {
            yield return new WaitForSeconds(delayBeforeFade);
            StartCoroutine(FadeOutVolume());
        }
    }

    IEnumerator FadeOutVolume()
    {
        float startVolume = audioSource.volume;

        // 指定したフェード時間内で音量を0にする
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        // 音量を完全に0に設定
        audioSource.volume = 0f;
    }
}
