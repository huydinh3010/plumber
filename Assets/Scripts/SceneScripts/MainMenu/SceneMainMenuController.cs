using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class SceneMainMenuController : MonoBehaviour
{
    [SerializeField] Sprite achievement;
    private bool firstFrame;
    private void Awake()
    {
        EventDispatcher.Instance.RegisterListener(EventID.OnPointChange, OnPointChange);
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadSceneManager.Instance.OpenScene();
        if (GameCache.Instance.showAchievement)
        {
            GameCache.Instance.showAchievement = false;
            PopupManager.Instance.ShowPopup(PopupName.Achievement, null);
        }
        else if (!GameCache.Instance.firstGameLoad && !GameData.Instance.dailyRewardStatus && GameData.Instance.continueDay > 0)
        {
            PopupManager.Instance.ShowPopup(PopupName.DailyReward, null);
        }
        GameCache.Instance.firstGameLoad = false;
    }



    // Update is called once per frame
    void Update()
    {

    }

    private void OnPointChange(object param)
    {
        if (GameCache.Instance.unlockAchievementProgress < GameConfig.ACHIEVEMENT_CONDITION_POINT.Length && GameData.Instance.points >= GameConfig.ACHIEVEMENT_CONDITION_POINT[GameCache.Instance.unlockAchievementProgress])
        {
            PopupManager.Instance.ShowNotification("Unlock achievement. Touch to go back Menu to get " + GameConfig.ACHIEVEMENT_COIN_REWARD[GameCache.Instance.unlockAchievementProgress] + " coins", achievement, 3f, ()=>{
                PopupManager.Instance.ShowPopup(PopupName.Achievement, null);
            });
            GameCache.Instance.unlockAchievementProgress++;
        }
    }

    public void BtnPlayOnClick()
    {
        AudioManager.Instance.Play("button_sound");
        LoadSceneManager.Instance.LoadScene("SimpleLevel");
    }

    public void BtnDailyChallengeOnClick()
    {
        AudioManager.Instance.Play("button_sound");
        LoadSceneManager.Instance.LoadScene("ChallengeLevel");
    }

    public void BtnAchievementOnClick()
    {
        AudioManager.Instance.Play("button_sound");
        PopupManager.Instance.ShowPopup(PopupName.Achievement, null);
    }

    public void BtnRateOnClick()
    {
        AudioManager.Instance.Play("button_sound");
        PopupManager.Instance.ShowPopup(PopupName.Rate, null);
    }

    public void BtnPlayServicesOnClick()
    {
        AudioManager.Instance.Play("button_sound");
        PopupManager.Instance.ShowPopup(PopupName.PlayServices, null);
    }

    public void BtnHelpOnClick()
    {
        AudioManager.Instance.Play("button_sound");
        GameCache.Instance.levelSelected = 1;
        GameCache.Instance.mode = 0;
        LoadSceneManager.Instance.LoadScene("GamePlay");
    }

    public void BtnShopOnClick()
    {
        AudioManager.Instance.Play("button_sound");
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
