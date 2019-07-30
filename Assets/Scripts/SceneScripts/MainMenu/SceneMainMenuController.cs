using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class SceneMainMenuController : MonoBehaviour
{
    [SerializeField] GameObject btnDailyChallenge;
    [SerializeField] GameObject btnRemoveAds;
    private bool firstFrame;
    private void Awake()
    {

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

        btnRemoveAds.SetActive(GameData.Instance.isAdsOn);

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

    public void BtnRemoveAdsOnClick()
    {
        //IAPManager.Instance.RegisterNoAdsCallback(()=> { btnRemoveAds.SetActive(false); });

        PopupManager.Instance.ShowPopup(PopupName.RemoveAds, new Dictionary<PopupButtonEvent, Action>() { { PopupButtonEvent.GoOnRemoveAds, OnAdsRemoved } });
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

    private void OnAdsRemoved()
    {
        btnRemoveAds.SetActive(GameData.Instance.isAdsOn);
    }

    private void OnDestroy()
    {
        PopupManager.Instance.ForceClosePopup();
    }
}
