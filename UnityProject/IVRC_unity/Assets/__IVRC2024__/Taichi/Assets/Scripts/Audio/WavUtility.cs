using System.IO;
using UnityEngine;
using System;

public static class WavUtility
{
    public static void SaveAsWav(string filename, AudioClip clip)
    {
        try
        {
            // 保存パスを設定（"Assets/Audio/User/任意の名前.wav"）
            string directoryPath = Path.Combine(Application.dataPath, "__IVRC2024__/Taichi/Assets/Audio/User");
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
}
