using UnityEngine;

public class MainSoundScript : MonoBehaviour {
    public static MainSoundScript instance = null;
    public bool DontDestroyEnabled = true;

    // AwakeはStartより先に呼ばれる
    void Awake () {
        if (instance == null) {
            instance = this;
            if (DontDestroyEnabled) {
                // シーンを遷移してもオブジェクトが消えないようにする
                DontDestroyOnLoad (this.gameObject);
            }
        } else {
            // すでに存在するインスタンスがあればこのオブジェクトを破棄
            Destroy (this.gameObject);
        }
    }
}
