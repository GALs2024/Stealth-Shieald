using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // シングルトンインスタンス
    public static SceneLoader Instance { get; private set; }

    private void Awake()
    {
        // シングルトンの設定
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンをまたいで保持
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// シーンを同期的に読み込む
    /// </summary>
    /// <param name="sceneName">読み込むシーンの名前</param>
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
    /// シーンを非同期的に読み込む
    /// </summary>
    /// <param name="sceneName">読み込むシーンの名前</param>
    /// <param name="onComplete">読み込み完了時のコールバック</param>
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

    // 非同期シーン読み込みのコルーチン
    private IEnumerator LoadSceneAsyncCoroutine(string sceneName, Action onComplete = null)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;

        // 読み込み進捗を表示したり、他の処理を実行可能
        while (!asyncOperation.isDone)
        {
            // 読み込みが90%以上完了したらシーンを有効化
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
    /// <param name="sceneName">確認するシーンの名前</param>
    /// <returns>シーンが存在する場合はtrue</returns>
    private bool SceneExists(string sceneName)
    {
        // ビルド設定に登録されているシーンがあるかチェック
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
