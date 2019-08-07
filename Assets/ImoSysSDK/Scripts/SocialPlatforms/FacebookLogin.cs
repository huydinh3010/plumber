using Facebook.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ImoSysSDK.SocialPlatforms {

    public class FacebookHelper {

        private static readonly object instanceLock = new object();
        private static FacebookHelper instance;

        private const string LOGIN_PATH = "/v1/games/facebook/login";
        private const string KEY_FACEBOOK_LOGGED_IN = "imo_is_fb_logged_in";
        private const string KEY_FACEBOOK_USER_ID = "imo_facebook_user_id";

        public delegate void OnLoginSuccessEvent();

        public delegate void OnLoginFailedEvent(string message);

        public event OnLoginFailedEvent onLoginFailed;

        public event OnLoginSuccessEvent onLoginSuccess;

        public static FacebookHelper Instance {
            get {
                if (instance == null) {
                    lock (instanceLock) {
                        if (instance == null) {
                            instance = new FacebookHelper();
                        }
                    }
                }
                return instance;
            }
        }

        public FacebookHelper() {
        }
        
        public bool IsLoggedIn {
            get {
                return PlayerPrefs.GetInt(KEY_FACEBOOK_LOGGED_IN, 0) != 0;
            }
            set {
                PlayerPrefs.SetInt(KEY_FACEBOOK_LOGGED_IN, value ? 1 : 0);
            }
        }

        public string FacebookUserId {
            get {
                return PlayerPrefs.GetString(KEY_FACEBOOK_USER_ID, string.Empty);
            }
            set {
                PlayerPrefs.SetString(KEY_FACEBOOK_USER_ID, value);
            }
        }

        public void Login(OnLoginFailedEvent onLoginFailed, OnLoginSuccessEvent onLoginSuccess) {
            this.onLoginFailed = onLoginFailed;
            this.onLoginSuccess = onLoginSuccess;
            if (FB.IsInitialized) {
                InternalLogin();
            } else {
                FB.Init(OnInitComplete, OnHideUnity);
            }
        }

        private void OnHideUnity(bool isGameShown) {
            if (!isGameShown) {
                Time.timeScale = 0;
            } else {
                Time.timeScale = 1;
            }
        }

        private void OnInitComplete() {
            if (FB.IsInitialized) {
                InternalLogin();
            } else {
                onLoginFailedCallback("Failed to initialize the Facebook SDK");
                Debug.Log("Failed to initialize the Facebook SDK");
            }
        }

        private void InternalLogin() {
            if (!FB.IsLoggedIn) {
                FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email" }, AuthCallback);
            } else {
                LoginWithServer(AccessToken.CurrentAccessToken);
            }
        }

        private void AuthCallback(ILoginResult result) {
            if (FB.IsLoggedIn) {
                var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
                LoginWithServer(aToken);
            } else {
                Debug.Log("User cancelled login");
                onLoginFailedCallback("User cancelled login");
            }
        }

        private void LoginWithServer(AccessToken accessToken) {
            FacebookInfoBody body = new FacebookInfoBody(accessToken.UserId, accessToken.TokenString);
            Network.RestClient.SendPostRequest(LOGIN_PATH, JsonUtility.ToJson(body), OnRequestFinished);
        }

        private void OnRequestFinished(long statusCode, string message, string data) {
            Debug.Log("IMO Message: " + message);
            Debug.Log("IMO Data: " + data);
            if (statusCode == 200) {
                IsLoggedIn = true;
                FacebookUserId = AccessToken.CurrentAccessToken.UserId;
                GameServices.Instance.PlayerId = "fb:" + AccessToken.CurrentAccessToken.UserId;
                onLoginSuccessCallback();
            } else {
                onLoginFailedCallback(message);
            }
        }

        private void onLoginFailedCallback(string message) {
            if (onLoginFailed != null) {
                onLoginFailed(message);
                onLoginFailed = null;
            }
        }

        private void onLoginSuccessCallback() {
            if (onLoginSuccess != null) {
                onLoginSuccess();
                onLoginSuccess = null;
            }
        }
    }


    public class FacebookInfoBody {
        public string fbId;
        public string fbToken;

        public FacebookInfoBody(string fbId, string fbToken) {
            this.fbId = fbId;
            this.fbToken = fbToken;
        }
    }
}