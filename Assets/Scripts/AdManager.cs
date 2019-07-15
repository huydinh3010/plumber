using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;

    private InterstitialAd interstitial;
    private RewardBasedVideoAd rewardBasedVideo;
    private Action RewardedCallback;
    private Action ClosedInterstitialCallback;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_ANDROID
        string appId = "ca-app-pub-3940256099942544~3347511713";
#elif UNITY_IPHONE
            string appId = "ca-app-pub-3940256099942544~1458002511";
#else
            string appId = "unexpected_platform";
#endif
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(appId);

        // Get singleton reward based video ad reference.
        this.rewardBasedVideo = RewardBasedVideoAd.Instance;

        // Called when an ad request has successfully loaded.
        rewardBasedVideo.OnAdLoaded += HandleRewardBasedVideoLoaded;
        // Called when an ad request failed to load.
        rewardBasedVideo.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
        // Called when an ad is shown.
        rewardBasedVideo.OnAdOpening += HandleRewardBasedVideoOpened;
        // Called when the ad starts to play.
        rewardBasedVideo.OnAdStarted += HandleRewardBasedVideoStarted;
        // Called when the user should be rewarded for watching a video.
        rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
        // Called when the ad is closed.
        rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
        // Called when the ad click caused the user to leave the application.
        rewardBasedVideo.OnAdLeavingApplication += HandleRewardBasedVideoLeftApplication;

        this.RequestRewardBasedVideo();
        this.RequestInterstitial();
    }

    private void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(adUnitId);

        // Called when an ad request has successfully loaded.
        this.interstitial.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        this.interstitial.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        this.interstitial.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        this.interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
    }
    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        //MonoBehaviour.print("HandleAdLoaded event received");
        Debug.Log("HandleAdLoaded event received");
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        //MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
        //                    + args.Message);
        Debug.Log("HandleFailedToReceiveAd event received with message: "
                            + args.Message);
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        //MonoBehaviour.print("HandleAdOpened event received");
        Debug.Log("HandleAdOpened event received");
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        ClosedInterstitialCallback();
        //MonoBehaviour.print("HandleAdClosed event received");
        Debug.Log("HandleAdClosed event received");
        interstitial.Destroy();
        RequestInterstitial();
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        //MonoBehaviour.print("HandleAdLeavingApplication event received");
        Debug.Log("HandleAdLeavingApplication event received");
    }
    public bool ShowInterstitial(Action action)
    {
        if (this.interstitial.IsLoaded())
        {
            ClosedInterstitialCallback = action;
            this.interstitial.Show();
            return true;
        }
        else
        {
            RequestInterstitial();
            return false;
        }
        
    }

    private void RequestRewardBasedVideo()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
            string adUnitId = "unexpected_platform";
#endif
        Debug.Log("Request Reward Video");
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded video ad with the request.
        this.rewardBasedVideo.LoadAd(request, adUnitId);
    }
    public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
    {
        //MonoBehaviour.print("HandleRewardBasedVideoLoaded event received");
        Debug.Log("HandleRewardBasedVideoLoaded event received");
    }

    public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        //MonoBehaviour.print(
        //    "HandleRewardBasedVideoFailedToLoad event received with message: "
        //                     + args.Message);
        Debug.Log("HandleRewardBasedVideoFailedToLoad event received with message: "
                             + args.Message);
    }

    public void HandleRewardBasedVideoOpened(object sender, EventArgs args)
    {
        //MonoBehaviour.print("HandleRewardBasedVideoOpened event received");
        Debug.Log("HandleRewardBasedVideoOpened event received");
    }

    public void HandleRewardBasedVideoStarted(object sender, EventArgs args)
    {
        //MonoBehaviour.print("HandleRewardBasedVideoStarted event received");
        Debug.Log("HandleRewardBasedVideoStarted event received");
    }

    public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
    {
        //MonoBehaviour.print("HandleRewardBasedVideoClosed event received");
        Debug.Log("HandleRewardBasedVideoClosed event received");
        this.RequestRewardBasedVideo();
    }

    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        //MonoBehaviour.print(
        //    "HandleRewardBasedVideoRewarded event received for "
        //                + amount.ToString() + " " + type);
        Debug.Log("HandleRewardBasedVideoRewarded event received for "
                        + amount.ToString() + " " + type);
        RewardedCallback();
    }

    public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
    {
        //MonoBehaviour.print("HandleRewardBasedVideoLeftApplication event received");
        Debug.Log("HandleRewardBasedVideoLeftApplication event received");
    }
    public void ShowRewardVideo(Action action)
    {
        if (rewardBasedVideo.IsLoaded())
        {
            Debug.Log("Show RewardVideo");
            RewardedCallback = action;
            rewardBasedVideo.Show();
        }
        else
        {
            RequestRewardBasedVideo();
        }
    }
}
