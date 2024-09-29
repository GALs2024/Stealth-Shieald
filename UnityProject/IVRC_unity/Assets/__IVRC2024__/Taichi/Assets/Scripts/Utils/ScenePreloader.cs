using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScenePreloader : MonoBehaviour
{
    private AsyncOperation asyncLoad;
    public MosaicGenerator mosaicGenerator;  // MosaicGeneratorの参照を取得

    // シーンをバックグラウンドでロードしておく
    public void PreloadScene(string sceneName)
    {
        StartCoroutine(PreloadAsync(sceneName));
    }

    private IEnumerator PreloadAsync(string sceneName)
    {
        asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            Debug.Log("Preloading progress: " + (asyncLoad.progress * 100) + "%");
            yield return null;

            // ロードが90%以上進んだらシーンをアクティベートできる状態になる
            if (asyncLoad.progress >= 0.9f)
            {
                Debug.Log("Scene is ready to be activated.");
                break;
            }
        }
    }

    // プレイヤーが準備できたときにシーンをアクティベートし、その後モザイク生成を開始
    public void ActivateLoadedScene()
    {
        if (asyncLoad != null && asyncLoad.progress >= 0.9f)
        {
            asyncLoad.allowSceneActivation = true;
            StartCoroutine(WaitForSceneActivationAndRunMosaic());
        }
    }

    private IEnumerator WaitForSceneActivationAndRunMosaic()
    {
        // シーンアクティベーション後にモザイク生成を開始
        yield return new WaitForEndOfFrame();  // シーンが完全にアクティブになるまで1フレーム待つ
        if (mosaicGenerator != null)
        {
            // 非同期的にMosaicGeneratorを実行
            StartCoroutine(mosaicGenerator.GenerateMosaicAsync());
        }
    }
}
