using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
public class PopupShop : MonoBehaviour, IPopup
{
    [SerializeField] RectTransform content;
    [SerializeField] Button btn_Buy_NoAds;
    [SerializeField] Button[] btn_Buy_Coins;
    [SerializeField] Text[] text_Coins;
    private Action btn_Close_Callback;
    private Action btn_Buy_Callback;
    private bool isShow;
    void Awake()
    {
        IAPManager.Instance.RegisterNoAdsCallback(() => { btn_Buy_NoAds.interactable = false; });
        btn_Buy_NoAds.GetComponentInChildren<Text>().text = "$" + GameConfig.SHOP_PRICE[0].ToString();
        for (int i = 0; i < btn_Buy_Coins.Length; i++)
        {
            text_Coins[i].text = "+" + GameConfig.SHOP_COIN[i + 1].ToString();
            btn_Buy_Coins[i].GetComponentInChildren<Text>().text = "$" + GameConfig.SHOP_PRICE[i + 1].ToString();
        }
    }

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
        isShow = false;
        content.anchoredPosition = Vector2.zero;
        btn_Buy_NoAds.interactable = GameData.Instance.isAdsOn;
    }

    public void Close()
    {
        isShow = false;
        EventDispatcher.Instance.PostEvent(EventID.OnPopupClosed, this);
        GetComponent<Animator>().Play("Close");
    }

    public void OnClosed()
    {
        GetComponent<RectTransform>().gameObject.SetActive(false);
    }

    public void OnDisplayed()
    {
        isShow = true;
    }

    public void Show(Dictionary<PopupButtonEvent, Action> list_actions, Dictionary<PopupSettingType, object> list_settings)
    {
        Setup();
        btn_Close_Callback = list_actions.ContainsKey(PopupButtonEvent.ClosePressed) ? list_actions[PopupButtonEvent.ClosePressed] : null;
        btn_Buy_Callback = list_actions.ContainsKey(PopupButtonEvent.BuyOnShopPressed) ? list_actions[PopupButtonEvent.BuyOnShopPressed] : null;
        GetComponent<Animator>().Play("Show");
    }

    public void BtnBuyNoAdsOnClick()
    {
        if (isShow)
        {
            btn_Buy_Callback?.Invoke();
            CodelessIAPStoreListener.Instance.InitiatePurchase("com.waterline.removeads");
        }
    }

    public void BtnBuyCoinOnClick(int coins)
    {
        if(isShow)
        {
            btn_Buy_Callback?.Invoke();
            CodelessIAPStoreListener.Instance.InitiatePurchase("com.waterline.coins" + coins);
        }
    }

    public void BtnCloseOnClick()
    {
        if (isShow)
        {
            AudioManager.Instance.Play(AudioManager.SoundName.BUTTON);
            Close();
            btn_Close_Callback?.Invoke();
        }
    }
}
