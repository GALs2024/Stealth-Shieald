using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneSwitcher : MonoBehaviour
{
    public string sceneName;  // 切り替えるシーンの名前
    public float delay = 10.0f;  // 遅延時間を設定

    private ScenePreloader scenePreloader;

    // Start メソッドでシーン切り替えを開始
    void Start()
    {
        // ScenePreloaderを同じゲームオブジェクトから取得
        this.scenePreloader = GetComponent<ScenePreloader>();

        if (scenePreloader == null)
        {
            Debug.LogError("ScenePreloaderがアタッチされていません！");
            return;
        }
        this.scenePreloader.PreloadScene(this.sceneName);

        StartCoroutine(SwitchSceneAfterDelay());
    }

    // コルーチンで指定した秒数後にシーンを切り替える
    IEnumerator SwitchSceneAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        // シーンをプリロード
        this.scenePreloader.ActivateLoadedScene();
    }

    // 実際にシーンを切り替える処理
    public void SwitchScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
