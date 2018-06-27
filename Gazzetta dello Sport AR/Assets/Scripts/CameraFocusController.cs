using UnityEngine;
using Vuforia;

public class CameraFocusController : MonoBehaviour
{
    VuforiaBehaviour refVB;
    private void Awake()
    {
        refVB = FindObjectOfType<VuforiaBehaviour>();
        //Debug.LogWarning("Dentro On Enable di CameraFocusController");
        //var vuforia = VuforiaARController.Instance;
        //vuforia.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        //vuforia.RegisterOnPauseCallback(OnPaused);
    }

    private void Update()
    {
        if (refVB.isActiveAndEnabled)
        {
            Debug.LogWarning("Dentro Update di CameraFocusController perché Vuforia Behavior abilitato");
            CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
        }
        else
        {
            Debug.LogWarning("Dentro Else di CameraFocusController perché Vuforia Behavior disabilitato");
        }
        
    }

    private void OnVuforiaStarted()
    {
        CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
    }

    private void OnPaused(bool paused)
    {
        if (!paused) 
        {
            // Set again autofocus mode when app is resumed
            CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
        }
    }
}