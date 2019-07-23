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

    public GameObject panelRate;
    public GameObject panelPlayServices;
    public GameObject panelDailyReward;

    public Button[] btnDays;
    public Sprite[] btnDaysActive;
    public Sprite[] btnDaysPassed;
    private int[] rewards = { 10, 25, 50, 75, 100 };
    private bool panelShowing;
    private bool firstFrame;
    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Time: " + DateTime.Now.TimeOfDay + "--Main Menu Start() start");
        sceneController.openScene();
        if (!GameCache.Instance.firstGameLoad && !GameData.Instance.clampDailyReward && GameData.Instance.continueDay > 0)
        {
            int i;
            for (i = 0; i < GameData.Instance.continueDay - 1; i++)
            {
                btnDays[i].GetComponent<Image>().sprite = btnDaysPassed[i];
            }
            btnDays[i].GetComponent<Image>().sprite = btnDaysActive[i];
            btnDays[i].enabled = true;
            showPanel(panelDailyReward);
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
        sceneController.loadScene("SimpleLevel");
    }

    public void BtnDailyChallengeOnClick()
    {
        sceneController.loadScene("ChallengeLevel");
    }

    public void BtnRemoveAdsOnClick()
    {
        IAPManager.Instance.BuyNoAds();
    }

    public void BtnRateOnClick()
    {
        showPanel(panelRate);
    }

    public void BtnDayOnPanelDailyRewardOnClick(int k)
    {
        if (panelShowing)
        {
            if (k == GameData.Instance.continueDay - 1)
            {
                btnDays[k].GetComponent<Image>().sprite = btnDaysPassed[k];
                GameData.Instance.increaseCoin(rewards[k]);
                btnDays[k].enabled = false;
                GameData.Instance.clampDailyReward = true;
                StartCoroutine(WaitForClosePanel());
            }
        }
    }

    private IEnumerator WaitForClosePanel()
    {
        yield return new WaitForSeconds(0.5f);
        closePanel(panelDailyReward);
    }

    private void showPanel(GameObject panel)
    {
        if (!panelShowing)
        {
            panel.GetComponent<Animator>().Play("Show");
            panelShowing = true;
        }
    }

    private void closePanel(GameObject panel)
    {
        panel.GetComponent<Animator>().Play("Close");
        panelShowing = false;
    }

    public void BtnPlayServicesOnClick()
    {
        showPanel(panelPlayServices);
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

    public void BtnCloseOnPanelOnClick(GameObject panel)
    {
        if(panelShowing) closePanel(panel);
    }

    public void BtnRateOnPanelOnClick()
    {
        if (panelShowing)
        {
            GameData.Instance.rate = true;
            // miss id
#if UNITY_ANDROID
            Application.OpenURL("market://details?id=" + Application.productName);
#elif UNITY_IPHONE
 Application.OpenURL("itms-apps://itunes.apple.com/app/idYOUR_ID");
#endif
        }
    }

    private void OnDestroy()
    {
        
    }


}
