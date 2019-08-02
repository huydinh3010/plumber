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
    [SerializeField] Button btn_Close;

    private Action btn_Close_Callback;
    private Action btn_Buy_Callback;

    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        IAPManager.Instance.RegisterNoAdsCallback(() => { btn_Buy_NoAds.interactable = false; });
        btn_Buy_NoAds.GetComponentInChildren<Text>().text = "$" + GameConfig.SHOP_PRICE[0].ToString();
        for (int i = 0; i < btn_Buy_Coins.Length; i++)
        {
            text_Coins[i].text = "+" + GameConfig.SHOP_COIN[i + 1].ToString();
            btn_Buy_Coins[i].GetComponentInChildren<Text>().text = "$" + GameConfig.SHOP_PRICE[i+1].ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Setup()
    {
        btn_Close.enabled = false;
        btn_Buy_NoAds.enabled = false;
        btn_Buy_NoAds.interactable = GameData.Instance.isAdsOn;
        foreach (Button button in btn_Buy_Coins)
        {
            button.enabled = false;
        }
    }

    public void Close()
    {
        btn_Close.enabled = false;
        btn_Buy_NoAds.enabled = false;
        foreach (Button button in btn_Buy_Coins)
        {
            button.enabled = false;
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
        btn_Close.enabled = true;
        btn_Buy_NoAds.enabled = true;
        foreach (Button button in btn_Buy_Coins)
        {
            button.enabled = true;
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
        btn_Buy_Callback?.Invoke();
    }

    public void BtnCloseOnClick()
    {
        Close();
        btn_Close_Callback?.Invoke();
    }
}
