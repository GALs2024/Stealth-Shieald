using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public AudioSource bgmSource;
    public List<AudioClip> bgmClips;
    [SerializeField] public List<string> bgmInheritedSceneNames = new List<string>();

    public static AudioManager instance;
    private string currentSceneName = "";
    private AudioClip lastPlayedBGM;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 最初のBGMを再生する
        if (bgmClips.Count > 0)
        {
            PlayBGMForScene(bgmInheritedSceneNames[0]);
        }
        else
        {
            Debug.LogWarning("BGMが設定されていません。");
        }
    }

    // シーンに対応したBGMを再生する
    public void PlayBGMForScene(string sceneName)
    {
        int sceneIndex = bgmInheritedSceneNames.IndexOf(sceneName);

        // シーンがリストに含まれている場合
        if (sceneIndex >= 0 && sceneIndex < bgmClips.Count)
        {
            AudioClip selectedBGM = bgmClips[sceneIndex];

            // 再生中のBGMと同じであればリセットしない
            Debug.Log(bgmSource.clip);
            Debug.Log(selectedBGM);
            if (bgmSource.clip == selectedBGM && bgmSource.isPlaying)
            {
                Debug.Log("同じBGMを引き継ぐのでリセットしません: " + selectedBGM.name);
                return;
            }

            // 新しいBGMを再生（または停止してリセット）
            bgmSource.clip = selectedBGM;
            bgmSource.Play();
            lastPlayedBGM = selectedBGM;
            currentSceneName = sceneName;
        }
        else
        {
            Debug.LogWarning("BGMが設定されていないシーンです: " + sceneName);
        }
    }
}
