using UnityEngine;

public class CustomSceneManager : MonoBehaviour
{
    [SerializeField] private SceneField sceneToLoad;

    // シーンを同期的に読み込むメソッド
    public void LoadScene()
    {
        SceneTransitionUtility.LoadScene(sceneToLoad.SceneName);
    }

    // 現在のシーンをリロードする
    public void ReloadScene()
    {
        SceneTransitionUtility.ReloadScene();
    }
}
