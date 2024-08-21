using OscCore;
using UnityEngine;

public class oscreceiver : MonoBehaviour
{
    [SerializeField] private int port = 9000;
    private const string OscAddress = "/example";
    private OscServer server;

    private void Awake()
    {
        server = new OscServer(port);
        server.TryAddMethod(OscAddress, ReadValues);
    }

    private void OnDestroy()
    {
        server.Dispose();
    }

    private void ReadValues(OscMessageValues values)
    {
        Debug.Log(values.ReadIntElement(0));
    }
}

