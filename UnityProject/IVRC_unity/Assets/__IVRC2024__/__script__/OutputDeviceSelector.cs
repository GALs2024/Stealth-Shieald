using AudioStream;
using AudioStreamSupport;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class OutputDeviceSelector : MonoBehaviour
{
    enum AudioPosition
    {
        Chair = 0, HMD = 1, None = 99
    }

    [SerializeField] AudioMixer audioMixer;
    [SerializeField] string HapticsDeviceID = "Chair_Output_ID";
    [SerializeField] string OculusDeviceID = "Oculus_Output_ID";

    [SerializeField] string HapticsDeviceName = "(HPAUSB0202)";
    [SerializeField] string OculusDeviceName = "(Oculus Virtual Audio Device)";
    [SerializeField] int defaultNumber = 0;
    private List<string> OutputDeviceNames = new List<string> { };
    void Start()
    {
        var availableOutputs = FMOD_SystemW.AvailableOutputs(LogLevel.DEBUG, gameObject.name, null);
        foreach (var availableOutput in availableOutputs)
        {
            //Debug.Log(availableOutput.name);
            OutputDeviceNames.Add(availableOutput.name);
        }

        SetDeviceID(AudioPosition.Chair);
        SetDeviceID(AudioPosition.HMD);

    }
    void SetDeviceID(AudioPosition audioPosition)
    {
        int value = -1;
        switch (audioPosition)
        {
            case AudioPosition.Chair:
                value = OutputDeviceNames.FindIndex(s => s.Contains(HapticsDeviceName));
                audioMixer.SetFloat(HapticsDeviceID, (float)value);
                break;
            case AudioPosition.HMD:
                value = OutputDeviceNames.FindIndex(s => s.Contains(OculusDeviceName));
                audioMixer.SetFloat(OculusDeviceID, (float)value);
                break;
            default:
                break;
        }
    }
}