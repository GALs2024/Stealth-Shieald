using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SceneBGMData
{
    public string sceneName;
    public AudioClip clip;
    public float volume;
}

public class AudioManager : MonoBehaviour
{
    public AudioSource bgmSource;
    [SerializeField] public List<SceneBGMData> sceneBGMDataList = new List<SceneBGMData>();

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
        if (sceneBGMDataList.Count > 0)
        {
            PlayBGMForScene(sceneBGMDataList[0].sceneName);
        }
        else
        {
            Debug.LogWarning("BGMが設定されていません。");
        }
    }

    // シーンに対応したBGMを再生する
    public void PlayBGMForScene(string sceneName)
    {
        SceneBGMData bgmData = sceneBGMDataList.Find(data => data.sceneName == sceneName);

        // シーンがリストに含まれている場合
        if (bgmData != null)
        {
            AudioClip selectedBGM = bgmData.clip;

            // 再生中のBGMと同じであればリセットしない
            if (bgmSource.clip == selectedBGM && bgmSource.isPlaying)
            {
                Debug.Log("同じBGMを引き継ぐのでリセットしません: " + selectedBGM.name);
                return;
            }

            // 新しいBGMを再生
            AudioUtils.PlayBGM(bgmSource, selectedBGM, bgmData.volume, true);
            lastPlayedBGM = selectedBGM;
            currentSceneName = sceneName;
        }
        else
        {
            Debug.LogWarning("BGMが設定されていないシーンです: " + sceneName);
        }
    }
}
