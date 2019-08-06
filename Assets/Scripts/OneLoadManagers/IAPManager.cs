using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviour
{
    public static IAPManager Instance;

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
        noAdsCallback?.Invoke();
    }

    public void OnIAPBuy100CoinsCompleted(Product product, int instanceId)
    {
        GameData.Instance.increaseCoin(100);
        Debug.Log("IAP Completed: " + instanceId);
    }

    public void OnIAPBuy200CoinsCompleted(Product product, int instanceId)
    {
        GameData.Instance.increaseCoin(200);
        Debug.Log("IAP Completed: " + instanceId);
    }

    public void OnIAPBuy500CoinsCompleted(Product product, int instanceId)
    {
        GameData.Instance.increaseCoin(500);
        Debug.Log("IAP Completed: " + instanceId);
    }

    public void OnIAPBuy1000CoinsCompleted(Product product, int instanceId)
    {
        GameData.Instance.increaseCoin(1000);
        Debug.Log("IAP Completed: " + instanceId);
    }

    public void OnIAPBuy2000CoinsCompleted(Product product, int instanceId)
    {
        GameData.Instance.increaseCoin(2000);
        Debug.Log("IAP Completed: " + instanceId);
    }

    public void OnIAPBuy5000CoinsCompleted(Product product, int instanceId)
    {
        GameData.Instance.increaseCoin(5000);
        Debug.Log("IAP Completed: " + instanceId);
    }

    public void OnIAPBuy10000oinsCompleted(Product product, int instanceId)
    {
        GameData.Instance.increaseCoin(10000);
        Debug.Log("IAP Completed: " + instanceId);
    }

    public void OnIAPFailed(Product product, string reason)
    {
        Debug.Log("IAP Failed: " + reason.ToString());
    }

}

