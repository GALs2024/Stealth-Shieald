using OscCore;
using UnityEngine;

public class oscsendertest : MonoBehaviour
{
    private OscClient client;
    public string address = "/shoulder";
    public string ipaddress = "192.168.3.252"; //localhost //192.168.80.246
    public int portNum = 10000;

    void Start()
    {
        client = new OscClient(ipaddress, portNum);
    }

    void Update()
    {
        // スペースキーでメッセージを送信
        if (Input.GetKeyDown(KeyCode.Space))
        {
            client.Send(address, 1);
        }
    }
}

