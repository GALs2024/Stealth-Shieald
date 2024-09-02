using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;

public class RandomAudioPlayer : MonoBehaviour
{
    public string folderPath = @"Assets\Audios\User"; // WAVファイルが格納されているフォルダのパス
    public float maxRandomDelay = 3.0f; // ランダムなディレイの最大値（秒）

    private List<AudioClip> audioClips = new List<AudioClip>();
    private AudioSource audioSource;

     public float audioClipsVolume = 3.0f;

    public void Init()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        StartCoroutine(LoadAudioClips());
    }

    IEnumerator LoadAudioClips()
    {
        // フォルダ内のすべてのWAVファイルパスを取得
        string[] files = Directory.GetFiles(folderPath, "*.wav");

        foreach (string file in files)
        {
            // ファイルパスのバックスラッシュをスラッシュに変換
            string filePath = "file:///" + file.Replace("\\", "/");
            yield return StartCoroutine(LoadClip(filePath));
        }

        PlayAudioClipsWithRandomDelay();
    }

    IEnumerator LoadClip(string path)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Failed to load audio clip: " + path);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                audioClips.Add(clip);
            }
        }
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
    }
}
