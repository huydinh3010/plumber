using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
public class SceneMainMenuController : MonoBehaviour
{
    //public Texture2D texture;
    public GameObject btnDailyChallenge;
    public GameObject btnRemoveAds;
    public GameObject btnMoreGame;
    public SceneController sceneController;

    //public GameObject panelRate;
    //public GameObject panelPlayServices;
    //public GameObject panelDailyReward;
    //public GameObject panelAchievement;
    //public RectTransform contentAchievement;

    //public Button[] btnDays;
    //public Sprite[] btnDaysActive;
    //public Sprite[] btnDaysPassed;
    //public Button[] btnAchievements;
    //public Image[] imageChecks;
    //private int[] rewards = { 10, 25, 50, 75, 100 };
    //private int[] achm_points = { 50, 100, 500, 1000, 2000, 5000, 10000, 20000, 50000, 100000 };
    //private int[] achm_coins_reward = { 10, 20, 50, 100, 200, 500, 1000, 2000, 5000, 10000 };

    //private bool panelShowing;
    private bool firstFrame;
    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Time: " + DateTime.Now.TimeOfDay + "--Main Menu Start() start");
        sceneController.openScene();
        //setupPanelDailyReward();
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

    //private void setupPanelDailyReward()
    //{
    //    if (!GameCache.Instance.firstGameLoad && !GameData.Instance.clampDailyReward && GameData.Instance.continueDay > 0)
    //    {
    //        int i;
    //        for (i = 0; i < GameData.Instance.continueDay - 1; i++)
    //        {
    //            btnDays[i].GetComponent<Image>().sprite = btnDaysPassed[i];
    //        }
    //        btnDays[i].GetComponent<Image>().sprite = btnDaysActive[i];
    //        btnDays[i].enabled = true;
    //        showPanel(panelDailyReward);
    //    }
    //}

    //private void setupPanelAchievement()
    //{
    //    float delta = contentAchievement.rect.height / achm_points.Length;
    //    contentAchievement.anchoredPosition = new Vector2(contentAchievement.anchoredPosition.x, delta * GameData.Instance.achievement_progress); 
    //    for(int i = 0; i < achm_points.Length; i++)
    //    {
    //        if (i + 1 <= GameData.Instance.achievement_progress)
    //        {
    //            btnAchievements[i].interactable = false;
    //            btnAchievements[i].GetComponent<CanvasGroup>().alpha = 1;
    //            imageChecks[i].enabled = true;
               
    //        }
    //        else 
    //        {
    //            if(GameData.Instance.points >= achm_points[i])
    //            {
    //                btnAchievements[i].interactable = true;
    //                btnAchievements[i].GetComponent<CanvasGroup>().alpha = 1;
    //                imageChecks[i].enabled = false;
    //            }
    //            else
    //            {
    //                btnAchievements[i].interactable = false;
    //                btnAchievements[i].GetComponent<CanvasGroup>().alpha = 0.5f;
    //                imageChecks[i].enabled = false;
    //            }
    //        }
    //    }
    //}

    public void BtnPlayOnClick()
    {
        sceneController.loadScene("SimpleLevel");
    }

    public void BtnDailyChallengeOnClick()
    {
        sceneController.loadScene("ChallengeLevel");
    }

    public void BtnAchievementOnClick()
    {
        //setupPanelAchievement();
        //showPanel(panelAchievement);
        PopupManager.Instance.ShowPopup(PopupName.Achievement, null);
    }

    public void BtnRemoveAdsOnClick()
    {
        IAPManager.Instance.BuyNoAds();
    }

    public void BtnRateOnClick()
    {
        //showPanel(panelRate);
        PopupManager.Instance.ShowPopup(PopupName.Rate, null);
    }

    //public void BtnDayOnPanelDailyRewardOnClick(int k)
    //{
    //    if (panelShowing)
    //    {
    //        if (k == GameData.Instance.continueDay - 1)
    //        {
    //            btnDays[k].GetComponent<Image>().sprite = btnDaysPassed[k];
    //            GameData.Instance.increaseCoin(rewards[k]);
    //            btnDays[k].enabled = false;
    //            GameData.Instance.clampDailyReward = true;
    //            StartCoroutine(WaitForClosePanel());
    //        }
    //    }
    //}

    //public void BtnCoinOnPanelAchievementOnClick(int k)
    //{
    //    Debug.Log("Onclick: k = " + k);
    //    if(panelShowing && k == GameData.Instance.achievement_progress)
    //    {
    //        GameData.Instance.increaseCoin(achm_coins_reward[k]);
    //        GameData.Instance.achievement_progress++;
    //        btnAchievements[k].interactable = false;
    //        imageChecks[k].enabled = true;
    //    }
    //}

    //private IEnumerator WaitForClosePanel()
    //{
    //    yield return new WaitForSeconds(0.5f);
    //    closePanel(panelDailyReward);
    //}

    //private void showPanel(GameObject panel)
    //{
    //    if (!panelShowing)
    //    {
    //        panel.GetComponent<Animator>().Play("Show");
    //        panelShowing = true;
    //    }
    //}

    //private void closePanel(GameObject panel)
    //{
    //    panel.GetComponent<Animator>().Play("Close");
    //    panelShowing = false;
    //}

    public void BtnPlayServicesOnClick()
    {
        //showPanel(panelPlayServices);
        PopupManager.Instance.ShowPopup(PopupName.PlayServices, null);
    }

    public void BtnHelpOnClick()
    {
        GameCache.Instance.level_selected = 1;
        GameCache.Instance.mode = 0;
        sceneController.loadScene("GamePlay");
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

    //public void BtnCloseOnPanelOnClick(GameObject panel)
    //{
    //    if(panelShowing) closePanel(panel);
    //}

//    public void BtnRateOnPanelOnClick()
//    {
//        if (panelShowing)
//        {
            
//            // miss id
//#if UNITY_ANDROID
//            Application.OpenURL("market://details?id=" + Application.productName);
//#elif UNITY_IPHONE
// Application.OpenURL("itms-apps://itunes.apple.com/app/idYOUR_ID");
//#endif
//            GameData.Instance.rate = false;
//        }
//    }

    private void OnDestroy()
    {
        
    }


}
