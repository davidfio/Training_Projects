using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMainScene : MonoBehaviour
{

    private void Awake()
    {
        Invoke("ChangeScene", 1f);
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene(1);
    }
}