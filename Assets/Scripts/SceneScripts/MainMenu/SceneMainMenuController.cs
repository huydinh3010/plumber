﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class SceneMainMenuController : MonoBehaviour
{
    private bool firstFrame;
    private void Awake()
    {
        EventDispatcher.Instance.RegisterListener(EventID.OnPointChange, OnPointChange);
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Time: " + DateTime.Now.TimeOfDay + "--Main Menu Start() start");
        LoadSceneManager.Instance.OpenScene();
        if (!GameCache.Instance.firstGameLoad && !GameData.Instance.dailyRewardStatus && GameData.Instance.continueDay > 0)
        {
            PopupManager.Instance.ShowPopup(PopupName.DailyReward, null);
        }
        GameCache.Instance.firstGameLoad = false;

        Debug.Log("Time: " + DateTime.Now.TimeOfDay + "--Main Menu Start() end");
    }

    // Update is called once per frame
    void Update()
    {
        if (!firstFrame)
        {
            Debug.Log("Time: " + DateTime.Now.TimeOfDay + "--Main Menu FirstFrame");
            firstFrame = true;
            
        }
    }

    private void OnPointChange(object param)
    {
        if (GameCache.Instance.unlockAchievementProgress < GameConfig.ACHIEVEMENT_CONDITION_POINT.Length && GameData.Instance.points >= GameConfig.ACHIEVEMENT_CONDITION_POINT[GameCache.Instance.unlockAchievementProgress])
        {
            PopupManager.Instance.ShowNotification("Unlock achievement. Go back Menu to get " + GameConfig.ACHIEVEMENT_COIN_REWARD[GameCache.Instance.unlockAchievementProgress] + " coins", null, 3f);
            GameCache.Instance.unlockAchievementProgress++;
        }
    }

    public void BtnPlayOnClick()
    {
        LoadSceneManager.Instance.LoadScene("SimpleLevel");
    }

    public void BtnDailyChallengeOnClick()
    {
        LoadSceneManager.Instance.LoadScene("ChallengeLevel");
    }

    public void BtnAchievementOnClick()
    {
        PopupManager.Instance.ShowPopup(PopupName.Achievement, null);
    }

    public void BtnRateOnClick()
    {
        PopupManager.Instance.ShowPopup(PopupName.Rate, null);
    }

    public void BtnPlayServicesOnClick()
    {
        PopupManager.Instance.ShowPopup(PopupName.PlayServices, null);
    }

    public void BtnHelpOnClick()
    {
        GameCache.Instance.levelSelected = 1;
        GameCache.Instance.mode = 0;
        LoadSceneManager.Instance.LoadScene("GamePlay");
    }

    public void BtnShopOnClick()
    {
        PopupManager.Instance.ShowPopup(PopupName.Shop, null);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveListener(EventID.OnPointChange, OnPointChange);
        try
        {
            PopupManager.Instance.ForceClosePopup();
        }
        catch (Exception e)
        {
            
        }
    }
}
