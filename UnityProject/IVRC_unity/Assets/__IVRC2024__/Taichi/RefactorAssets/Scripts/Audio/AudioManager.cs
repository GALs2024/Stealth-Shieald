using UnityEngine;
using UnityEngine.SceneManagement;  // シーン管理のために追加
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
        Debug.Log("AudioManager Awake");
        if (instance == null)
        {
            Debug.Log("AudioManager instance is null");
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log("AudioManager instance is not null");
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        Debug.Log("AudioManager OnEnable");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        Debug.Log("AudioManager OnDisable");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // シーンがロードされたときに呼び出される
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        PlayBGMForScene(scene.name);
    }

    void Start()
    {
        string startingSceneName = SceneManager.GetActiveScene().name;
        PlayBGMForScene(startingSceneName);
    }

    public void PlayBGMForScene(string sceneName)
    {
        SceneBGMData bgmData = sceneBGMDataList.Find(data => data.sceneName == sceneName);

        if (bgmData != null)
        {
            AudioClip selectedBGM = bgmData.clip;

            // BGMが設定されていない（AudioClipがNoneまたはnull）の場合
            if (selectedBGM == null)
            {
                Debug.Log("AudioClipがNoneのため、BGMは再生されません。シーン名: " + sceneName);

                if (bgmSource.isPlaying)
                {
                    bgmSource.Stop();
                    bgmSource.clip = null;
                }

                return;
            }

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

            // BGMが設定されていない場合も再生中のBGMを停止する（引き継がない）
            if (bgmSource.isPlaying)
            {
                bgmSource.Stop();
                bgmSource.clip = null; // 現在のBGMクリップをクリアする
            }
        }
    }
}
