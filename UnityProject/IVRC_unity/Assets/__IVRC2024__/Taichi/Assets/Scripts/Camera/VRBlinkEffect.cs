using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VRBlinkEffect : MonoBehaviour
{
    public Image fadeImage;
    [SerializeField]
    private float fadeInDuration = 0.1f;  // フェードインの時間
    [SerializeField]
    private float holdDuration = 0.5f;    // 黒くなっている時間
    [SerializeField]
    private float fadeOutDuration = 0.1f; // フェードアウトの時間
    public Transform cameraTransform;

    private void Start()
    {
        fadeImage.color = new Color(0, 0, 0, 0);
    }

    private void Update()
    {
        // カメラの位置に合わせてUIの位置を調整
        transform.position = cameraTransform.position + cameraTransform.forward * 0.5f;
        transform.rotation = cameraTransform.rotation;
    }

    public void Test(int[] test)
    {
        Debug.Log("Test");
    }

    public void Blink()
    {
        StartCoroutine(BlinkCoroutine());
    }

    // シグナルによってfadeInDurationを調整するメソッド
    public void SetFadeInDuration(float duration)
    {
        fadeInDuration = duration;
    }

    // シグナルによってholdDurationを調整するメソッド
    public void SetHoldDuration(float duration)
    {
        holdDuration = duration;
    }

    // シグナルによってfadeOutDurationを調整するメソッド
    public void SetFadeOutDuration(float duration)
    {
        fadeOutDuration = duration;
    }

    private IEnumerator BlinkCoroutine()
    {
        // フェードイン（黒くなる）
        float time = 0;
        while (time < fadeInDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, time / fadeInDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // ホールド（黒くなっている時間）
        yield return new WaitForSeconds(holdDuration);

        // フェードアウト（元に戻る）
        time = 0;
        while (time < fadeOutDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, time / fadeOutDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }
}
