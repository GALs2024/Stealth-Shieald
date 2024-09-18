using UnityEngine;

public class CustomSceneManager : MonoBehaviour
{
    // インスペクターから設定するシーン
    [SerializeField] private SceneField sceneToLoad;

    // フェード付きでシーンを読み込むかどうか
    [SerializeField] private bool useFadeTransition = false;

    void Start()
    {
        LoadScene();
    }

    /// <summary>
    /// シーンを同期的に読み込むメソッド
    /// </summary>
    public void LoadScene()
    {
        SceneTransitionUtility.Instance.LoadScene(sceneToLoad.SceneName);
    }

    /// <summary>
    /// シーンを非同期的に読み込むメソッド
    /// </summary>
    public void LoadSceneAsync()
    {
        SceneTransitionUtility.Instance.LoadSceneAsync(sceneToLoad.SceneName, () =>
        {
            Debug.Log($"シーン '{sceneToLoad.SceneName}' の非同期読み込みが完了しました");
        });
    }

    /// <summary>
    /// 現在のシーンをリロードする
    /// </summary>
    public void ReloadScene()
    {
        SceneTransitionUtility.Instance.ReloadScene();
    }
}
