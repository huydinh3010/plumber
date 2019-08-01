using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;

    private InterstitialAd[] interstitials = new InterstitialAd[2];
    private RewardBasedVideoAd rewardBasedVideo;
    private BannerView bannerView;
    private Action RewardedCallback;
    private Action ClosedInterstitialCallback;
    private Action BannerLoadedCallback;
    private Action BannerClosedCallback;
    private int interstitial1Count;
    private int interstitial2Count;
    private const int INTERSTITIAL_STEP = 1;
    private int interstitialIndex;
    private bool rewarded;
    private float bannerHeight;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    public void Initialize()
    {
#if UNITY_ANDROID
        string appId = "ca-app-pub-3940256099942544~3347511713";
        // string appId = "ca-app-pub-8912425266737526~2657028361";
#elif UNITY_IPHONE
            string appId = "ca-app-pub-3940256099942544~1458002511";
#else
            string appId = "unexpected_platform";
#endif
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(appId);
        this.rewardBasedVideo = RewardBasedVideoAd.Instance;
        rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
        this.RequestRewardBasedVideo();
        if (GameData.Instance.isAdsOn)
        {
            for(int i = 0; i < interstitials.Length; i++)
            {
                RequestInterstitial(i);
            }
            RequestBanner();
        }
    }

    private void RequestInterstitial(int index)
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/1033173712";
        //string adUnitId = "ca-app-pub-8912425266737526/2510576754";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif
        if (interstitials[index] != null) interstitials[index].Destroy();
        interstitials[index] = new InterstitialAd(adUnitId);
        interstitials[index].OnAdClosed += (object sender, EventArgs args) => {
            ClosedInterstitialCallback?.Invoke();
            RequestInterstitial(index);
        };
        AdRequest request = new AdRequest.Builder().Build();
        interstitials[index].LoadAd(request);
    }

    public bool canShowInterstitial1()
    {
        if (!GameData.Instance.isAdsOn) return false;
        if (++interstitial1Count == INTERSTITIAL_STEP)
        {
            interstitial1Count = 0;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool canShowInterstitial2()
    {
        if (!GameData.Instance.isAdsOn) return false;
        if (rewarded)
        {
            rewarded = false;
            return false;
        }
        if (++interstitial2Count == INTERSTITIAL_STEP)
        {
            interstitial2Count = 0;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool ShowInterstitial(Action action)
    {
        if (interstitials[interstitialIndex].IsLoaded())
        {
            ClosedInterstitialCallback = action;
            interstitials[interstitialIndex].Show();
            interstitialIndex = (interstitialIndex + 1) % 2;
            rewarded = false;
            return true;
        }
        else
        {
            RequestInterstitial(interstitialIndex);
            interstitialIndex = (interstitialIndex + 1) % 2;
            return false;
        }
    }

    private void RequestRewardBasedVideo()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/5224354917";
        //string adUnitId = "ca-app-pub-8912425266737526/8950973977";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
            string adUnitId = "unexpected_platform";
#endif

        AdRequest request = new AdRequest.Builder().Build();
        this.rewardBasedVideo.LoadAd(request, adUnitId);
    }

    private void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        rewarded = true;
        Debug.Log("HandleRewardBasedVideoRewarded event received for "
                        + amount.ToString() + " " + type);
        RewardedCallback?.Invoke();
    }

    public bool ShowRewardVideo(Action action)
    {
        if (rewardBasedVideo.IsLoaded())
        {
            Debug.Log("Show RewardVideo");
            RewardedCallback = action;
            rewardBasedVideo.Show();
            return true;
        }
        else
        {
            RequestRewardBasedVideo();
            return false;
        }
    }

    private void RequestBanner()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
            string adUnitId = "unexpected_platform";
#endif
        bannerHeight = 0;
        if (bannerView != null) bannerView.Destroy();
        bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);
        bannerView.OnAdClosed += HandleOnBannerAdsClosed;
        bannerView.OnAdLoaded += HandleOnBannerAdsLoaded;
        //bannerView.OnAdFailedToLoad += HandleOnBannerFailedToLoad;
        AdRequest request = new AdRequest.Builder().Build();
        bannerView.LoadAd(request);
    }
    private void HandleOnBannerAdsClosed(object sender, EventArgs args)
    {
        Debug.Log("HandleAdClosed event received");
        BannerClosedCallback?.Invoke();
        bannerHeight = 0;
        RequestBanner();
    }

    private void HandleOnBannerAdsLoaded(object sender, EventArgs args)
    {
        Debug.Log("Banner Ads Loaded");
        bannerHeight = bannerView.GetHeightInPixels();
        BannerLoadedCallback?.Invoke();
    }

    //private void HandleOnBannerFailedToLoad(object sender, EventArgs args)
    //{
    //    BannerClosedCallback?.Invoke();
    //}

    public void ShowNewBanner()
    {
        if (GameData.Instance.isAdsOn) RequestBanner();
    }

    public float GetBannerHeight()
    {

        return bannerHeight;
    }

    public void AddBannerCallback(Action loaded, Action closed)
    {
        BannerLoadedCallback = loaded;
        BannerClosedCallback = closed;
    }

    public void ClearBannerCallback()
    {
        BannerLoadedCallback = null;
        BannerClosedCallback = null;
    }
}


