using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class SceneSimpleLevelController : MonoBehaviour
{
    public SceneController sceneController;
    public GameObject Grid;
    public GameObject OneStarLv;
    public GameObject TwoStarsLv;
    public GameObject ThreeStarsLv;
    public GameObject UnlockLv;
    public GameObject LockLv;
    public GameObject ContentObj;
    //public GameObject panelAddCoin;
    //public GameObject panelLastLevel;
    public Text txtCoins;

    //private bool panelShowing;

    private void Awake()
    {
        sceneController.openScene();
        txtCoins.text = GameData.Instance.coins.ToString();
        for(int p = 0; p < 35; p++)
        {
            GameObject goGridClone = Instantiate(Grid, Vector3.zero, Quaternion.identity, ContentObj.transform);
            goGridClone.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(-18700 + 1100 * p, 0, 0);
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    GameObject go = null;
                    if (p * 16 + i * 4 + j < GameData.Instance.level_stars.Count)
                    {
                        int level = p * 16 + i * 4 + j + 1;
                        switch (GameData.Instance.level_stars[p * 16 + i * 4 + j])
                        {
                            case 0:
                                go = Instantiate(UnlockLv, new Vector3(), Quaternion.identity, goGridClone.transform);
                                break;
                            case 1:
                                go = Instantiate(OneStarLv, new Vector3(), Quaternion.identity, goGridClone.transform);
                                break;
                            case 2:
                                go = Instantiate(TwoStarsLv, new Vector3(), Quaternion.identity, goGridClone.transform);
                                break;
                            case 3:
                                go = Instantiate(ThreeStarsLv, new Vector3(), Quaternion.identity, goGridClone.transform);
                                break;
                            default:
                                break;
                        }
                        go.GetComponentInChildren<Text>().text = level.ToString();
                        go.GetComponent<Button>().onClick.AddListener(() => { BtnLevelOnScrollViewOnClick(level); });
                    }
                    else
                    {
                        go = Instantiate(LockLv, new Vector3(), Quaternion.identity, goGridClone.transform);
                    }
                    go.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
                }
            }
        }
        if (GameCache.Instance.lastLevel)
        {
            //showPanel(panelLastLevel);
            PopupManager.Instance.ShowPopup(PopupName.LastLevel, null);
            GameCache.Instance.lastLevel = false;
        }
        EventDispatcher.Instance.RegisterListener(EventID.OnCoinChange, OnCoinChange);
    }

    private void OnCoinChange(object param)
    {
        StartCoroutine(coinChangeEffect(txtCoins, Convert.ToInt32(param)));
    }

    IEnumerator coinChangeEffect(Text text, int value)
    {
        int frame = 10;
        int delta = (Mathf.Abs(value) / frame) + 1;
        int text_value = int.Parse(text.text);
        if (value > 0)
        {
            while (value > 0)
            {
                value -= delta;
                if (value < 0) text_value += delta + value;
                else text_value += delta;
                text.text = (text_value).ToString();
                yield return 1;
            }
        }
        else
        {
            while (value < 0)
            {
                value += delta;
                if (value > 0) text_value = text_value - delta + value;
                else text_value -= delta;
                text.text = (text_value).ToString();
                yield return 1;
            }
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



    //private void showPanel(GameObject panel)
    //{
    //    panel.GetComponent<Animator>().Play("Show");
    //    panelShowing = true;
    //}

    //private void closePanel(GameObject panel)
    //{
    //    panel.GetComponent<Animator>().Play("Close");
    //    panelShowing = false;
    //}

    public void BtnBackOnClick()
    {
        sceneController.loadScene("MainMenu");
    }

    public void BtnLevelOnScrollViewOnClick(int level)
    {
        GameCache.Instance.level_selected = level;
        GameCache.Instance.mode = 1;
        sceneController.loadScene("GamePlay");
    }

    public void BtnAddCoinOnClick()
    {
        //showPanel(panelAddCoin);
        PopupManager.Instance.ShowPopup(PopupName.AddCoin, null);
    }
    
    //public void BtnCloseOnPanelOnClick(GameObject target)
    //{
    //    if (panelShowing) closePanel(target);
    //}

    //public void BtnWatchVideoOnPanelOnClick()
    //{
    //    if (panelShowing)
    //    {
    //        bool hasVideo = AdManager.Instance.ShowRewardVideo(() =>
    //        {
    //            GameData.Instance.increaseCoin(10);
    //        });
    //        FirebaseManager.Instance.LogEventRequestRewardedVideo("10_coins", hasVideo, GameCache.Instance.level_selected);
    //        FacebookManager.Instance.LogEventRequestRewardedVideo("10_coins", hasVideo, GameCache.Instance.level_selected);
    //    }
    //}

    //public void BtnShareFbOnPanelOnClick()
    //{
    //    if (panelShowing)
    //    {
    //        FacebookManager.Instance.ShareWithFriends(() => {
    //            GameData.Instance.increaseCoin(50);
    //            FirebaseManager.Instance.LogEventShareFacebook();
    //            FacebookManager.Instance.LogEventShareFacebook();
    //        });
    //    }
    //}
    

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveListener(EventID.OnCoinChange, OnCoinChange);
    }
}
