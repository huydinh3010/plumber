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

    public void OnIAPNoAdsCompleted()
    {
        GameData.Instance.isAdsOn = false;
        noAdsCallback?.Invoke();
    }

    public void OnIAPBuyCoinsCompleted(int value)
    {
        GameData.Instance.increaseCoin(value);
    }
}

