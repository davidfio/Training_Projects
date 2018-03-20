using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class PickUpFromGallery : MonoBehaviour
{
    public GameObject imageToSet;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
            // Don't attempt to pick media from Gallery/Photos if
            // another media pick operation is already in progress
            if (NativeGallery.IsMediaPickerBusy())
                return;

            if (Input.mousePosition.x < Screen.width * 2 / 3)
            {
                // Pick a PNG image from Gallery/Photos
                PickImage();
            }
            else
            {
                // Pick a video from Gallery/Photos
                PickVideo();
            }
          
        }
    }

    private void PickImage()
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                // Create Texture from selected image
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(File.ReadAllBytes(path));
                imageToSet.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));

               
                // Assign texture to a temporary cube and destroy it after 5 seconds
                //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //cube.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 10f;
                //cube.transform.forward = -Camera.main.transform.forward;
                //cube.GetComponent<Renderer>().material.mainTexture = texture;
                //Destroy(cube, 5f);

                // If a procedural texture is not destroyed manually, 
                // it will only be freed after a scene change
                //Destroy(texture, 5f);
            }
        }, "Select a PNG image", "image/png");

        Debug.Log("Permission result: " + permission);
    }

    private void PickVideo()
    {
        NativeGallery.Permission permission = NativeGallery.GetVideoFromGallery((path) =>
        {
            Debug.Log("Video path: " + path);
            if (path != null)
            {
                // Play the selected video
                Handheld.PlayFullScreenMovie("file://" + path);
            }
        }, "Select a video");

        Debug.Log("Permission result: " + permission);
    }
}
