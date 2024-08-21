using OscCore;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class OSCSendManager : MonoBehaviour //SingletonMonoBehaviour<OSCSendManager>
{
    private UdpClient udpClient;
    private OscClient oscClient;
    private string ipAddress = "192.168.3.252"; // 送信先のIPアドレス
    private int port = 9000; // 送信先のポート番号
    //protected override bool dontDestroyOnLoad => false;

    private void Start()
    {
        udpClient = new UdpClient();
        oscClient = new OscClient(ipAddress, port);
    }

    public void SendUdpMessage(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        udpClient.Send(data, data.Length, ipAddress, port);
    }

    public void SendOscMessage(string address, string handType, string objectName)

    {
        oscClient.Send(address, handType);
        oscClient.Send(address, objectName);
    }

    private void OnDestroy()
    {
        udpClient.Close();
    }
}
