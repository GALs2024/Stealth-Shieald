using System.ComponentModel;
using UniRx;
using UnityEngine;

public class GetNearPosition : SingletonMonoBehaviour<GetNearPosition>
{
    [SerializeField] private OSCSendManager oscSendManager;
    [SerializeField] private Transform[] _target;
    [SerializeField] private Transform _leftHandTransform;
    [SerializeField] private Transform _rightHandTransform;
    [SerializeField] private CollisionObjectToText _leftHandCollisionObjectToText;
    [SerializeField] private CollisionObjectToText _rightHandCollisionObjectToText;

    protected override bool dontDestroyOnLoad => false;


    void Start()
    {
        _leftHandCollisionObjectToText._isTouched.SkipLatestValueOnSubscribe().Subscribe(value =>
        {
            if (value)
            {
                GameObject leftTarget = GetNearTarget(CollisionObjectToText.HandType.LeftHand).gameObject;
                Debug.Log($"<color=cyan>Vibrate : {CollisionObjectToText.HandType.LeftHand.ToString()} {leftTarget.name}");
                oscSendManager.SendOscMessage("/collision", CollisionObjectToText.HandType.LeftHand.ToString(), leftTarget.name);
            }
        }).AddTo(gameObject);

        _rightHandCollisionObjectToText._isTouched.SkipLatestValueOnSubscribe().Subscribe(value =>
        {
            if(value)
            {
                GameObject rightTarget = GetNearTarget(CollisionObjectToText.HandType.RightHand).gameObject;
                Debug.Log($"<color=cyan>Vibrate : {CollisionObjectToText.HandType.RightHand.ToString()} {rightTarget.name}");

                oscSendManager.SendOscMessage("/collision", CollisionObjectToText.HandType.RightHand.ToString(), rightTarget.name);
            }
        }).AddTo(gameObject);
    }

    private Transform GetNearTarget(CollisionObjectToText.HandType handType)
    {
        Transform handTransfrom = handType == CollisionObjectToText.HandType.LeftHand ? _leftHandTransform : _rightHandTransform;
        float minDistance = float.MaxValue;
        Transform nearTarget = null;
        foreach (var target in _target)
        {
            float distance = Vector3.Distance(handTransfrom.position, target.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearTarget = target;
            }
        }
        return nearTarget;
    }
}
