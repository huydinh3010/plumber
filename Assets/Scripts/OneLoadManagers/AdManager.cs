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
    private BannerView bannerView;
    private Action RewardedCallback;
    private Action ClosedInterstitialCallback;
    private Action BannerLoadedCallback;
    private Action BannerClosedCallback;
    private float bannerHeight;
    private float timer = 0f;
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

    private void Update()
    {
        if (timer > 0) timer -= Time.deltaTime;
    }

    public void Initialize()
    {
#if UNITY_ANDROID
#if ENV_PROD
        string appId = "ca-app-pub-8912425266737526~2657028361";
#else
        string appId = "ca-app-pub-3940256099942544~3347511713";
#endif
#elif UNITY_IPHONE
#if ENV_PROD
        string appId = "";
#else
        string appId = "ca-app-pub-3940256099942544~1458002511";
#endif
#else
        string appId = "unexpected_platform";
#endif
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(appId);
        this.rewardBasedVideo = RewardBasedVideoAd.Instance;
        
        rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
        rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
        this.RequestRewardBasedVideo();
        if (GameData.Instance.isAdsOn)
        {
            RequestInterstitial();
            RequestBanner();
        }
    }

    

    private void RequestInterstitial()
    {
#if UNITY_ANDROID

#if ENV_PROD
        string adUnitId = "ca-app-pub-8912425266737526/2510576754";
#else
        string adUnitId = "ca-app-pub-3940256099942544/1033173712";
#endif
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif
        if (interstitial != null) interstitial.Destroy();
        interstitial = new InterstitialAd(adUnitId);
        interstitial.OnAdClosed += (object sender, EventArgs args) => {
            ClosedInterstitialCallback?.Invoke();
            RequestInterstitial();
            timer = 20f;
        };
        AdRequest request = new AdRequest.Builder().Build();
        interstitial.LoadAd(request);
    }

    public bool canShowInterstitial()
    {
        if (!GameData.Instance.isAdsOn) return false;
        return timer <= 0;
    }

    

    public bool ShowInterstitial(Action action)
    {
        if (interstitial.IsLoaded())
        {
            ClosedInterstitialCallback = action;
            interstitial.Show();
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
#if ENV_PROD
        string adUnitId = "ca-app-pub-8912425266737526/8950973977";
#else
        string adUnitId = "ca-app-pub-3940256099942544/5224354917";
#endif
#elif UNITY_IPHONE
        
#if ENV_PROD
        string adUnitId = "";
#else
        string adUnitId = "ca-app-pub-3940256099942544/1712485313";
#endif
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
        //Debug.Log("HandleRewardBasedVideoRewarded event received for "
                        //+ amount.ToString() + " " + type);
        RewardedCallback?.Invoke();
    }

    private void HandleRewardBasedVideoClosed(object sender, EventArgs e)
    {
        RequestRewardBasedVideo();
    }

    public bool ShowRewardVideo(Action action)
    {
        if (rewardBasedVideo.IsLoaded())
        {
            //Debug.Log("Show RewardVideo");
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

    public void RequestBanner()
    {
#if UNITY_ANDROID
#if ENV_PROD
        string adUnitId = "ca-app-pub-8912425266737526/9538246423";
#else
        string adUnitId = "ca-app-pub-3940256099942544/6300978111";
#endif
#elif UNITY_IPHONE
#if ENV_PROD
        string adUnitId = "";
#else
        string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#endif
#else
        string adUnitId = "unexpected_platform";
#endif
        bannerHeight = 0;
        if (bannerView != null) bannerView.Destroy();
        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
        bannerView.OnAdClosed += HandleOnBannerAdsClosed;
        bannerView.OnAdLoaded += HandleOnBannerAdsLoaded;
        //bannerView.OnAdFailedToLoad += HandleOnBannerFailedToLoad;
        AdRequest request = new AdRequest.Builder().Build();
        bannerView.LoadAd(request);
    }
    private void HandleOnBannerAdsClosed(object sender, EventArgs args)
    {
        //Debug.Log("HandleAdClosed event received");
        BannerClosedCallback?.Invoke();
        bannerHeight = 0;
        RequestBanner();
    }

    private void HandleOnBannerAdsLoaded(object sender, EventArgs args)
    {
        //Debug.Log("Banner Ads Loaded");
        bannerHeight = bannerView.GetHeightInPixels() + 20;
        // 
        bannerView.Show();
        //
        BannerLoadedCallback?.Invoke();
    }

    public bool isBannerShowing()
    {
        return bannerHeight > 0;
    }

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

    public void CloseBanner()
    {
        bannerView.Destroy();
        bannerHeight = 0f;
        BannerClosedCallback?.Invoke();
    }

    public void ClearBannerCallback()
    {
        BannerLoadedCallback = null;
        BannerClosedCallback = null;
    }
}