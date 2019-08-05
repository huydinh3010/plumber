using Facebook.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ImoSysSDK.SocialPlatforms {

    public class FacebookLogin {

        private const string LOGIN_PATH = "/v1/games/facebook/login";

        public delegate void OnLoginSuccessEvent();

        public delegate void OnLoginFailedEvent(string message);

        public event OnLoginFailedEvent onLoginFailed;

        public event OnLoginSuccessEvent onLoginSuccess;

        public FacebookLogin(OnLoginFailedEvent onLoginFailed, OnLoginSuccessEvent onLoginSuccess) {
            this.onLoginFailed = onLoginFailed;
            this.onLoginSuccess = onLoginSuccess;
        }

        public void Login() {
            if (FB.IsInitialized) {
                FB.ActivateApp();
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
                FB.ActivateApp();
                InternalLogin();
            } else {
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
            }
        }

        private void LoginWithServer(AccessToken accessToken) {
            FacebookInfoBody body = new FacebookInfoBody(accessToken.UserId, accessToken.TokenString);
            Network.RestClient.SendPostRequest(LOGIN_PATH, JsonUtility.ToJson(body), OnRequestFinished);
        }

        private void OnRequestFinished(long statusCode, string message, string data) {
            if (statusCode == 200) {
                if (onLoginSuccess != null) {
                    onLoginSuccess();
                }
            } else {
                onLoginFailed(message);
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