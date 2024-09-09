using UnityEngine;
using System;
using System.Collections;

public class MicrophoneRecorder : MonoBehaviour
{
    public event Action<byte[], AudioClip> OnRecordingStopped;

    private AudioClip _recordedClip;
    private bool _isRecording = false;
    private float _threshold = 0.01f;
    private float _silenceDuration = 2f;
    private float _timeSinceLastSound = 0f;
    private int _frequency = 44100;
    private int _lastMicPosition = 0;
    private float _maxRecordingTime = 8f; // 最大録音時間
    private float _autoStartRecordingDelay = 3f; // 3秒待機後に録音開始

    private Coroutine _monitoringCoroutine;

    public void StartMonitoring(float threshold, float silenceDuration, int frequency = 44100)
    {
        _threshold = threshold;
        _silenceDuration = silenceDuration;
        _frequency = frequency;

        _monitoringCoroutine = StartCoroutine(MonitorMicrophone());
    }

    public void StopMonitoring()
    {
        if (_isRecording)
        {
            StopRecording();
        }
        if (_monitoringCoroutine != null)
        {
            StopCoroutine(_monitoringCoroutine);
            _monitoringCoroutine = null;
        }
        Debug.Log("MicrophoneRecorder: Monitoring stopped.");
        Microphone.End(null);  // マイク入力を停止
    }

    private IEnumerator MonitorMicrophone()
    {
        _recordedClip = Microphone.Start(null, true, 10, _frequency);
        int sampleSize = 256;
        float[] samples = new float[sampleSize];

        float _timeSinceStartMonitoring = 0f; // 監視開始時間をリセット

        while (true)
        {
            _timeSinceStartMonitoring += Time.deltaTime;

            int microphonePosition = Microphone.GetPosition(null);
            if (microphonePosition < _lastMicPosition)
            {
                _lastMicPosition = 0; // ループした場合
            }

            _recordedClip.GetData(samples, _lastMicPosition);

            float maxLevel = 0f;
            foreach (float sample in samples)
            {
                if (sample > maxLevel)
                {
                    maxLevel = sample;
                }
            }

            // 音声が検知されるか、3秒経過したら録音開始
            if (maxLevel > _threshold || _timeSinceStartMonitoring >= this._autoStartRecordingDelay)
            {
                if (!_isRecording)
                {
                    _isRecording = true;
                    StartRecording();
                }
                if (maxLevel > _threshold)
                {
                    _timeSinceLastSound = 0f;
                }
            }

            // 録音中の場合
            if (_isRecording)
            {
                _timeSinceLastSound += Time.deltaTime;

                // 録音を最大10秒までに制限
                if (_timeSinceStartMonitoring >= this._maxRecordingTime || _timeSinceLastSound > _silenceDuration)
                {
                    _isRecording = false;
                    StopRecording();
                }
            }

            _lastMicPosition = microphonePosition;
            yield return null;
        }
    }

    private void StartRecording()
    {
        Debug.Log("Recording started.");
        _timeSinceLastSound = 0f; // 録音開始時に時間をリセット
    }

    private void StopRecording()
    {
        Debug.Log("Recording stopped.");

        if (Microphone.IsRecording(null))
        {
            Microphone.End(null);
        }

        // 実際の録音データから_silenceDuration分のサンプルを削除
        AudioClip trimmedClip = TrimSilenceDuration(_recordedClip, _lastMicPosition, _silenceDuration);

        byte[] audioData = WavUtility.FromAudioClip(trimmedClip);
        OnRecordingStopped?.Invoke(audioData, trimmedClip);

        if (_monitoringCoroutine != null)
        {
            StopCoroutine(_monitoringCoroutine);
            _monitoringCoroutine = null;
        }
    }

    private AudioClip TrimSilenceDuration(AudioClip clip, int lastMicPosition, float silenceDuration)
    {
        int samplesPerChannel = lastMicPosition;
        int samplesToTrim = Mathf.CeilToInt(_frequency * silenceDuration);

        int newSampleCount = Mathf.Max(0, samplesPerChannel - samplesToTrim);
        if (newSampleCount <= 0)
        {
            Debug.LogWarning("TrimSilenceDuration: No valid audio left after trimming.");
            return clip; // トリム後に音声がなくなる場合、元のクリップを返す
        }

        float[] originalData = new float[samplesPerChannel * clip.channels];
        clip.GetData(originalData, 0);

        float[] trimmedData = new float[newSampleCount * clip.channels];
        Array.Copy(originalData, 0, trimmedData, 0, newSampleCount * clip.channels);

        AudioClip trimmedClip = AudioClip.Create(clip.name + "_trimmed", newSampleCount, clip.channels, clip.frequency, false);
        trimmedClip.SetData(trimmedData, 0);

        return trimmedClip;
    }
}
