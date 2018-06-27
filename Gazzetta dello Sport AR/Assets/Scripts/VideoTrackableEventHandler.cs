/*===============================================================================
Copyright (c) 2017 PTC Inc. All Rights Reserved.
 
Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
===============================================================================*/
using UnityEngine;
using System.Collections;


public class VideoTrackableEventHandler : DefaultTrackableEventHandler
{
    #region PROTECTED_METHODS

    public GameObject fullscreenButton;
    private StreamVideo refStreamVideo;

    private void Awake()
    {
        refStreamVideo = FindObjectOfType<StreamVideo>();
    }

    // Here I do autoplay videos Bulgari and Lampoon OnTrackingFound
    protected override void OnTrackingFound()
    {
        fullscreenButton.SetActive(true);
        mTrackableBehaviour.GetComponentInChildren<VideoController>().Play();
        
        base.OnTrackingFound();
    }


    protected override void OnTrackingLost()
    {
        if (!refStreamVideo.isPlaying)
        {
            fullscreenButton.SetActive(false);
            mTrackableBehaviour.GetComponentInChildren<VideoController>().Pause();
        }
        else
        {
            Debug.Log("Sta riproducendo un video quindi non lo disattivo");
        }
        
        base.OnTrackingLost();
    }

    #endregion // PROTECTED_METHODS
}
