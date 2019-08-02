using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    [SerializeField] PopupDailyReward popupDailyReward;
    [SerializeField] PopupPlayServices popupPlayServices;
    [SerializeField] PopupRate popupRate;
    [SerializeField] PopupAchievement popupAchievement;
    [SerializeField] PopupAddCoin popupAddCoin;
    [SerializeField] PopupPassLevel popupPassLevel;
    [SerializeField] PopupNextLevel popupNextLevel;
    [SerializeField] PopupLastLevel popupLastLevel;
    [SerializeField] PopupRemoveAds popupRemoveAds;
    [SerializeField] PopupShop popupShop;

    [SerializeField] GameObject go_DailyReward;
    [SerializeField] GameObject go_PlayServices;
    [SerializeField] GameObject go_Rate;
    [SerializeField] GameObject go_Achievement;
    [SerializeField] GameObject go_AddCoin;
    [SerializeField] GameObject go_PassLevel;
    [SerializeField] GameObject go_NextLevel;
    [SerializeField] GameObject go_LastLevel;
    [SerializeField] GameObject go_RemoveAds;
    [SerializeField] GameObject go_Shop;
    private IPopup activePopup;
    private bool showing;

    public bool Showing
    {
        get
        {
            return showing;
        }
    }

    public IPopup ActivePopup
    {
        get
        {
            return activePopup;
        }
    }

    private void Awake()
    {
        Instance = Instance == null ? this : Instance;
        EventDispatcher.Instance.RegisterListener(EventID.OnPopupClosed, OnPopupClosed);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnPopupClosed(object param)
    {
        showing = false;
        activePopup = null;

    }

    public void ShowPopup(PopupName name, Dictionary<PopupButtonEvent, Action> list_actions, Dictionary<PopupSettingType, object> list_settings = null)
    {
        if (!showing)
        {
            showing = true;

            if (list_actions == null) list_actions = new Dictionary<PopupButtonEvent, Action>();
            switch (name)
            {
                case PopupName.DailyReward:
                    go_DailyReward.SetActive(true);
                    activePopup = popupDailyReward;
                    break;
                case PopupName.RemoveAds:
                    go_RemoveAds.SetActive(true);
                    activePopup = popupRemoveAds;
                    break;
                case PopupName.PlayServices:
                    go_PlayServices.SetActive(true);
                    activePopup = popupPlayServices;
                    break;
                case PopupName.Rate:
                    go_Rate.SetActive(true);
                    activePopup = popupRate;
                    break;
                case PopupName.Achievement:
                    go_Achievement.SetActive(true);
                    activePopup = popupAchievement;
                    break;
                case PopupName.AddCoin:
                    go_AddCoin.SetActive(true);
                    activePopup = popupAddCoin;
                    break;
                case PopupName.PassLevel:
                    go_PassLevel.SetActive(true);
                    activePopup = popupPassLevel;
                    break;
                case PopupName.NextLevel:
                    go_NextLevel.SetActive(true);
                    activePopup = popupNextLevel;
                    break;
                case PopupName.LastLevel:
                    go_LastLevel.SetActive(true);
                    activePopup = popupLastLevel;
                    break;
                case PopupName.Shop:
                    go_Shop.SetActive(true);
                    activePopup = popupShop;
                    break;
            }
            activePopup.Show(list_actions, list_settings);
        }
    }

    public void ClosePopup()
    {
        if (activePopup != null)
        {
            activePopup.Close();
        }
    }

    public void ForceClosePopup()
    {
        if(activePopup != null)
        {
            activePopup.OnClosed();
        }
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveListener(EventID.OnPopupClosed, OnPopupClosed);
    }
}


public enum PopupName
{
    DailyReward,
    RemoveAds,
    PlayServices,
    Rate,
    Achievement,
    AddCoin,
    PassLevel,
    NextLevel,
    LastLevel,
    Shop,
}

public enum PopupButtonEvent
{
    ClosePressed,
    DayOnDailyRewardPressed,
    GoOnRemoveAds,
    NotNowOnRatePressed,
    RateOnRatePressed,
    CoinOnAchievementPressed,
    WatchVideoMoreCoinPressed,
    WatchVideo10TimesCoinPressed,
    ShareFacebookPressed,
    NextLevelPressed,
    BuyOnShopPressed,
}

public enum PopupSettingType
{
    PassLevelImageType,
}

