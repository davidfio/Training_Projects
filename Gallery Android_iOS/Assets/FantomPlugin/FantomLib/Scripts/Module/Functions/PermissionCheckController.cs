using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace FantomLib
{
    /// <summary>
    /// Permission Check Controller
    /// 
    /// Check if permission is given and call back (Synonymous with 'in AndroidManifest.xml').
    /// https://developer.android.com/reference/android/Manifest.permission.html
    ///·Use "Constant Value" in the developer manual for the permission string (eg: "android.permission.RECORD_AUDIO").
    ///
    /// 
    /// パーミッションが許可（付与）されているかどうかを調べ、コールバックする（「AndroidManifest.xml」にあるか？と同義）。
    ///・パーミッションの文字列はデベロッパーマニュアルの「Constant Value」を使う（例："android.permission.RECORD_AUDIO"）。
    /// https://developer.android.com/reference/android/Manifest.permission.html
    /// 
    ///
    ///·Permissions used in fantomPlugin are as follows:
    ///・プラグインで利用するパーミッションは以下の通り：
    /// android.permission.RECORD_AUDIO
    /// android.permission.WRITE_EXTERNAL_STORAGE (or android.permission.READ_EXTERNAL_STORAGE : When read only)
    /// android.permission.BLUETOOTH
    /// android.permission.VIBRATE
    /// android.permission.BODY_SENSORS
    /// </summary>
    public class PermissionCheckController : MonoBehaviour
    {
        //Inspector Settings
        public string permission = "android.permission.WRITE_EXTERNAL_STORAGE";     //Permission to check

        public bool checkOnStart = false;   //Execute check automatically at 'Start()'

        //Callbacks
        [Serializable] public class ResultHandler : UnityEvent<string, bool> { }    //permission, granted
        public ResultHandler OnResult;

        public UnityEvent OnGranted;        //When permission granted
        public UnityEvent OnDenied;         //When permission denied



        // Use this for initialization
        private void Start()
        {
            if (checkOnStart)
                CheckPermission();
        }

        // Update is called once per frame
        //private void Update()
        //{

        //}


        //Check for permission (using local value)
        public void CheckPermission()
        {
            if (string.IsNullOrEmpty(permission))
                return;

#if UNITY_EDITOR
            Debug.Log("PermissionCheckController.CheckPermission called.");
#elif UNITY_ANDROID
            bool granted = AndroidPlugin.CheckPermission(permission);
            if (OnResult != null)
                OnResult.Invoke(permission, granted);

            if (granted)
            {
                if (OnGranted != null)
                    OnGranted.Invoke();
            }
            else
            {
                if (OnDenied != null)
                    OnDenied.Invoke();
            }
#endif
        }

        //Set permission string dynamically and check (current permission string will be overwritten)
        public void CheckPermission(string permission)
        {
            this.permission = permission;
            CheckPermission();
        }
    }
}
