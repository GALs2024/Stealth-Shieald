using System.IO;
using UnityEngine;
using System;

public static class WavUtility
{
    public static void SaveAsWav(string filename, string relDirPath, AudioClip clip)
    {
        try
        {
            // 保存パスを設定（"Assets/Audio/User/任意の名前.wav"）
            string directoryPath = Path.Combine(Application.dataPath, relDirPath);
            string filePath = Path.Combine(directoryPath, filename + ".wav");

            // ディレクトリが存在しない場合は作成する
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // AudioClipをWAV形式のバイト配列に変換
            byte[] wavData = FromAudioClip(clip);

            // ファイルとして保存
            File.WriteAllBytes(filePath, wavData);

            Debug.Log($"Saved WAV file to {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving WAV file: " + e.Message);
        }
    }

    public static void DeleteAllFilesInDirectory(string relDirPath)
    {
        string directoryPath = Path.Combine(Application.dataPath, relDirPath);
        if (Directory.Exists(directoryPath))
        {
            string[] files = Directory.GetFiles(directoryPath);
            foreach (string file in files)
            {
                File.Delete(file);
            }
        }
    }

    // AudioClipをWAV形式のバイト配列に変換するメソッド
    public static byte[] FromAudioClip(AudioClip clip)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            WriteWavFile(stream, clip);
            return stream.ToArray();
        }
    }

    // WAVファイル形式でデータを書き込む内部メソッド
    private static void WriteWavFile(Stream stream, AudioClip clip)
    {
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            int headerSize = 44;
            int fileSize = clip.samples * clip.channels * 2 + headerSize;
            int samples = clip.samples;

            writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
            writer.Write(fileSize - 8);
            writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));
            writer.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
            writer.Write(16);
            writer.Write((short)1);
            writer.Write((short)clip.channels);
            writer.Write(clip.frequency);
            writer.Write(clip.frequency * clip.channels * 2);
            writer.Write((short)(clip.channels * 2));
            writer.Write((short)16);
            writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));
            writer.Write(samples * clip.channels * 2);

            float[] data = new float[samples * clip.channels];
            clip.GetData(data, 0);
            foreach (var sample in data)
            {
                short val = (short)(sample * short.MaxValue);
                writer.Write(val);
            }
        }
    }

    public static AudioClip ToAudioClip(byte[] wavFile)
    {
        // WAVファイルのヘッダーを解析
        int channels = BitConverter.ToInt16(wavFile, 22);
        int sampleRate = BitConverter.ToInt32(wavFile, 24);
        int dataSize = BitConverter.ToInt32(wavFile, 40);
        
        // サンプルデータのスタート位置（44バイト目から）
        int startIndex = 44;

        // サンプル数の計算
        int sampleCount = dataSize / 2; // 16ビットの場合、2バイト = 1サンプル

        // サンプルデータをfloat配列に変換
        float[] samples = new float[sampleCount];
        for (int i = 0; i < sampleCount; i++)
        {
            short sample = BitConverter.ToInt16(wavFile, startIndex + i * 2);
            samples[i] = sample / 32768f; // 16ビットPCMなので、-32768〜32767を-1〜1に正規化
        }

        // AudioClipを生成
        AudioClip audioClip = AudioClip.Create("WavAudio", sampleCount, channels, sampleRate, false);
        audioClip.SetData(samples, 0);

        return audioClip;
    }

    private static float[] ConvertWavToSamples(byte[] audioData)
    {
        AudioClip audioClip = ToAudioClip(audioData); // 仮のメソッド
        float[] samples = new float[audioClip.samples];
        audioClip.GetData(samples, 0);
        return samples;
    }

    public static bool IsSilent(byte[] audioData)
    {
        float[] samples = ConvertWavToSamples(audioData);
        
        // しきい値の設定（0.01以下の振幅を無音とみなす）
        float silenceThreshold = 0.01f;
        foreach (float sample in samples)
        {
            if (Mathf.Abs(sample) > silenceThreshold)
            {
                return false; // 音がある
            }
        }
        return true; // 無音
    }
}
