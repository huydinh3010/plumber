using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using System;

public class FacebookManager : MonoBehaviour
{
    public static FacebookManager Instance;
    private Action SharedCallback;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Start()
    {
        FB.Init(SetInit, OnHideUnity);
    }

    private void SetInit()
    {
        Debug.Log("Facebook init done!");
        if (FB.IsLoggedIn)
        {
            //FB.ActivateApp();
            // Fb logged in
        }
        else
        {
            //call login facebook
        }
    }
    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
    private void FBLogin()
    {
        FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email" }, AuthCallback);
    }
    void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            Debug.Log("FB login worked");
            ShareWithFriends(SharedCallback);
        }
        else
        {
            Debug.Log("FB login fail");
        }
    }
    public void ShareWithFriends(Action action)
    {
        SharedCallback = action;
        if (FB.IsLoggedIn)
        {
            FB.FeedShare(
                link: new System.Uri("https://testgame.huydinh"),
                callback: ShareCallback
                );
        }
        else
        {
            FBLogin();
        }
    }

    private void ShareCallback(IShareResult result)
    {
        if(!result.Cancelled && string.IsNullOrEmpty(result.Error))
        {
            SharedCallback();
        }
    }
    
}
