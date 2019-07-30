using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Purchasing;

public class PopupRemoveAds : MonoBehaviour, IPopup
{
    [SerializeField] Button btn_Close;
    [SerializeField] Button btn_Go;
    [SerializeField] Text content;
    private Action btn_Close_Callback;
    private Action btn_Go_Callback;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Setup()
    {
#if UNITY_ANDROID
        content.text = "Go to GooglePlay to pay";
#elif UNITY_IPHONE
        content.text = "Go to AppStore to pay";
#else
            content.text = "";
#endif
    }

    public void Close()
    {
        btn_Close.enabled = false;
        btn_Go.enabled = false;
        EventDispatcher.Instance.PostEvent(EventID.OnPopupClosed, this);
        GetComponent<Animator>().Play("Close");
    }

    public void OnClosed()
    {
        GetComponent<RectTransform>().gameObject.SetActive(false);
    }

    public void OnDisplayed()
    {
        btn_Close.enabled = true;
        btn_Go.enabled = true;
    }

    public void Show(Dictionary<PopupButtonEvent, Action> list_actions, Dictionary<PopupSettingType, object> list_settings)
    {
        Setup();
        btn_Close_Callback = list_actions.ContainsKey(PopupButtonEvent.ClosePressed) ? list_actions[PopupButtonEvent.ClosePressed] : null;
        btn_Go_Callback = list_actions.ContainsKey(PopupButtonEvent.GoOnRemoveAds) ? list_actions[PopupButtonEvent.GoOnRemoveAds] : null;
        GetComponent<Animator>().Play("Show");
    }

    public void BtnCloseOnClick()
    {
        Close();
        btn_Close_Callback?.Invoke();
    }

    public void BtnGoOnClick()
    {
        //IAPManager.Instance.BuyNoAds();
    }

    public void OnPurchaseComplete(Product product, int instanceId)
    {
        GameData.Instance.isAdsOn = false;
        Close();
        Debug.Log("Complete: ");
        btn_Go_Callback?.Invoke();
    }

    //public void OnPurchaseComplete()
    //{
    //    GameData.Instance.ads_on = false;
    //    Close();
    //    Debug.Log("Complete: ");
    //    btn_Go_Callback?.Invoke();
    //}

    public void OnPurchaseFailed(Product product, string reason)
    {
        Close();
        Debug.Log("Failed: " + reason);
        btn_Go_Callback?.Invoke();
    }

    //public void OnPurchaseFailed()
    //{
    //    Close();
    //    btn_Go_Callback?.Invoke();
    //}
}
