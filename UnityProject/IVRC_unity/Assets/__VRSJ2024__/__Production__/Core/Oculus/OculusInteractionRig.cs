using UnityEngine;

public class OculusInteractionRig : SingletonMonoBehaviour<OculusInteractionRig>
{
    public GameObject _head;
    public GameObject _leftHand;
    public GameObject _rightHand;
    
    protected override bool dontDestroyOnLoad => false;
}
