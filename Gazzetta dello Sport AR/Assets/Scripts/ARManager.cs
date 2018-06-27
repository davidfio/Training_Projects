using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;

public class ARManager : MonoBehaviour
{
    private GameObject[] targetArray;
    public Vuforia.VuforiaBehaviour cameraVuforiaBehaviour;
    private GameObject lastClickedObject;

    public bool isFlashOn;

    // Deactive the targets to not start recognition under UI
    private void Start()
    {
        Screen.fullScreen = false;
        targetArray = GameObject.FindGameObjectsWithTag("Target");
        Invoke("DeactiveGO", .5f);
    }

    // Disattiva i target allo Start della scena
    private void DeactiveGO()
    {
        for (int i = 0; i < targetArray.Length; i++)
        {
            targetArray[i].SetActive(false);
        }

        cameraVuforiaBehaviour.enabled = false;
    }

    public void ReloadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void OnOffFlashlight()
    {
        if (!isFlashOn)
        {
            Vuforia.CameraDevice.Instance.SetFlashTorchMode(true);
            isFlashOn = true;
        }
        else
        {
            Vuforia.CameraDevice.Instance.SetFlashTorchMode(false);
            isFlashOn = false;
        }
    }

    public void DisableFlashLight()
    {
        Vuforia.CameraDevice.Instance.SetFlashTorchMode(false);
        isFlashOn = false;
    }

    public void DownloadPDF()
    {
        Application.OpenURL("https://drive.google.com/drive/folders/1FmH4AH39unetxC3EiCaepIlKp2DNaiEp");
    }
}