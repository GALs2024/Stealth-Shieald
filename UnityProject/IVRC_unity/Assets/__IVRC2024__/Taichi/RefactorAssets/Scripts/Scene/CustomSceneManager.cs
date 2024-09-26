using UnityEngine;

public class CustomSceneManager : MonoBehaviour
{
    [SerializeField] private SceneField sceneToLoad;

    // シーンを同期的に読み込むメソッド
    public void LoadScene()
    {
        LoadSceneWithBGM(sceneToLoad.SceneName);
    }

    // 現在のシーンをリロードする
    public void ReloadScene()
    {
        SceneTransitionUtility.ReloadScene();

    }

    // シーンをロードし、BGMを設定する
    private void LoadSceneWithBGM(string sceneName)
    {
        SceneTransitionUtility.LoadScene(sceneName);

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayBGMForScene(sceneName);
        }
        else
        {
            Debug.LogWarning("AudioManagerが見つかりません。");
        }
    }
}
