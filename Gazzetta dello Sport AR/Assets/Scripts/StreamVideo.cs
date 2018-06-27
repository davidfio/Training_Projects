using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

public class StreamVideo : MonoBehaviour
{
    public RawImage image;
    public VideoPlayer[] videoTargetArray;
    public bool isPlaying;

    public GameObject playPanel;
    public GameObject blackPanel;
    public Vuforia.VuforiaBehaviour refVuforia;

    public void PlayFullScreenVideo()
    {
        image.gameObject.SetActive(true);

        playPanel.SetActive(false);
        refVuforia.enabled = false;
        blackPanel.SetActive(true);

        Application.runInBackground = true;

        for (int i = 0; i < videoTargetArray.Length; i++)
        {
            if (videoTargetArray[i].isActiveAndEnabled)
            {
                StartCoroutine(PlayVideoCO(videoTargetArray[i]));
                Debug.LogWarning("Lancio PlayVideoCO del target: " + videoTargetArray[i].gameObject.name);
            }
        }

        //Application.runInBackground = true;

        //for (int i = 0; i < videoTargetArray.Length; i++)
        //{
        //    if (videoTargetArray[i].isActiveAndEnabled)
        //    {
        //        StartCoroutine(PlayVideoCO(videoTargetArray[i]));
        //        Debug.LogWarning("Lancio PlayVideoCO del target: " + videoTargetArray[i].gameObject.name);
        //    }
        //}      
    }

    private IEnumerator PlayVideoCO(VideoPlayer _video)
    {
        _video.Prepare();

        //Wait until Movie is prepared
        WaitForSeconds waitTime = new WaitForSeconds(1);
        while (!_video.isPrepared)
        {
            yield return waitTime;
            break;
        }
        
        //Assign the Texture from Movie to RawImage to be displayed
        image.texture = _video.texture;

        image.rectTransform.Rotate(new Vector3(0, 0, -90));
        
        //Play Movie 
        _video.Play();

        isPlaying = true;

        while (_video.isPlaying)
        {
            yield return null;
        }

        isPlaying = false;
    }
}