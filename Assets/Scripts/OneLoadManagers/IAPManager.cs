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
        Debug.Log("IAP noads completed");
        noAdsCallback?.Invoke();
    }

    public void OnIAPBuyCoinsCompleted(Product product, int instanceId)
    {
        //GameData.Instance.increaseCoin(GameConfig.SHOP_COIN[1]);
        Debug.Log("IAP Completed: " + instanceId);
    }

    public void OnIAPFailed(Product product, string reason)
    {
        Debug.Log("IAP Failed: " + reason.ToString());
    }

}

