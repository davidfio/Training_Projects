using UnityEngine;
using UnityEngine.UI;

namespace FFmpeg.Demo
{
	public class TrimView : MonoBehaviour
	{
        TrimData config = new TrimData();
        public Text pathText;

		//------------------------------

		public void Open()
		{
			gameObject.SetActive(true);
		}

		//------------------------------

		public void OnInputPath(string fullPath)
		{
            //config.inputPath = fullPath;
            config.inputPath = pathText.text;
		}

		public void OnStartTime(string time)
		{
            config.fromTime = time;
		}

		public void OnOutputPath(string fullPath)
		{
            config.outputPath = pathText.text + "new video.mp4";
		}

		public void OnDuration(string duration)
		{
            config.durationSec = int.Parse(duration);
		}

		//------------------------------

		public void OnTrim()
		{
            FFmpegCommands.Trim(config);
            
            gameObject.SetActive(false);
        }

        public void PlayNewVideo()
        {
            Handheld.PlayFullScreenMovie("file://" + pathText.text + "new video.mp4");
        }

        //------------------------------
    }
}