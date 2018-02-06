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

    private Vector3 lastPos;
    private Quaternion lastRot, lastChildRot;
    [Tooltip("Sono le transform del Player padre e della MainCamera ")]
    public Transform myTransform, myChildTransform;

    [SerializeField]
    [Tooltip("Piú é alto il valore maggiore sará la velocitá di sync")]
    private float lerpRate = 20;
    [SerializeField]
    [Tooltip("Valore di soglia minimo in cui far scattare la sync della posizione")]
    private float posThreshold = 0.1f;
    [SerializeField]
    [Tooltip("Valore di soglia minima per far scattare la sync della rotazione ")]
    private float rotThreshold = 1;

    void Start()
    {
        //myTransform = transform;
        //myChildTransform = myTransform.GetComponentInChildren<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        TransmitMotion();
        LerpMotion();
    }

    // Il command del server per far sincronizzare la transform del parent
    [Command]
    void Cmd_ParentPositionToServer(Vector3 _pos, float _y, float _x)
    {
        syncPos = _pos;
        syncYRot = _y;
        syncXRot = _x;
    }

    // Il commande del server per far sincronizzare la rotazione del figlio (in questo caso la Main Camera)
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

            if (Quaternion.Angle(myChildTransform.rotation, lastChildRot) > rotThreshold)
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

            // Lerpa il parent nella posizione e rotazione in base all'ultima transform ricevuta
            myTransform.position = Vector3.Lerp(myTransform.transform.position, syncPos, Time.deltaTime * lerpRate);
            Vector3 newRot = new Vector3(syncXRot, syncYRot, 0);
            myTransform.rotation = Quaternion.Lerp(myTransform.rotation, Quaternion.Euler(newRot), Time.deltaTime * lerpRate);

            // Lerpra il figlio con l'ultimo valore di rotazione disponibile
            Vector3 newChildRot = new Vector3(syncChildXRot, syncChildYRot, 0);
            myChildTransform.rotation = Quaternion.Lerp(myChildTransform.rotation, Quaternion.Euler(newChildRot), Time.deltaTime * lerpRate);
        }
    }
}
