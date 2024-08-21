using TMPro;
using UnityEngine;
using UniRx;
//using OscCore;
//using System.Net.Sockets;
//using System.Text;

public class CollisionObjectToText : MonoBehaviour
{
    public enum HandType
    {
        None = 99,
        LeftHand = 1,
        RightHand = 2
    }

    [SerializeField] HandType _handType = HandType.None;
    [SerializeField] TextMeshProUGUI _text;
    private string _handTypeString;
    private const string _avatarTag = "Avatar";

    [ReadOnly] public BoolReactiveProperty _isTouched;

    /*
    private UdpClient udpClient;
    private OscClient oscClient;
    private string ipAddress = "192.168.3.252"; // 送信先のIPアドレス
    private int port = 9000; // 送信先のポート番号
    */
    private void Awake()
    {
        _handTypeString = _handType.ToString();
        _text.text = _handTypeString + " : None";
    }
    /*
    private void Start() {
        udpClient = new UdpClient();
        oscClient = new OscClient(ipAddress, port);
    }
    */
    /// <summary>
    /// For OnCollision
    /// </summary>
    /// <param name="collision"></param>
    /*
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag(_avatarTag))
        {
            string objectName = collision.gameObject.name;
            string message = _handTypeString + " : " + objectName;
            _text.text = message;
            SendUdpMessage(message);
            SendOscMessage("/collision/stay", _handTypeString, objectName);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag(_avatarTag))
        {
            string message = _handTypeString + " : None";
            _text.text = message;
            SendUdpMessage(message);
            //SendOscMessage("/collision/exit", _handTypeString, "None");
        }
    }
    */
    /*
    ///For OnTriger
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Somethig OntriggerEnter");
        if (other.gameObject.CompareTag(_avatarTag))
        {
            string objectName = other.gameObject.name;
            string message = _handTypeString + " : " + objectName;
            _text.text = message;
            //SendUdpMessage(message);
            //SendOscMessage("/collision", _handTypeString, objectName);
        }
    }
    */
    private void OnTriggerStay(Collider other)
    {
        //Debug.Log("Somethig OntriggerEnter");
        if (other.gameObject.CompareTag(_avatarTag))
        {
            string objectName = other.gameObject.name;
            string message = _handTypeString + " : " + objectName;
            _text.text = message;
            _isTouched.Value = true;
            //SendUdpMessage(message);
            //SendOscMessage("/collision", _handTypeString, objectName);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(_avatarTag))
        {
            string message = _handTypeString + " : None";
            _text.text = message;
            _isTouched.Value = false;
            //SendUdpMessage(message);
            //SendOscMessage("/collision/exit", _handTypeString, "None");
        }
    }
    /*
    private void SendUdpMessage(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        udpClient.Send(data, data.Length, ipAddress, port);
    }

    private void SendOscMessage(string address, string handType, string objectName)

    {
        oscClient.Send(address, handType);
        oscClient.Send(address, objectName);
    }

    private void OnDestroy()
    {
        udpClient.Close();
    }
    */
}
