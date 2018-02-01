using UnityEngine;
using UnityEngine.Networking;

public class RotationMotion : NetworkBehaviour
{

    [SyncVar]
    private float syncYRot;

    private Quaternion lastRot;
    private Transform myTransform;
    [SerializeField]
    private float lerpRate = 10;

    [SerializeField]
    private float rotThreshold = 5;

    // Use this for initialization
    void Start()
    {
        myTransform = transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        TransmitMotion();
        LerpMotion();
    }

    [Command]
    void Cmd_ProvidePositionToServer(float rot)
    {
        syncYRot = rot;
    }

    [ClientCallback]
    void TransmitMotion()
    {
        if (hasAuthority)
        {
            if (Quaternion.Angle(myTransform.rotation, lastRot) > rotThreshold)
            {
                Cmd_ProvidePositionToServer(myTransform.localEulerAngles.y);

                lastRot = myTransform.rotation;
            }
        }
    }

    void LerpMotion()
    {
        if (!hasAuthority)
        {
            Vector3 newRot = new Vector3(0, syncYRot, 0);
            myTransform.rotation = Quaternion.Lerp(myTransform.rotation, Quaternion.Euler(newRot), Time.deltaTime * lerpRate);
        }
    }
}
