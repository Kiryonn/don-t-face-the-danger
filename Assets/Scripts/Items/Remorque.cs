using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

public class Remorque : Task
{
    [SerializeField] float cameraOffsetDistance;
    CinemachineVirtualCamera virtualCamera;
    [SerializeField] Transform remorque;
    [SerializeField] Vector3 positionOffset;
    [SerializeField] float scaleOffset;
    HingeJoint trailerJoint;
    CustomJoint trailer;
    [System.NonSerialized] UnityEvent<bool> OnAttach = new UnityEvent<bool>();

    [Header("Hinge joints parameters")]
    float minLimit;
    float maxLimit;
    Vector3 anchorValues;
    Rigidbody remorqueBody;

    float baseCameraDistance;
    bool check = false;

    [SerializeField] AudioClip equipSFX;
    protected override void OnStart()
    {
        base.OnStart();
        virtualCamera = (CinemachineVirtualCamera) GameManager.Instance.cam
            .GetComponent<CinemachineBrain>().ActiveVirtualCamera;
        baseCameraDistance = GetCameraDistance();

        remorqueBody = remorque.gameObject.GetComponent<Rigidbody>();
        remorqueBody.isKinematic = true;
        //trailerJoint = remorque.gameObject.GetComponent<HingeJoint>();
        trailer = remorque.gameObject.GetComponent<CustomJoint>();
        OnAttach.AddListener(trailer.UpdateJoint);
        /*minLimit = trailerJoint.limits.min;
        maxLimit = trailerJoint.limits.max;
        anchorValues = trailerJoint.anchor;
        Destroy(trailerJoint);*/
    }

    public override void Interact()
    {
        base.Interact();
        GameManager.Instance.player.canMove = false;
        GameManager.Instance.GetComponent<TransitionManager>().FadeTransition(1f,3f, 1f);
        Invoke("AttachRemorque",3f);

    }

    void AttachRemorque()
    {
        AudioManager.instance.PlaySFX(equipSFX);
        Transform parent;
        parent = GameManager.Instance.player.transform;

        remorque.parent = parent;
        remorque.localPosition = positionOffset;
        remorque.localScale = Vector3.one * scaleOffset;
        remorque.localRotation = Quaternion.identity;

        remorque.parent = parent.parent;
        trailer.SetTracteur(parent);
        OnAttach.Invoke(true);
        remorqueBody.isKinematic = false;


        //ConnectHingeJoint();
        //trailerJoint.connectedBody = parent.GetComponent<Rigidbody>();
        GameManager.Instance.SwitchCam(CamTypes.Equipments);
        //UpdateCameraDistance(cameraOffsetDistance);
        GameManager.Instance.player.canMove = true;
    }

    public void DetachRemorque()
    {
        OnAttach.Invoke(false);
        //RemoveHingeJoint();
        remorque.parent = transform;
        remorqueBody.isKinematic = true;
        //UpdateCameraDistance(baseCameraDistance);
        GameManager.Instance.SwitchCam(CamTypes.Tractor);
    }

    float GetCameraDistance()
    {
        CinemachineComponentBase componentBase = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
        if (componentBase is Cinemachine3rdPersonFollow)
        {
            return (componentBase as Cinemachine3rdPersonFollow).CameraDistance;
        }
        return 0f;
    }

    void UpdateCameraDistance(float dist)
    {
        CinemachineComponentBase componentBase = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
        if (componentBase is Cinemachine3rdPersonFollow)
        {
            (componentBase as Cinemachine3rdPersonFollow).CameraDistance = dist;
        }
    }

    void ConnectHingeJoint()
    {
        trailerJoint = remorque.gameObject.AddComponent<HingeJoint>();
        trailerJoint.autoConfigureConnectedAnchor = false;
        trailerJoint.anchor = anchorValues;
        trailerJoint.axis = Vector3.up;

        trailerJoint.useLimits = true;
        JointLimits jointLimits = trailerJoint.limits;
        jointLimits.max = maxLimit;
        jointLimits.min = minLimit;

        trailerJoint.limits = jointLimits;
        remorqueBody.isKinematic = false;
    }

    void RemoveHingeJoint()
    {
        remorqueBody.isKinematic = true;
        Destroy(trailerJoint);
    }
}
