using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace ImoSysSDK.Network
{
    using Core;
    using System.Collections.Generic;

    public class RestClient
    {
//#if ENV_PROD || ENV_STAGE
        public const string DOMAIN = "https://api.gamesontop.com";
//#else
        //public const string DOMAIN = "https://api-staging.gamesontop.com";
//#endif
        
        public static string deviceId;

        public delegate void OnRequestFinished(long statusCode, string message, string data);

        public static void SendPostRequest(string path, string sJson, OnRequestFinished onRequestFinished) {
            SendRequest(UnityWebRequest.kHttpVerbPOST, path, null, sJson, onRequestFinished);
        }

        public static void SendGetRequest(string path, Dictionary<string, string> queryParams, OnRequestFinished onRequestFinished) {
            SendRequest(UnityWebRequest.kHttpVerbGET, path, queryParams, null, onRequestFinished);
        }

        private static void SendRequest(string method, string path, Dictionary<string, string> queryParams, string sJson, OnRequestFinished onRequestFinished)
        {
            if (deviceId == null)
            {
#if UNITY_EDITOR
                deviceId = ImoSysSDK.Instance.DeviceId;
                SendRequest(deviceId, method, path, queryParams, sJson, onRequestFinished);
#else
#if UNITY_ANDROID
                AndroidJavaClass pluginClass = new AndroidJavaClass("com.imosys.core.ImoSysIdentifier");
                AndroidJavaObject imosysIdentifierObject = pluginClass.CallStatic<AndroidJavaObject>("getInstance");
                imosysIdentifierObject.Call("getDeviceIdAsync", new ImoSysIdentifierPluginCallback((deviceId) =>
                {
                    RestClient.deviceId = deviceId;
                    SendRequest(RestClient.deviceId, method, path, queryParams, sJson, onRequestFinished);
                }));

#elif UNITY_IOS
                deviceId = ImoSysSDK.Instance.DeviceId;
                SendRequest(deviceId, method, path, queryParams, sJson, onRequestFinished);
#endif
#endif
            } else
            {
                SendRequest(deviceId, method, path, queryParams, sJson, onRequestFinished);
            }
        }

        private static void SendRequest(string deviceId, string method, string path, Dictionary<string, string> queryParams, string sJson, OnRequestFinished onRequestFinished)
        {
            StringBuilder sb = new StringBuilder(DOMAIN);
            sb.Append(path);
            if (queryParams != null && queryParams.Count > 0) {
                sb.Append('?');
                bool first = true;
                foreach(KeyValuePair<string, string> entry in queryParams) {
                    if (!first) {
                        sb.Append('&');
                    } else {
                        first = false;
                    }
                    sb.Append(UnityWebRequest.EscapeURL(entry.Key));
                    sb.Append('=');
                    sb.Append(UnityWebRequest.EscapeURL(entry.Value));
                }
                sb.Append(Encoding.UTF8.GetString(UnityWebRequest.SerializeSimpleForm(queryParams)));
            }
            string url = sb.ToString();
            UnityWebRequest request = new UnityWebRequest(url, method);
            if ((method.Equals(UnityWebRequest.kHttpVerbPOST) || method.Equals(UnityWebRequest.kHttpVerbPUT)) && !string.IsNullOrEmpty(sJson)) {
                Debug.Log("Post body: " + sJson);
                request.SetRequestHeader("Content-Type", "application/json");
                request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(sJson));
            }
            AddRequiredHeaders(request, deviceId);
            request.downloadHandler = new DownloadHandlerBuffer();
            UnityWebRequestAsyncOperation operation = request.SendWebRequest();
            operation.completed += (ops) => {
                string message = request.isNetworkError ? request.error : string.Empty;
                onRequestFinished(request.responseCode, message, request.downloadHandler.text);
            };            
        }

        private static void AddRequiredHeaders(UnityWebRequest request, string deviceId)
        {
            request.SetRequestHeader("pn", Application.identifier);
#if UNITY_ANDROID
            request.SetRequestHeader("p", "1");
#else
            request.SetRequestHeader("p", "2");
#endif
            request.SetRequestHeader("d", deviceId);
        }
    }
}