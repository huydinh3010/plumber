using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
public class SceneMainMenuController : MonoBehaviour
{
    public GameObject btnDailyChallenge;
    public GameObject btnRemoveAds;
    public GameObject btnMoreGame;
    private bool firstFrame;
    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Time: " + DateTime.Now.TimeOfDay + "--Main Menu Start() start");
        LoadSceneManager.Instance.OpenScene();
        if (!GameCache.Instance.firstGameLoad && !GameData.Instance.clampDailyReward && GameData.Instance.continueDay > 0)
        {
            PopupManager.Instance.ShowPopup(PopupName.DailyReward, null);
        }
        
        btnRemoveAds.SetActive(GameData.Instance.ads_on);
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
        IAPManager.Instance.BuyNoAds();
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
        GameCache.Instance.level_selected = 1;
        GameCache.Instance.mode = 0;
        LoadSceneManager.Instance.LoadScene("GamePlay");
    }

    public void BtnMoreGameOnClick()
    {
        // miss id
#if UNITY_ANDROID
        Application.OpenURL("market://details?id=YOUR_ID");
#elif UNITY_IPHONE
 Application.OpenURL("itms-apps://itunes.apple.com/app/idYOUR_ID");
#endif
    }

    private void OnDestroy()
    {
        PopupManager.Instance.ClosePopup();
    }


}
