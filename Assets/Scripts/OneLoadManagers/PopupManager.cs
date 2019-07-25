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

    private bool showing;
    

    private void Awake()
    {
        Instance = Instance == null ? this : Instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowPopup(PopupName name, Dictionary<PopupButtonName, Action> list_actions, Dictionary<PopupSettingType, object> list_settings = null)
    {
        switch (name)
        {
            case PopupName.DailyReward:
                popupDailyReward.Show(list_actions);
                break;
            case PopupName.PlayServices:
                popupPlayServices.Show(list_actions);
                break;
            case PopupName.Rate:
                popupRate.Show(list_actions);
                break;
            case PopupName.Achievement:
                popupAchievement.Show(list_actions);
                break;
            case PopupName.AddCoin:
                popupAddCoin.Show(list_actions);
                break;
            case PopupName.PassLevel:
                popupPassLevel.Show(list_actions, list_settings);
                break;
            case PopupName.NextLevel:
                popupNextLevel.Show(list_actions);
                break;
            case PopupName.LastLevel:
                popupLastLevel.Show(list_actions);
                break;
        }
    }
    
    public void ClosePopup(PopupName name)
    {
        switch (name)
        {
            case PopupName.DailyReward:
                popupDailyReward.Close();
                break;
            case PopupName.PlayServices:
                popupPlayServices.Close();
                break;
            case PopupName.Rate:
                popupRate.Close();
                break;
            case PopupName.Achievement:
                popupAchievement.Close();
                break;
            case PopupName.AddCoin:
                popupAddCoin.Close();
                break;
            case PopupName.PassLevel:
                popupPassLevel.Close();
                break;
            case PopupName.NextLevel:
                popupNextLevel.Close();
                break;
            case PopupName.LastLevel:
                popupLastLevel.Close();
                break;
        }
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

public enum PopupButtonName
{
    Close,
    DayOnDailyReward,
    RateOnRate,
    NotNowOnRate,
    CoinOnAchievement,
    WatchVideoMoreCoin,
    WatchVideo10TimesCoin,
    ShareFacebook,
    NextLevel,
}

public enum PopupSettingType
{
    PassLevelImage,
}