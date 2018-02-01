using UnityEngine;
using UnityEngine.Networking;

public class TransformMotion : NetworkBehaviour
{

    [SyncVar]
    private Vector3 syncPos;

    [SyncVar]
    private float syncYRot;

    [SyncVar]
    private float syncXRot;

    [SyncVar]
    private Vector3 syncChildPos;

    [SyncVar]
    private float syncChildYRot;

    [SyncVar]
    private float syncChildXRot;

    private Vector3 lastPos, lastChildPos;
    private Quaternion lastRot, lastChildRot;
    public Transform myTransform, myChildTransform;

    [SerializeField]
    private float lerpRate = 10;
    [SerializeField]
    private float posThreshold = 0.5f;
    [SerializeField]
    private float rotThreshold = 5;

    // Use this for initialization
    void Start()
    {
        //myTransform = transform;
        //myChildTransform = myTransform.GetComponentInChildren<Transform>();

        //Debug.LogWarning("The name of the child is: " + myChildTransform.gameObject.name);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        TransmitMotion();
        LerpMotion();
    }

    [Command]
    void Cmd_ParentPositionToServer(Vector3 _pos, float _y, float _x)
    {
        syncPos = _pos;
        syncYRot = _y;
        syncXRot = _x;
    }

    [Command]
    void Cmd_ChildRotationToServer(float _childY, float _childX)
    {
        syncChildYRot = _childY;
        syncChildXRot = _childX;
    }

    [ClientCallback]
    void TransmitMotion()
    {
        if (hasAuthority)
        {
            if (Vector3.Distance(myTransform.position, lastPos) > posThreshold || Quaternion.Angle(myTransform.rotation, lastRot) > rotThreshold)
            {
                Cmd_ParentPositionToServer(myTransform.position, myTransform.localEulerAngles.y, myTransform.localEulerAngles.x);

                lastPos = myTransform.position;
                lastRot = myTransform.rotation;
            }

            if (Vector3.Distance(myChildTransform.position, lastChildPos) > posThreshold || Quaternion.Angle(myChildTransform.rotation, lastChildRot) > rotThreshold)
            {
                Cmd_ChildRotationToServer(myChildTransform.localEulerAngles.y, myChildTransform.localEulerAngles.x);

                lastChildRot = myChildTransform.rotation;
            }

        }
    }

    void LerpMotion()
    {
        if (!hasAuthority)
        {
            myTransform.position = Vector3.Lerp(myTransform.transform.position, syncPos, Time.deltaTime * lerpRate);
            Vector3 newRot = new Vector3(syncXRot, syncYRot, 0);
            myTransform.rotation = Quaternion.Lerp(myTransform.rotation, Quaternion.Euler(newRot), Time.deltaTime * lerpRate);


            Vector3 newChildRot = new Vector3(syncChildXRot, syncChildYRot, 0);
            myChildTransform.rotation = Quaternion.Lerp(myChildTransform.rotation, Quaternion.Euler(newChildRot), Time.deltaTime * lerpRate);

        }
    }
}
