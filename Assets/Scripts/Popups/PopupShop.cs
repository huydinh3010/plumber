using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupShop : MonoBehaviour, IPopup
{
    [SerializeField] Button btn_Buy_NoAds;
    [SerializeField] Button[] btn_Buy_Coins;
    [SerializeField] Text[] text_Coins;
    private ImoSysSDK.Purchasing.IMOIAPButton[] iapButtons;
    private Action btn_Close_Callback;
    private Action btn_Buy_Callback;
    private bool isShow;
    void Awake()
    {
        IAPManager.Instance.RegisterNoAdsCallback(() => { btn_Buy_NoAds.interactable = false; });
        btn_Buy_NoAds.GetComponentInChildren<Text>().text = "$" + GameConfig.SHOP_PRICE[0].ToString();
        iapButtons = new ImoSysSDK.Purchasing.IMOIAPButton[GameConfig.SHOP_PRICE.Length];
        iapButtons[0] = btn_Buy_NoAds.GetComponent<ImoSysSDK.Purchasing.IMOIAPButton>();
        for (int i = 0; i < btn_Buy_Coins.Length; i++)
        {
            text_Coins[i].text = "+" + GameConfig.SHOP_COIN[i + 1].ToString();
            btn_Buy_Coins[i].GetComponentInChildren<Text>().text = "$" + GameConfig.SHOP_PRICE[i + 1].ToString();
            iapButtons[i+1] = btn_Buy_Coins[i].GetComponent<ImoSysSDK.Purchasing.IMOIAPButton>();
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
        for (int i = 0; i < iapButtons.Length; i++)
        {
            iapButtons[i].enabled = false;
        }
        btn_Buy_NoAds.interactable = GameData.Instance.isAdsOn;
    }

    public void Close()
    {
        isShow = false;
        for (int i = 0; i < iapButtons.Length; i++)
        {
            iapButtons[i].enabled = false;
        }
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
        for (int i = 0; i < iapButtons.Length; i++)
        {
            iapButtons[i].enabled = true;
        }
    }

    public void Show(Dictionary<PopupButtonEvent, Action> list_actions, Dictionary<PopupSettingType, object> list_settings)
    {
        Setup();
        btn_Close_Callback = list_actions.ContainsKey(PopupButtonEvent.ClosePressed) ? list_actions[PopupButtonEvent.ClosePressed] : null;
        btn_Buy_Callback = list_actions.ContainsKey(PopupButtonEvent.BuyOnShopPressed) ? list_actions[PopupButtonEvent.BuyOnShopPressed] : null;
        GetComponent<Animator>().Play("Show");
    }

    public void BtnBuyOnClick()
    {
        if (isShow)
        {
            btn_Buy_Callback?.Invoke();
        }
    }

    public void BtnCloseOnClick()
    {
        if (isShow)
        {
            Close();
            btn_Close_Callback?.Invoke();
        }
    }
}
