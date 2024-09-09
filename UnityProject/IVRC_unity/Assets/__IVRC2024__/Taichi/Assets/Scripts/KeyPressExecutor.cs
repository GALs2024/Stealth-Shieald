using UnityEngine;

public class KeyPressController : MonoBehaviour
{
    public KeyCode keyToPress = KeyCode.Space;  // 監視するキーを指定
    public GameObject targetObject;             // 操作するゲームオブジェクトを指定

    void Update()
    {
        if (Input.GetKeyDown(keyToPress))
        {
            if (targetObject != null)
            {
                // 対象のゲームオブジェクトにアタッチされているスクリプトを取得
                HandWaveDetectManager handWaveDetectManager = targetObject.GetComponent<HandWaveDetectManager>();

                if (handWaveDetectManager != null)
                {
                    // ここでスクリプトの機能を呼び出す
                    handWaveDetectManager.Init(); // 例としてDoSomething()メソッドを呼び出す
                }
                else
                {
                    Debug.LogWarning("指定したゲームオブジェクトにMyScriptがアタッチされていません。");
                }
            }
            else
            {
                Debug.LogWarning("ターゲットのゲームオブジェクトが設定されていません。");
            }
        }
    }
}
