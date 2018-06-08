using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using FantomLib;

//Demonstration of get image information from the gallery.
//(*) Please download whole sphere mesh 'Sphere100.fbx' from the URL.
//http://warapuri.com/post/131599525953/
//･When saving a screenshot to External Storage, the following permission is required for 'AndroidManifest.xml'.
//(*)'<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />' in 'AndroidManifest.xml'
//
//ギャラリーからの画像情報取得のデモ
//※全天球のメッシュ「Sphere100.fbx」は以下からダウンロードして下さい。
//http://warapuri.com/post/131599525953/
//・スクリーンショットをストレージに保存する場合は、以下のパーミッションが「AndroidManifest.xml」に必要になります。
//※'<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />' in 'AndroidManifest.xml'
public class GalleryPickTest2 : MonoBehaviour {

    public Material textureMat;         //Material applying texture.                                //テクスチャを適用するマテリアル
    public Image image;                 //Image to apply texture.                                   //テクスチャを適用する画像
    public GameObject cube;             //Cube object to apply texture.                             //テクスチャを適用するキューブオブジェクト
    public GameObject sphere;           //Whole sphere (for 360 degrees image) to apply texture.    //テクスチャを適用する全天球オブジェクト
    public GameObject chara;            //Character Model or other (Texture does not apply)         //表示するキャラクターなど（テクスチャは適用しない）

    public int defaultWidth = 512;      //Alternate value when width get failed.                    //幅の取得に失敗したときの代替値
    public int defaultHeight = 512;     //Alternate value when height get failed.                   //高さの取得に失敗したときの代替値

    public GameObject[] hideUIOnScreenshot;    //UI to hide in screenshot.

    public Screenshot screenshot;               //Screenshot function

    //Mainly 'ToastController.Show' is called.
    public ToastController toastControl;

    //Mainly 'MediaScannerController.StartScan' is called.
    public MediaScannerController mediaScannerControl;

    public SendTextController sendTextControl;  //For share contents
    public Button shareButton;                  //Share (Send text) button

    public MailerController mailerControl;      //For mail attachment (Email address is input to application)
    public Button mailButton;                   //Mail button


    public SystemLanguage localizeLanguage = SystemLanguage.Unknown;    //current localize language

    //Saved message
    public LocalizeString savedMessage = new LocalizeString(SystemLanguage.English,
        new List<LocalizeString.Data>()
        {
            new LocalizeString.Data(SystemLanguage.English, "Save ScreenShot completed."),    //default language
            new LocalizeString.Data(SystemLanguage.Japanese, "スクリーンショットが保存されました。"),
            new LocalizeString.Data(SystemLanguage.ChineseSimplified, "屏幕截图已保存。"),
            new LocalizeString.Data(SystemLanguage.Korean, "스크린 샷이 저장되었습니다."),
        });

    //Share message
    public LocalizeString shareText = new LocalizeString(SystemLanguage.English,
        new List<LocalizeString.Data>()
        {
            new LocalizeString.Data(SystemLanguage.English, "Share the screenshots!"),    //default language
            new LocalizeString.Data(SystemLanguage.Japanese, "スクリーンショットをシェアするよ！"),
            new LocalizeString.Data(SystemLanguage.ChineseSimplified, "我将分享截图！"),
            new LocalizeString.Data(SystemLanguage.Korean, "스크린 샷을 공유하는거야!"),
        });



    //==========================================================

    // Use this for initialization
    private void Start () {
        if (cube != null)
            cube.SetActive(false);
        if (sphere != null)
            sphere.SetActive(false);
        if (chara != null)
            chara.SetActive(false);

        SetEnableShareButtons(false);

#if !UNITY_EDITOR && UNITY_ANDROID
        XDebug.Log("'WRITE_EXTERNAL_STORAGE' permission = " + AndroidPlugin.CheckPermission("android.permission.WRITE_EXTERNAL_STORAGE"));
#endif
    }

    // Update is called once per frame
    //private void Update () {

    //}


    //==========================================================
    //UI

    //Callback handeler when switch UI image.      //UI の Image
    public void OnImageModeClick(bool isOn)
    {
        if (image != null)
            image.gameObject.SetActive(isOn);
    }

    //Callback handeler when switch cube object.   //Cube
    public void OnCubeModeClick(bool isOn)
    {
        if (cube != null)
            cube.SetActive(isOn);
    }

    //Callback handeler when switch whole sphere (360 degrees).    //全天球（360度）
    public void OnSphereModeClick(bool isOn)
    {
        if (sphere != null)
            sphere.SetActive(isOn);
    }

    //Callback handeler when display character etc.    //キャラクターなどの表示
    public void OnCharaClick(bool isOn)
    {
        if (chara != null)
            chara.SetActive(isOn);
    }


    //==========================================================
    //Gallery pick and load image

    //Callback handler when image information can be get from the gallery.  //ギャラリーから画像情報を取得できたときのコールバックハンドラ
    public void OnGalleryPick(ImageInfo info)
    {
        XDebug.Log("OnGalleryPick: " + info);

        int width = info.width > 0 ? info.width : defaultWidth;       //Alternate value when width get failed.    //幅の取得に失敗したときの代替値
        int height = info.height > 0 ? info.height : defaultHeight;   //Alternate value when height get failed.   //高さの取得に失敗したときの代替値
        LoadAndSetImage(info.path, width, height);

        lastScreenshot = info;
        SetEnableShareButtons(!string.IsNullOrEmpty(lastScreenshot.uri));
    }

    //Image loading and setting.    //画像の読み込みとセット
    private void LoadAndSetImage(string path, int width, int height)
    {
        Texture2D texture = LoadToTexture2D(path, width, height, TextureFormat.ARGB32, false, FilterMode.Bilinear);
        if (texture != null)
        {
            RectTransform rt = image.rectTransform;
            int h = (int)rt.sizeDelta.y;
            int w = width * h / height;
            rt.sizeDelta = new Vector2(w, h);   //Make the same ratio as the image with the height as the reference.  //縦を基準として画像と同じ比率にする

            try
            {
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
                image.sprite = sprite;
                textureMat.mainTexture = texture;
            }
            catch (Exception)
            {
                XDebug.Log("Sprite.Create failed.");
            }
        }
        else
        {
            XDebug.Log("CreateTexture2D failed.");
#if !UNITY_EDITOR && UNITY_ANDROID
            XDebug.Log("'READ_EXTERNAL_STORAGE' permission = " + AndroidPlugin.CheckPermission("android.permission.READ_EXTERNAL_STORAGE"));
#endif
        }
    }

    //Load the image from the specified path and generates a Texture2D.     //指定パスから画像を読み込み、テクスチャを生成する。
    private static Texture2D LoadToTexture2D(string path, int width, int height, TextureFormat format, bool mipmap, FilterMode filter)
    {
        if (string.IsNullOrEmpty(path))
            return null;

        try
        {
            byte[] bytes = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(width, height, format, mipmap);
            texture.LoadImage(bytes);
            texture.filterMode = filter;
            texture.Compress(false);
            return texture;
        }
        catch (Exception e)
        {
            XDebug.Log(e.ToString());
            return null;
        }
    }

    //Callback handler when error or cancel.    //エラーやキャンセルのときのコールバックハンドラ
    public void OnError(string message)
    {
        XDebug.Log("GalleryPickTest2.OnError : " + message);
    }


    //==========================================================
    //Screenshot

    //Run screenshot
    public void ScreenShot()
    {
        if (screenshot == null || screenshot.IsSaving)
            return;     //Ignore while saving.

        StartCoroutine(StartScreenshot());
    }

    //Hide the UI and execute the screenshot. If save the screenshot successfully, run MeidaScanner.
    private IEnumerator StartScreenshot()
    {
        SetVisibleUI(false);
        yield return null;
        screenshot.StartScreenshot();
    }

    //Toggle display of UI when screenshot
    public void SetVisibleUI(bool visible)
    {
        foreach (var item in hideUIOnScreenshot)
            item.SetActive(visible);
    }

    //Callback handler when screen shot fails
    public void ReceiveScreenshotError(string message)
    {
        SetVisibleUI(true);
        XDebug.Log("Error Screenshot : " + message);
    }

    //Callback handler when screenshot succeeds
    public void ReceiveScreenshotComplete(string path)
    {
        SetVisibleUI(true);

        if (toastControl != null)
            toastControl.Show(savedMessage.TextByLanguage(localizeLanguage));

        if (mediaScannerControl != null)
            mediaScannerControl.StartScan(path);

        XDebug.Log("Save to : " + path);
    }

    ContentInfo lastScreenshot;     //For share file (Only last loaded)

    //Callback handler when MediaScanner scan completed.
    public void ReceiveMediaScan(ContentInfo info)
    {
        XDebug.Log("ReceiveMediaScan : " + info);

        lastScreenshot = info;
        SetEnableShareButtons(!string.IsNullOrEmpty(lastScreenshot.uri));
    }

    //UI-Buttons on/off
    public void SetEnableShareButtons(bool enable)
    {
        if (shareButton != null)
            shareButton.interactable = enable;
        if (mailButton != null)
            mailButton.interactable = enable;
    }

    //Share screenshot
    public void ShareScreenshot()
    {
        if (lastScreenshot == null)
            return;

        string uri = lastScreenshot.uri;
        string path = lastScreenshot.path;
        XDebug.Log("Last screenshot : path = " + path + ", uri = " + uri);
        
        if (string.IsNullOrEmpty(uri))
            return;

        if (sendTextControl != null)
            sendTextControl.Send(shareText.TextByLanguage(localizeLanguage), uri);
    }

    //Attach the image to an email and show
    //(*) However, when adding an attached file, it is the same method as 'Send text + attached file' (Other than the mailer is displayed).
    //※ただし、添付ファイルを追加する場合は、「テキスト送信＋添付ファイル」と同じ方法になる（メーラー以外も表示される）。
    public void SendMailScreenshot()
    {
        if (lastScreenshot == null)
            return;

        string uri = lastScreenshot.uri;
        string path = lastScreenshot.path;
        XDebug.Log("Last screenshot : path = " + path + ", uri = " + uri);

        if (string.IsNullOrEmpty(uri))
            return;

        if (mailerControl != null)
        {
            mailerControl.SetAttachment(uri);
            mailerControl.Show();
        }
    }

    //Callback handler for 'LocalizeLanguageChanger'
    public void OnLanguageChanged(SystemLanguage language)
    {
        XDebug.Log("Localize language changed (Share, Mail) : " + language);
        localizeLanguage = language;
    }
}
