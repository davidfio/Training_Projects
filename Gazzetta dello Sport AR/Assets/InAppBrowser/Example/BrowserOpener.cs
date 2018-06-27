using UnityEngine;
using System.Collections;

public class BrowserOpener : MonoBehaviour
{
	public void OpenLink_OnTrackingFound(string URL)
    {
		InAppBrowser.DisplayOptions displayOptions = new InAppBrowser.DisplayOptions();
        displayOptions.backButtonText = "Back";
        displayOptions.hidesTopBar = false;
        displayOptions.displayURLAsPageTitle = false;

        InAppBrowser.OpenURL(URL, displayOptions);
        Debug.Log("Aperto il sito: " + URL);
    }

	public void OnClearCacheClicked()
    {
		InAppBrowser.ClearCache();
	}

    public void CloseBrowser()
    {
        InAppBrowser.CloseBrowser();
    }
}
