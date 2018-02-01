using UnityEngine;
using UnityEngine.Networking;

public class NewTransformMotion : NetworkBehaviour
{

    [SyncVar]
    private Vector3 syncPos;


    private Vector3 lastPos;
    private Transform myTransform;
    [SerializeField]
    private float lerpRate = 10;
    [SerializeField]
    private float posThreshold = 0.5f;


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
    void Cmd_ProvidePositionToServer(Vector3 pos)
    {
        syncPos = pos;
    }

    [ClientCallback]
    void TransmitMotion()
    {
        if (hasAuthority)
        {
            if (Vector3.Distance(myTransform.position, lastPos) > posThreshold)
            {
                Cmd_ProvidePositionToServer(myTransform.position);

                lastPos = myTransform.position;
            }
        }
    }

    void LerpMotion()
    {
        if (!hasAuthority)
        {
            myTransform.position = Vector3.Lerp(myTransform.transform.position, syncPos, Time.deltaTime * lerpRate);
        }
    }
}
