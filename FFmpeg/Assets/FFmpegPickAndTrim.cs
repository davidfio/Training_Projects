using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace FFmpeg.Demo
{
    public class FFmpegPickAndTrim : MonoBehaviour, IFFmpegHandler
    {
        private string pathNewVideoToPlay;
        public Text pathText;
        public Image imageWatermark;


        FFmpegHandler defaultHandler = new FFmpegHandler();
        TrimData config = new TrimData();
        WatermarkData watermarkData = new WatermarkData();

        //------------------------------

        private void Awake()
        {
            FFmpegParser.Handler = this;
            Screen.fullScreen = false;

            config.fromTime = "01";
            config.durationSec = 10;

            watermarkData.imagePath = "jar: file://" + Application.dataPath + "!/assets/UI/Certificati.png";
            watermarkData.imageScale = 1f;
            watermarkData.xPosNormal = 0f;
            watermarkData.yPosNormal = 0f;
        }

        public void PickTrimPlay()
        {
            NativeGallery.Permission permission = NativeGallery.GetVideoFromGallery((path) =>
            {
                if (path != null)
                {
                    // Play the selected video                    
                    config.inputPath = path;
                    //watermarkData.inputPath = path;
                    config.outputPath = path.Substring(0, path.Length - 4) + "_Trimmed.mp4";
                    //watermarkData.outputPath = path.Substring(0, path.Length - 4) + "_Trimmed.mp4";
                    pathNewVideoToPlay = config.outputPath;

                    FFmpegCommands.Trim(config);
                    //FFmpegCommands.Watermark(watermarkData);
                }
            }, "Select a video");
        }
        //FFmpeg processing callbacks
        //------------------------------

        //Begining of video processing
        public void OnStart()
        {
			defaultHandler.OnStart();
        }

		//You can make custom progress bar here (parse msg)
		public void OnProgress(string msg)
        {
            defaultHandler.OnProgress(msg);
            Console.Print(msg);
        }

		//Notify user about failure here
		public void OnFailure(string msg)
        {
            defaultHandler.OnFailure(msg);
            Console.Print(msg);
        }

		//Notify user about success here
		public void OnSuccess(string msg)
        {
			defaultHandler.OnSuccess(msg);
            Console.Print(msg);
        }

		//Last callback - do whatever you need next
		public void OnFinish()
        {
            defaultHandler.OnFinish();

            //NativeGallery.Permission permission = NativeGallery.SaveVideoToGallery(pathNewVideoToPlay, "TrimmedVideo", "newVideo_Trimmed.mp4");
            NativeGallery.SaveVideoToGallery(pathNewVideoToPlay, "TrimmedVideo", "newTrimmedVideo.mp4");
            Handheld.Vibrate();
            //Handheld.PlayFullScreenMovie("file://" + pathNewVideoToPlay, Color.black, FullScreenMovieControlMode.Full, FullScreenMovieScalingMode.AspectFit);
            Application.OpenURL("https://www.instagram.com/?hl=it");
        }
    }
}