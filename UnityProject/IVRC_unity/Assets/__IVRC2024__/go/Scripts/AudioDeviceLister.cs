using UnityEngine;

public class AudioDeviceLister : MonoBehaviour
{
    void Start()
    {
        ListOutputDevices();
    }

    void ListOutputDevices()
    {
        // デバイスの数を取得してリストに表示
        for (int i = 0; i < Microphone.devices.Length; i++)
        {
            Debug.Log("Device ID: " + i + " - Device Name: " + Microphone.devices[i]);
        }
    }
}
