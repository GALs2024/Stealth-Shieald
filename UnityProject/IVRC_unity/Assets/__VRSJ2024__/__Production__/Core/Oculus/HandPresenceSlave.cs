using Oculus.Interaction.HandGrab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HandVisualMode
{
    Always,
    OnlyPenetrate,
    None
}

public class HandPresenceSlave : MonoBehaviour
{
    [SerializeField]private HandVisualMode handVisualMode;
    public Transform target;
    private Rigidbody rb;
    public Renderer physicsHandRenderer; 
    public Renderer realHandRenderer;
    public Material transSkin;
    public Material handsSkin;
    [SerializeField] private float showPhysicsHandRendererDistance = 0.05f;
    [SerializeField] private HandGrabInteractor handGrab;
    public Vector3 adjustRotate;
    public Transform rootSourceBone;
    [SerializeField] private Transform rootBone;
    public List<Transform> _sourceBones;
    public List<Transform> _bones;
    public bool isSetBoans = true;
    private Collider[] handColliders;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        _sourceBones = new List<Transform>();
        _bones = new List<Transform>();
        GetBones(rootSourceBone, _sourceBones);
        GetBones(rootBone, _bones);
        handColliders = GetComponentsInChildren<Collider>();
        if(handVisualMode == HandVisualMode.Always)
        {
            physicsHandRenderer.enabled = true;
        }
        else
        {
            physicsHandRenderer.enabled = false;
        }
    }

    void Update()
    {
        transform.localScale = target.localScale;
        if (isSetBoans)
        {
            SetBones();
        }

        if(handVisualMode == HandVisualMode.OnlyPenetrate) 
        {
            float distance = Vector3.Distance(this.transform.position, target.position);
            ///HandGrabInteractor‚ÉGrabó‘Ô‚©‚Ç‚¤‚©ŠO•”‚©‚çŽQÆ‚Å‚«‚é‚æ‚¤‚É‚·‚é
            if (distance > showPhysicsHandRendererDistance && !handGrab.IsGrabbing)
            {
                physicsHandRenderer.enabled = true;
                realHandRenderer.material = transSkin;
            }
            else
            {
                physicsHandRenderer.enabled = false;
                realHandRenderer.material = handsSkin;
            }
        }
    }

    void FixedUpdate()
    {
        rb.velocity = (target.position - transform.position) / Time.fixedDeltaTime;

        Quaternion rotationDifference = target.rotation * Quaternion.Inverse(transform.rotation);
        rotationDifference.ToAngleAxis(out float angleInDegree, out Vector3 rotationAxis);

        Vector3 rotationDifferenceInDegree = angleInDegree * rotationAxis;

        rb.angularVelocity = (rotationDifferenceInDegree * Mathf.Deg2Rad / Time.fixedDeltaTime);
        rb.MoveRotation(target.rotation * Quaternion.Euler(adjustRotate));
    }

    private void SetBones()
    {
        if (_sourceBones.Count == _bones.Count)
        {
            for (int i = 0; i < _sourceBones.Count; i++)
            {
                _bones[i].localRotation = _sourceBones[i].localRotation;
            }
        }
    }

    private void GetBones(Transform rootBone, List<Transform> bone)
    {
        int length = rootBone.childCount;

        for (int i = 0; i < length; i++)
        {
            Transform b = rootBone.GetChild(i);
            if (b.name.IndexOf("b_") == 0)
            {
                bone.Add(b);

                if (b.childCount > 0)
                {
                    GetBones(b, bone);
                }
            }
        }
    }

    public void EnableHandCollider()
    {
        foreach(var item in handColliders)
        {
            item.enabled = true;
        }
    }

    public void EnableHandColliderDelay(float delay)
    {
        Invoke("EnableHandCollider", delay);
    }

    public void DisableHandCollider()
    {
        foreach (var item in handColliders)
        {
            item.enabled = false;
        }
    }
}
