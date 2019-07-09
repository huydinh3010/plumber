using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    private void Awake()
    {
        sceneController.openScene();
        if(!GameCache.Instance.firstGameLoad && !GameData.Instance.clampDailyReward && GameData.Instance.continueDay > 0)
        {
            int i;
            for(i = 0; i < GameData.Instance.continueDay - 1; i++)
            {
                btnDays[i].GetComponent<Image>().sprite = btnDaysPassed[i];
            }
            btnDays[i].GetComponent<Image>().sprite = btnDaysActive[i];
            btnDays[i].enabled = true;
            panelDailyReward.GetComponent<Animator>().Play("Show");
        } 
    }

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
            
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

    }

    public void BtnRateOnClick()
    {
        panelRate.GetComponent<Animator>().Play("Show");
    }

    public void BtnDayOnPanelDailyRewardOnClick(int k)
    {
        if(k == GameData.Instance.continueDay - 1)
        {
            btnDays[k].GetComponent<Image>().sprite = btnDaysPassed[k];
            GameData.Instance.increaseCoin(rewards[k]);
            btnDays[k].enabled = false;
            GameData.Instance.clampDailyReward = true;
            StartCoroutine(WaitForClosePanel());
        }
    }

    private IEnumerator WaitForClosePanel()
    {
        yield return new WaitForSeconds(1);
        panelDailyReward.GetComponent<Animator>().Play("Close");
    }

    public void BtnPlayServicesOnClick()
    {
        panelPlayServices.GetComponent<Animator>().Play("Show");
    }

    public void BtnHelpOnClick()
    {
        GameCache.Instance.level_selected = 1;
        GameCache.Instance.mode = 0;
        sceneController.loadScene("GamePlay");
    }

    public void BtnMoreGameOnClick()
    {

    }

    public void BtnCloseOnPanelOnClick(GameObject panel)
    {
        panel.GetComponent<Animator>().Play("Close");
    }

    public void BtnRateOnPanelOnClick()
    {
        
    }

    

    private void OnDestroy()
    {
        
    }


}
