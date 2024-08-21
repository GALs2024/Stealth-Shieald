using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscCore;

public class VibratingObject : MonoBehaviour
{
    public float vibrationSpeed = 1.0f;
    public float vibrationAmplitude = 0.1f;
    private bool isVibrating = true;
    private Vector3 originalPosition;
    private OscClient oscClient;
    public string oscAddress = "/vibration";
    public string ipaddress = "192.168.3.252";
    public int portNum = 10000;

    void Start()
    {
        originalPosition = transform.position;
        oscClient = new OscClient(ipaddress, portNum);
    }

    void Update()
    {
        if (isVibrating)
        {
            float vibration = Mathf.Sin(Time.time * vibrationSpeed) * vibrationAmplitude;
            transform.position = originalPosition + new Vector3(vibration, 0, 0);
            SendOscSignal(1);
        }
        else
        {
            // 振動が停止している場合、OSC信号の値を0に設定
            SendOscSignal(0);
        }
    }

    public void StopVibration()
    {
        if (isVibrating) // 一度だけ停止処理を行う
        {
            isVibrating = false;
            transform.position = originalPosition;
            SendOscSignal(0); // 振動が停止したことをOSCで送信
        }
    }

    private void SendOscSignal(int value)
    {
        oscClient.Send(oscAddress, value);
    }
}
