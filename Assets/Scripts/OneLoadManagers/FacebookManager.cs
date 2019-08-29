using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using System;

public class FacebookManager : MonoBehaviour
{
    public static FacebookManager Instance;
    private Action SharedCallback;
    private Action ShareFailedCallback;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Start()
    {

    }

    public void Initialize()
    {
        FB.Init(SetInit, OnHideUnity);

    }

    private void SetInit()
    {
        if (FB.IsLoggedIn)
        {

        }
        else
        {

        }
    }
    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            Time.timeScale = 0;
            AudioManager.Instance.setMute(true);
        }
        else
        {
            Time.timeScale = 1;
            AudioManager.Instance.setMute(!GameData.Instance.isSoundOn);
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
            ShareWithFriends(SharedCallback, ShareFailedCallback);
        }
        else
        {

            Debug.Log("FB login fail");
            ShareFailedCallback?.Invoke();
        }
    }
    public void ShareWithFriends(Action onSuccess, Action onFailed)
    {
        SharedCallback = onSuccess;
        ShareFailedCallback = onFailed;
        if (FB.IsLoggedIn)
        {
#if UNITY_IPHONE
            FB.Mobile.ShareDialogMode = ShareDialogMode.FEED;
            FB.ShareLink(
                contentURL: new System.Uri("https://play.google.com/store/apps/details?id=com.waterline.pipeman"),
                callback: ShareCallback
                );
#else
            FB.FeedShare(
                link: new System.Uri("https://play.google.com/store/apps/details?id=com.waterline.pipeman"),
                callback: ShareCallback
                );
#endif
        }
        else
        {
            FBLogin();
        }
    }

    private void ShareCallback(IShareResult result)
    {
        Debug.Log("Result.Error = " + result.Error);
        Debug.Log("Result.Cancelled = " + result.Cancelled);
        Debug.Log("Result.PostID = " + result.PostId);
        Debug.Log("Result.RawResutl = " + result.RawResult);
        if(!result.Cancelled && string.IsNullOrEmpty(result.Error))
        {
            SharedCallback?.Invoke();
            Debug.Log("Share fb done");
        }
        else
        {
            ShareFailedCallback?.Invoke();
            Debug.Log("Share failed");
        }
        Debug.Log("-----------------");
    }

    public void LogEventLevelStart(int level, string type, int day)
    {
        var dictionary = new Dictionary<string, object>();
        dictionary.Add("level", level);
        dictionary.Add("type", type);
        dictionary.Add("day", day);
        FB.LogAppEvent("level_start", parameters: dictionary);
    }

    public void LogEventLevelEnd(int level, string type, int day, float duration_secs, int turn_count, int remove_pipe_count, int construct_pipe_count)
    {
        var dictionary = new Dictionary<string, object>();
        dictionary.Add("level", level);
        dictionary.Add("type", type);
        dictionary.Add("day", day);
        dictionary.Add("duration_secs", duration_secs);
        dictionary.Add("turn_count", turn_count);
        dictionary.Add("remove_pipe_count", remove_pipe_count);
        dictionary.Add("construct_pipe_count", construct_pipe_count);
        FB.LogAppEvent("level_end", parameters: dictionary);
    }

    public void LogEventRequestRewardedVideo(string purpose, bool has_video, int level)
    {
        var dictionary = new Dictionary<string, object>();
        dictionary.Add("purpose", purpose);
        dictionary.Add("has_video", has_video);
        dictionary.Add("level", level);
        FB.LogAppEvent("start_rewarded_ads", parameters: dictionary);
    }

    public void LogEventShareFacebook()
    {
        FB.LogAppEvent("share_facebook");
    }

}
