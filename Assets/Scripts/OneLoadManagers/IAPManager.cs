using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviour
{
    public static IAPManager Instance;

    [SerializeField] Sprite coin;
    [SerializeField] Sprite noAds;
    [SerializeField] Sprite error;

    private Action noAdsCallback;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        
    }

    public void RegisterNoAdsCallback(Action action)
    {
        noAdsCallback += action;
    }

    public void RemoveNoAdsCallback(Action action)
    {
        noAdsCallback -= action;
    }

    public void OnIAPNoAdsCompleted(Product product, int instanceId)
    {
        GameData.Instance.isAdsOn = false;
        AdManager.Instance.CloseBanner();
        Debug.Log("IAP noads completed");
        PopupManager.Instance.ShowNotification("Thanks for buying. Now the game will not show ads!", noAds, 2.5f);
        noAdsCallback?.Invoke();
    }

    public void OnIAPBuy100CoinsCompleted(Product product, int instanceId)
    {
        GameData.Instance.increaseCoin(100);
        Debug.Log("IAP Completed: " + instanceId);
        PopupManager.Instance.ShowNotification("Thanks for buying. You get 100 coins", coin, 2.5f);
    }

    public void OnIAPBuy200CoinsCompleted(Product product, int instanceId)
    {
        GameData.Instance.increaseCoin(200);
        Debug.Log("IAP Completed: " + instanceId);
        PopupManager.Instance.ShowNotification("Thanks for buying. You get 200 coins", coin, 2.5f);
    }

    public void OnIAPBuy500CoinsCompleted(Product product, int instanceId)
    {
        GameData.Instance.increaseCoin(500);
        Debug.Log("IAP Completed: " + instanceId);
        PopupManager.Instance.ShowNotification("Thanks for buying. You get 500 coins", coin, 2.5f);
    }

    public void OnIAPBuy1000CoinsCompleted(Product product, int instanceId)
    {
        GameData.Instance.increaseCoin(1000);
        Debug.Log("IAP Completed: " + instanceId);
        PopupManager.Instance.ShowNotification("Thanks for buying. You get 1000 coins", coin, 2.5f);
    }

    public void OnIAPBuy2000CoinsCompleted(Product product, int instanceId)
    {
        GameData.Instance.increaseCoin(2000);
        Debug.Log("IAP Completed: " + instanceId);
        PopupManager.Instance.ShowNotification("Thanks for buying. You get 2000 coins", coin, 2.5f);
    }

    public void OnIAPBuy5000CoinsCompleted(Product product, int instanceId)
    {
        GameData.Instance.increaseCoin(5000);
        Debug.Log("IAP Completed: " + instanceId);
        PopupManager.Instance.ShowNotification("Thanks for buying. You get 5000 coins", coin, 2.5f);
    }

    public void OnIAPBuy10000oinsCompleted(Product product, int instanceId)
    {
        GameData.Instance.increaseCoin(10000);
        Debug.Log("IAP Completed: " + instanceId);
        PopupManager.Instance.ShowNotification("Thanks for buying. You get 10000 coins", coin, 2.5f);
    }

    public void OnIAPFailed(Product product, string reason)
    {
        Debug.Log("IAP Failed: " + reason.ToString());
        if(reason != "Verify failed from server") PopupManager.Instance.ShowNotification("Buy failed. Please try again later!", error, 2f);
    }

}

