using UnityEngine;
using UnityEngine.Audio;

public static class AudioMixerUtils
{
    /// <summary>
    /// オーディオミキサーのボリュームをセットする（デシベルベース）
    /// </summary>
    public static void SetVolume(AudioMixer mixer, string parameterName, float volume)
    {
        if (mixer == null) return;

        // 0~1の音量を-80～0のデシベルに変換
        mixer.SetFloat(parameterName, Mathf.Log10(volume) * 20);
    }

    /// <summary>
    /// オーディオミキサーのボリュームを取得する（デシベルベース）
    /// </summary>
    public static float GetVolume(AudioMixer mixer, string parameterName)
    {
        if (mixer == null) return 0f;

        float value;
        mixer.GetFloat(parameterName, out value);

        // デシベルを0~1の範囲に変換
        return Mathf.Pow(10, value / 20);
    }
}
