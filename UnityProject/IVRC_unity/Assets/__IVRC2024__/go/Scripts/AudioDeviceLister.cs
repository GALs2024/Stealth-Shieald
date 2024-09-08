using UnityEngine;

public class AudioDeviceLister : MonoBehaviour
{
    void Start()
    {
        ListOutputDevices();
    }

    void ListOutputDevices()
    {
        // �f�o�C�X�̐����擾���ă��X�g�ɕ\��
        for (int i = 0; i < Microphone.devices.Length; i++)
        {
            Debug.Log("Device ID: " + i + " - Device Name: " + Microphone.devices[i]);
        }
    }
}
