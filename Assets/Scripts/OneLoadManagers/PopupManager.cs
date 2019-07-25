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
                    activePopup = popupDailyReward;
                    break;
                case PopupName.PlayServices:
                    activePopup = popupPlayServices;
                    break;
                case PopupName.Rate:
                    activePopup = popupRate;
                    break;
                case PopupName.Achievement:
                    activePopup = popupAchievement;
                    break;
                case PopupName.AddCoin:
                    activePopup = popupAddCoin;
                    break;
                case PopupName.PassLevel:
                    activePopup = popupPassLevel;
                    break;
                case PopupName.NextLevel:
                    activePopup = popupNextLevel;
                    break;
                case PopupName.LastLevel:
                    activePopup = popupLastLevel;
                    break;
            }
            activePopup.Show(list_actions, list_settings);
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
    PlayServices,
    Rate,
    Achievement,
    AddCoin,
    PassLevel,
    NextLevel,
    LastLevel,
}

public enum PopupButtonEvent
{
    ClosePressed,
    DayOnDailyRewardPressed,
    NotNowOnRatePressed,
    RateOnRatePressed,
    CoinOnAchievementPressed,
    WatchVideoMoreCoinPressed,
    WatchVideo10TimesCoinPressed,
    ShareFacebookPressed,
    NextLevelPressed,
}

public enum PopupSettingType
{
    PassLevelImageType,
}

