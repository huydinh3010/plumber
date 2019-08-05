using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace ImoSysSDK.Network
{
    using Core;
    public class RestClient
    {
//#if ENV_PROD || ENV_STAGE
        public const string DOMAIN = "https://api.gamesontop.com";
//#else
        //public const string DOMAIN = "https://api-staging.gamesontop.com";
//#endif
        
        public static string deviceId;

        public delegate void OnRequestFinished(long statusCode, string message, string data);

        public static void SendPostRequest(string url, string sJson, OnRequestFinished onRequestFinished)
        {
            if (deviceId == null)
            {
#if UNITY_ANDROID
                AndroidJavaClass pluginClass = new AndroidJavaClass("com.imosys.core.ImoSysIdentifier");
                AndroidJavaObject imosysIdentifierObject = pluginClass.CallStatic<AndroidJavaObject>("getInstance");
                imosysIdentifierObject.Call("getDeviceIdAsync", new ImoSysIdentifierPluginCallback((deviceId) =>
                {
                    RestClient.deviceId = deviceId;
                    SendPostRequest(deviceId, url, sJson, onRequestFinished);
                }));

#elif UNITY_IOS
                deviceId = ImoSysSDK.Instance.DeviceId;
                SendPostRequest(deviceId, url, sJson, onRequestFinished);
#endif
            }
            else
            {
                SendPostRequest(deviceId, url, sJson, onRequestFinished);
            }
        }

        public static void SendPostRequest(string deviceId, string url, string sJson, OnRequestFinished onRequestFinished)
        {
            //Debug.Log("Post body: " + sJson);
            UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
            request.SetRequestHeader("Content-Type", "application/json");
            AddRequiredHeaders(request, deviceId);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(sJson));
            UnityWebRequestAsyncOperation operation = request.SendWebRequest();
            operation.completed += (ops) => {
                string message = request.isNetworkError ? request.error : string.Empty;
                onRequestFinished(request.responseCode, message, request.downloadHandler.text);
            };            
        }

        private static void AddRequiredHeaders(UnityWebRequest request, string deviceId)
        {
            request.SetRequestHeader("pn", Application.identifier);
            request.SetRequestHeader("p", Application.platform == RuntimePlatform.Android ? "1" : "2");
            request.SetRequestHeader("d", deviceId);
        }
    }
}