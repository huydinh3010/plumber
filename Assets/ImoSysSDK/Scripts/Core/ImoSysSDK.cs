﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ImoSysSDK.Core
{

    public class ImoSysSDK
    {

        private static readonly object instanceLock = new object();
        private static ImoSysSDK instance;

        private string deviceId;

        [RuntimeInitializeOnLoadMethod]
        public static void Init() {
            Instance.Initialize();
        }

        private ImoSysSDK()
        {
        }

        public static ImoSysSDK Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (instanceLock)
                    {
                        if (instance == null)
                        {
                            instance = new ImoSysSDK();
                        }
                    }
                }
                return instance;
            }
        }

#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void IOSInitialize();

    [DllImport("__Internal")]
    private static extern IntPtr IOSGetDeviceId();
#endif


        private void Initialize()
        {
#if UNITY_IOS
        IOSInitialize();
#endif
        }

        public string DeviceId
        {
            get
            {
                if (this.deviceId == null)
                {
#if UNITY_EDITOR
                    this.deviceId = PlayerPrefs.GetString("editor_device_id");
                    if (string.IsNullOrEmpty(deviceId)) {
                        this.deviceId = "uid:" + System.Guid.NewGuid().ToString();
                        PlayerPrefs.SetString("editor_device_id", this.deviceId);
                    }
#else
#if UNITY_IOS
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    IntPtr deviceIdPtr = IOSGetDeviceId();
                    this.deviceId = Marshal.PtrToStringAnsi(deviceIdPtr);
                }
#elif UNITY_ANDROID
                    if (Application.platform == RuntimePlatform.Android)
                    {
                        AndroidJavaClass pluginClass = new AndroidJavaClass("com.imosys.core.ImoSysIdentifier");
                        AndroidJavaObject imosysIdentifierObject = pluginClass.CallStatic<AndroidJavaObject>("getInstance");
                        this.deviceId = imosysIdentifierObject.Call<string>("getDeviceIdSync");
                    }
#endif
#endif
                }
                return deviceId;
            }
        }

    }
}