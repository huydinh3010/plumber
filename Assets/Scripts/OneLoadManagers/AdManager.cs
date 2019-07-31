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
    private int interstitial1Count;
    private int interstitial2Count;
    private const int INTERSTITIAL_STEP = 1;
    private bool rewarded;
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
            this.RequestInterstitial();
            this.RequestBanner();
        }
    }

    private void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/1033173712";
        //string adUnitId = "ca-app-pub-8912425266737526/2510576754";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif
        if (interstitial != null) interstitial.Destroy();
        this.interstitial = new InterstitialAd(adUnitId);
        this.interstitial.OnAdClosed += HandleOnIntersitialAdClosed;
        AdRequest request = new AdRequest.Builder().Build();
        this.interstitial.LoadAd(request);
    }

    private void HandleOnIntersitialAdClosed(object sender, EventArgs args)
    {
        ClosedInterstitialCallback?.Invoke();
        Debug.Log("HandleAdClosed event received");
        RequestInterstitial();
    }

    public bool ShowInterstitial1(Action action)
    {
        if (!GameData.Instance.isAdsOn ) return false;
        if(rewarded)
        {
            rewarded = false;
            return false;
        }
        interstitial1Count++;
        if (interstitial1Count == INTERSTITIAL_STEP)
        {
            if (interstitial.IsLoaded())
            {
                ClosedInterstitialCallback = action;
                this.interstitial.Show();
                interstitial1Count = 0;
                return true;
            }
            else
            {
                RequestInterstitial();
                interstitial1Count--;
            }
        }
        return false;
    }

    public bool ShowInterstitial2(Action action)
    {
        rewarded = false;
        if (!GameData.Instance.isAdsOn) return false;
        interstitial2Count++;
        if (interstitial2Count == INTERSTITIAL_STEP)
        {
            if (interstitial.IsLoaded())
            {
                ClosedInterstitialCallback = action;
                this.interstitial.Show();
                interstitial2Count = 0;
                return true;
            }
            else
            {
                RequestInterstitial();
                interstitial2Count--;
            }
        }
        return false;
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
        if (bannerView != null) bannerView.Destroy();
        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
        bannerView.OnAdClosed += HandleOnBannerAdsClosed;
        bannerView.OnAdLoaded += HandleOnBannerAdsLoaded;
        AdRequest request = new AdRequest.Builder().Build();
        bannerView.LoadAd(request);
    }
    private void HandleOnBannerAdsClosed(object sender, EventArgs args)
    {
        Debug.Log("HandleAdClosed event received");
        BannerClosedCallback?.Invoke();
        RequestBanner();
    }

    private void HandleOnBannerAdsLoaded(object sender, EventArgs args)
    {
        BannerLoadedCallback();
    }

    public void ShowNewBanner()
    {
        if (GameData.Instance.isAdsOn) RequestBanner();
    }

    public float GetBannerHeight()
    {
        return bannerView == null ? 0 : bannerView.GetHeightInPixels();
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


