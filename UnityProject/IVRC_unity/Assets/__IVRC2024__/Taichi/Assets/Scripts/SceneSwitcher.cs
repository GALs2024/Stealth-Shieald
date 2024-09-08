using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneSwitcher : MonoBehaviour
{
    public string sceneName;  // 切り替えるシーンの名前
    public float delay = 10.0f;  // 遅延時間を設定

    // Start メソッドでシーン切り替えを開始
    void Start()
    {
        StartCoroutine(SwitchSceneAfterDelay());
    }

    // コルーチンで指定した秒数後にシーンを切り替える
    IEnumerator SwitchSceneAfterDelay()
    {
        yield return new WaitForSeconds(delay);  // 10秒待機
        SwitchScene();  // シーンを切り替え
    }

    // 実際にシーンを切り替える処理
    public void SwitchScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
