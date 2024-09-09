using System.Collections;
using UnityEngine;

public class DelayedShow : MonoBehaviour
{
    public GameObject targetObject;  // 表示させる対象のオブジェクト
    public float delayTime = 5f;     // 遅延時間 (秒)

    void Start()
    {
        // 初めにオブジェクトを非表示にする
        targetObject.SetActive(false);

        // コルーチンを開始して、指定時間後にオブジェクトを表示
        StartCoroutine(ShowObjectAfterDelay());
    }

    // 指定時間後にオブジェクトを表示するコルーチン
    IEnumerator ShowObjectAfterDelay()
    {
        // 指定された秒数待つ
        yield return new WaitForSeconds(delayTime);

        // オブジェクトを表示する
        targetObject.SetActive(true);
    }
}
