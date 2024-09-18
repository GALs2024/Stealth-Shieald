using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionUtility : MonoBehaviour
{
    // シングルトンインスタンス
    public static SceneTransitionUtility Instance { get; private set; }

    private void Awake()
    {
        // シングルトンの設定
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 指定したシーンを同期的に読み込む
    /// </summary>
    public void LoadScene(string sceneName)
    {
        if (SceneExists(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"シーン '{sceneName}' は存在しません");
        }
    }

    /// <summary>
    /// 指定したシーンを非同期的に読み込む
    /// </summary>
    public void LoadSceneAsync(string sceneName, Action onComplete = null)
    {
        if (SceneExists(sceneName))
        {
            StartCoroutine(LoadSceneAsyncCoroutine(sceneName, onComplete));
        }
        else
        {
            Debug.LogError($"シーン '{sceneName}' は存在しません");
        }
    }

    /// <summary>
    /// 現在のシーンをリロードする
    /// </summary>
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// 非同期シーン読み込みのコルーチン
    /// </summary>
    private IEnumerator LoadSceneAsyncCoroutine(string sceneName, Action onComplete = null)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;

        // 読み込み進捗を表示したり、他の処理を実行可能
        while (!asyncOperation.isDone)
        {
            if (asyncOperation.progress >= 0.9f)
            {
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }

        // 読み込み完了後のコールバック
        onComplete?.Invoke();
    }

    /// <summary>
    /// シーンが存在するか確認する
    /// </summary>
    private bool SceneExists(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            if (name == sceneName)
            {
                return true;
            }
        }
        return false;
    }
}
