using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;
using System.Linq;

public class Teat_RandomAudioPlayer : MonoBehaviour
{
    public string inputDirPath = @"__IVRC2024__/Taichi/Assets/Audio/User"; // WAVファイルが格納されているフォルダのパス

    private List<AudioClip> audioClips = new List<AudioClip>();
    public AudioSource audioSource;

    public float maxRandomDelay = 1.0f;
    public float audioClipsVolume = 1.0f;

    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        LoadAudioClips();
    }

    void LoadAudioClips()
    {
        string _inputDirPath = Path.Combine(Application.dataPath, this.inputDirPath);
        string[] wavFiles = Directory.GetFiles(_inputDirPath, "*.wav");
        string[] mp3Files = Directory.GetFiles(_inputDirPath, "*.mp3");

        string[] allFiles = wavFiles.Concat(mp3Files).ToArray();

        foreach (string file in allFiles)
        {
            string filePath = "file:///" + file.Replace("\\", "/");
            LoadClip(filePath);
        }
    }

    void LoadClip(string path)
    {
        string extension = Path.GetExtension(path).ToLower();

        if (extension == ".meta")
        {
            return;
        }

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
            return;
        }

        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, audioType);
        www.SendWebRequest();

        while (!www.isDone)
        {
            // 同期的にリクエストを待つ
        }

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Failed to load audio clip: " + path);
        }
        else
        {
            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);

            NormalizeAudioClip(clip);

            audioClips.Add(clip);
        }
    }

    void NormalizeAudioClip(AudioClip clip)
    {
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        float maxSampleValue = samples.Max(Mathf.Abs);

        if (maxSampleValue == 0)
        {
            return;
        }

        float normalizationFactor = 1.0f / maxSampleValue;
        for (int i = 0; i < samples.Length; i++)
        {
            samples[i] *= normalizationFactor;
        }

        clip.SetData(samples, 0);
    }

    // ランダムディレイを管理するメソッド
    public void PlayAudioClipsWithRandomDelay()
    {
        foreach (var clip in audioClips)
        {
            StartCoroutine(PlayClipWithDelay(clip));
        }
    }

    // 指定されたディレイ後にクリップを再生
    IEnumerator PlayClipWithDelay(AudioClip clip)
    {
        float delay = Random.Range(0f, maxRandomDelay);
        yield return new WaitForSeconds(delay);

        audioSource.PlayOneShot(clip, audioClipsVolume);
    }

    // フェードアウトを関数化して任意のタイミングで呼び出せるようにする
    public void StartFadeOut(float fadeDuration)
    {
        StartCoroutine(FadeOutVolume(fadeDuration));
    }

    IEnumerator FadeOutVolume(float fadeDuration)
    {
        float startVolume = audioSource.volume;

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        audioSource.volume = 0f;
    }
}
