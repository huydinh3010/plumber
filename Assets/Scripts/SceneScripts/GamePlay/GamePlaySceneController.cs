using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GamePlaySceneController : MonoBehaviour
{
    public SceneController sceneController;
    public GameController game;
    //public GameObject panelRate;
    //public GameObject panelAddCoin;
    //public GameObject panelPassedLevel;
    //public GameObject panelNextLevel;
    public Button btnSound;
    public Button btnRemove;
    public Button btnConstruct;
    public Button btnAddCoin;
    //public Button btnWatchVideo10TimesCoin;
    //public Image coinReward;
    //public Image pointReward;
    public Text txtCoins;
    public Text txtPoints;
    public Text txtLevel;
    //public Sprite[] s_coins;
    //public Sprite[] s_points;
    public Sprite[] s_sounds;
    private bool gameover;
    private bool animPlaying;
    private bool tutorial;
    private bool en_rmbtn;
    private bool en_construct_btn;
    //private bool panelShowing;
    private void Awake()
    {

        sceneController.openScene();
        EventDispatcher.Instance.RegisterListener(EventID.OnCoinChange, onCoinChange);
        EventDispatcher.Instance.RegisterListener(EventID.OnPointChange, onPointChange);
        EventDispatcher.Instance.RegisterListener(EventID.OnLevelSelectChange, onLevelSelectChange);
        EventDispatcher.Instance.RegisterListener(EventID.PipeAnimationEnd, endGame);
        EventDispatcher.Instance.RegisterListener(EventID.PipeAnimationStart, AnimationStart);

        if (GameCache.Instance.mode == 0 || (GameCache.Instance.level_selected == 1 && GameCache.Instance.mode == 1)) // help
        {
            newGameLevel(0);
        }
        else if (GameCache.Instance.mode == 1) // simple
        {
            newGameLevel(1);
        }
        else if (GameCache.Instance.mode == 2) // challenge
        {
            newGameLevel(2);
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

    private void newGameLevel(int type)
    {
        // UI
        if(type == 0)
        {
            game = GetComponent<HelpLevelController>();
            tutorial = true;
        }
        else if(type == 1)
        {
            game = GetComponent<SimpleModeController>();
            tutorial = false;
        }
        else
        {
            game = GetComponent<ChallengeModeController>();
            tutorial = false;
        }
        btnAddCoin.interactable = true;
        //btnRemove.interactable = GameData.Instance.coins >= 50;
        //btnConstruct.interactable = GameData.Instance.coins >= 25;
        if (GameData.Instance.sound_on) btnSound.GetComponent<Image>().sprite = s_sounds[1];
        else btnSound.GetComponent<Image>().sprite = s_sounds[0];
        txtCoins.text = GameData.Instance.coins.ToString();
        txtPoints.text = GameData.Instance.points.ToString();
        txtLevel.text = "Level " + GameCache.Instance.level_selected.ToString();
        //Gamelogic
        gameover = animPlaying = false;
        game.loadLevelData();
        game.setupLevel();
        en_rmbtn = game.remove_pipe_count == 0;
        en_construct_btn = !game.endConstructPipe;
        btnRemove.interactable = en_rmbtn && GameData.Instance.coins >= 50;
        btnConstruct.interactable = en_construct_btn && GameData.Instance.coins >= 25;
        string[] str_type = { "tutorial", "simple","daily_challenge" };
        FirebaseManager.Instance.LogEventLevelStart(GameCache.Instance.level_selected, str_type[type], GameData.Instance.day);
        FacebookManager.Instance.LogEventLevelStart(GameCache.Instance.level_selected, str_type[type], GameData.Instance.day);
    }


    public void AnimationStart(object param)
    {
        animPlaying = true;
        btnConstruct.interactable = false;
        btnRemove.interactable = false;
        btnAddCoin.interactable = false;
    }

    private void endGame(object param)
    {
        gameover = true;
        bool unlock_level = false;
        if (GameCache.Instance.mode == 0)
        {
            sceneController.loadScene("MainMenu");
        }
        else if (GameCache.Instance.mode == 1)
        {
            if (GameData.Instance.level_stars[GameCache.Instance.level_selected - 1] > 0)
            {
                int c_star = GameData.Instance.level_stars[GameCache.Instance.level_selected - 1];
                int n_star = game.getStar();
                if (n_star > c_star) GameData.Instance.level_stars[GameCache.Instance.level_selected - 1] = n_star;
                //showPanel(panelNextLevel);
                PopupManager.Instance.ShowPopup(PopupName.NextLevel, new Dictionary<PopupButtonEvent, Action>() { { PopupButtonEvent.ClosePressed, CloseLevelPopupCallback }, { PopupButtonEvent.NextLevelPressed, NextLevelCallback } });
            }
            else
            {
                int star = game.getStar();
                GameData.Instance.level_stars[GameCache.Instance.level_selected - 1] = star;
                GameData.Instance.increaseCoin(star);
                GameData.Instance.increasePoint(star * 10);
                //coinReward.sprite = s_coins[star - 1];
                //pointReward.sprite = s_points[star - 1];
                if (GameData.Instance.unlock_level < 560)
                {
                    GameData.Instance.level_stars.Add(0);
                    GameData.Instance.unlock_level++;
                    unlock_level = true;
                }
                GameData.Instance.unlocklv_state.newLevel();
                //btnWatchVideo10TimesCoin.interactable = true;
                //showPanel(panelPassedLevel);
                PopupManager.Instance.ShowPopup(PopupName.PassLevel, new Dictionary<PopupButtonEvent, Action>() { { PopupButtonEvent.ClosePressed, CloseLevelPopupCallback }, { PopupButtonEvent.NextLevelPressed, NextLevelCallback } }, new Dictionary<PopupSettingType, object>() { { PopupSettingType.PassLevelImageType, star } });
            }
            
        }
        else if (GameCache.Instance.mode == 2)
        {
            if (GameData.Instance.completed[GameCache.Instance.level_selected - 1] == 1)
            {
                //showPanel(panelNextLevel);
                PopupManager.Instance.ShowPopup(PopupName.NextLevel, new Dictionary<PopupButtonEvent, Action>() { { PopupButtonEvent.ClosePressed, CloseLevelPopupCallback }, { PopupButtonEvent.NextLevelPressed, NextLevelCallback } });
            }
            else
            {
                int star = game.getStar();
                GameData.Instance.completed[GameCache.Instance.level_selected - 1] = 1;
                GameData.Instance.increaseCoin(star);
                GameData.Instance.increasePoint(star * 10);
                //coinReward.sprite = s_coins[star - 1];
                //pointReward.sprite = s_points[star - 1];
                //showPanel(panelPassedLevel);
                PopupManager.Instance.ShowPopup(PopupName.PassLevel, new Dictionary<PopupButtonEvent, Action>() { { PopupButtonEvent.ClosePressed, CloseLevelPopupCallback }, { PopupButtonEvent.NextLevelPressed, NextLevelCallback } }, new Dictionary<PopupSettingType, object>() { { PopupSettingType.PassLevelImageType, star} });
                //btnWatchVideo10TimesCoin.interactable = true;
            }
        }
        string type = tutorial ? "tutorial" : (GameCache.Instance.mode == 1 ? "simple" : "daily_challenge");
        int remove_pipe_count = game.remove_pipe_count;
        int construct_pipe_count = game.construct_pipe_count;
        Debug.Log("Duration: " + game.duration_secs);
        Debug.Log("Turn: " + game.turn_count);
        FirebaseManager.Instance.LogEventLevelEnd(GameCache.Instance.level_selected, type, GameData.Instance.day, game.duration_secs, game.turn_count, remove_pipe_count, construct_pipe_count);
        FacebookManager.Instance.LogEventLevelEnd(GameCache.Instance.level_selected, type, GameData.Instance.day, game.duration_secs, game.turn_count, remove_pipe_count, construct_pipe_count);
        if(unlock_level) FirebaseManager.Instance.SetUserProperties(GameData.Instance.unlock_level);
    }

    private void onLevelSelectChange(object param)
    {
        txtLevel.text = "Level " + GameCache.Instance.level_selected.ToString();
    }

    private void onCoinChange(object param)
    {
        StartCoroutine(coinChangeEffect(txtCoins, Convert.ToInt32(param)));
        btnRemove.interactable = GameData.Instance.coins >= 50;
        btnConstruct.interactable = GameData.Instance.coins >= 25;
        if (!en_rmbtn) btnRemove.interactable = false;
        if (!en_construct_btn) btnConstruct.interactable = false;
    }

    private void onPointChange(object param)
    {
        StartCoroutine(coinChangeEffect(txtPoints, Convert.ToInt32(param)));
    }

    IEnumerator coinChangeEffect(Text text, int value)
    {
        int frame = 10;
        int delta = (Mathf.Abs(value) / frame) + 1;
        int text_value = int.Parse(text.text);
        if(value > 0)
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

    private void nextLevel()
    {
        if (GameCache.Instance.mode == 1)
        {
            if (GameCache.Instance.level_selected == 560)
            {
                GameCache.Instance.lastLevel = true;
                sceneController.loadScene("SimpleLevel");
            }
            else
            {
                GameCache.Instance.level_selected++;
                game.destroy();
                newGameLevel(1);
            }
        }
        else if (GameCache.Instance.mode == 2)
        {
            if (GameCache.Instance.level_selected == 8)
            {
                sceneController.loadScene("ChallengeLevel");
            }
            else
            {
                GameCache.Instance.level_selected++;
                game.destroy();
                newGameLevel(2);
            }
        }
    }

    //private void showPanel(GameObject panel)
    //{
    //    panel.GetComponent<Animator>().Play("Show");
    //    panelShowing = true;
    //    game.panelShowing = true;
    //}

    //private void closePanel(GameObject panel)
    //{
    //    panel.GetComponent<Animator>().Play("Close");
    //    panelShowing = false;
    //    game.panelShowing = false;
    //}

    public void btnSoundOnClick()
    {
        GameData.Instance.sound_on = !GameData.Instance.sound_on;
        AudioManager.Instance.soundVolume(GameData.Instance.sound_on ? 1 : 0);
        AudioManager.Instance.Play("button_sound");
        if (GameData.Instance.sound_on)
        {
            btnSound.GetComponent<Image>().sprite = s_sounds[1];
        }
        else btnSound.GetComponent<Image>().sprite = s_sounds[0];
    }

    public void btnRemoveOnClick()
    {
        if (!tutorial && !animPlaying && en_rmbtn && GameData.Instance.decreaseCoin(50))
        {
            game.removeIncorrectPipes();
            btnRemove.interactable = false;
            en_rmbtn = false;
        }
    }

    public void btnConstructOnClick()
    {
        if (!tutorial && !animPlaying && en_construct_btn && GameData.Instance.decreaseCoin(25))
        {
            if (game.constructPipes())
            {
                btnConstruct.interactable = false;
                en_construct_btn = false;
            }
        }
    }

    //public void btnAddCoinOnClick()
    //{
    //    if (!tutorial && !animPlaying  && !panelShowing )
    //    {
    //        showPanel(panelAddCoin);
    //        PopupManager.Instance.ShowPopup(PopupName.AddCoin, null);
    //    }
    //}

    public void btnAddCoinOnClick()
    {
        if(!tutorial && !animPlaying)
        {
            PopupManager.Instance.ShowPopup(PopupName.AddCoin, null);
        }
    }

    public void btnBackOnClick()
    {
        game.stop_time = true;
        AudioManager.Instance.Stop("water");
        switch (GameCache.Instance.mode)
        {
            case 0:
                sceneController.loadScene("MainMenu");
                break;
            case 1:
                sceneController.loadScene("SimpleLevel");
                break;
            case 2:
                sceneController.loadScene("ChallengeLevel");
                break;
        }

    }
    //public void btnCloseOnPanelRateOnClick()
    //{
    //    if (panelShowing)
    //    {
    //        closePanel(panelRate);
    //        nextLevel();
    //    }
    //}

    //public void btnClose1OnPanelOnClick(GameObject target)
    //{
    //    if (panelShowing)
    //    {
    //        closePanel(target);
    //    }
    //}

    //public void btnClose2OnPanelOnClick(GameObject target)
    //{
    //    if (panelShowing)
    //    {
    //        closePanel(target);
    //        if (GameCache.Instance.canShowAds() && AdManager.Instance.ShowInterstitial(() =>
    //        {
    //            switch (GameCache.Instance.mode)
    //            {
    //                case 1:
    //                    sceneController.loadScene("SimpleLevel");
    //                    break;
    //                case 2:
    //                    sceneController.loadScene("ChallengeLevel");
    //                    break;
    //            }
    //        }))
    //        {
    //            Debug.Log("Show Interstitial Ads");
    //        }
    //        else
    //        {
    //            switch (GameCache.Instance.mode)
    //            {
    //                case 1:
    //                    sceneController.loadScene("SimpleLevel");
    //                    break;
    //                case 2:
    //                    sceneController.loadScene("ChallengeLevel");
    //                    break;
    //            }
    //        }
    //    }
    //}

    //public void btnWatchVideo10TimesCoinOnPanelOnClick()
    //{
    //    if (panelShowing)
    //    {
    //        bool hasVideo = AdManager.Instance.ShowRewardVideo(() =>
    //        {
    //            GameData.Instance.increaseCoin(10 * game.getStar());
    //            btnWatchVideo10TimesCoin.interactable = false;
    //            Debug.Log("Rewarded 10 times coins");
    //        });
    //        FirebaseManager.Instance.LogEventRequestRewardedVideo("x10_coins", hasVideo, GameCache.Instance.level_selected);
    //        FacebookManager.Instance.LogEventRequestRewardedVideo("x10_coins", hasVideo, GameCache.Instance.level_selected);
    //    }
    //}

    //public void btnWatchVideoMoreCoinOnPanelOnClick()
    //{
    //    if (panelShowing)
    //    {
    //        bool hasVideo = AdManager.Instance.ShowRewardVideo(() =>
    //        {
    //            GameData.Instance.increaseCoin(10);
    //            Debug.Log("Rewarded 10 coins");
    //        });
    //        FirebaseManager.Instance.LogEventRequestRewardedVideo("10_coins", hasVideo, GameCache.Instance.level_selected);
    //        FacebookManager.Instance.LogEventRequestRewardedVideo("10_coins", hasVideo, GameCache.Instance.level_selected);
    //    }
    //}

    //public void btnShareFbOnPanelOnClick()
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

    //public void btnNextOnPanelOnClick(GameObject target)
    //{
    //    if (panelShowing)
    //    {
    //        closePanel(target);
    //        if (GameCache.Instance.canShowAds())
    //        {
    //            if (!AdManager.Instance.ShowInterstitial(() => { nextLevel(); }))
    //            {
    //                nextLevel();
    //            }
    //            else
    //            {
    //                Debug.Log("Show Interstitial Ads");
    //            }
    //        }
    //        else if (GameCache.Instance.canShowRatePanel())
    //        {
    //            showPanel(panelRate);
    //        }
    //        else
    //        {
    //            nextLevel();
    //        }
    //    }
    //}



    //    public void btnRateOnPanelOnClick()
    //    {
    //        if (panelShowing)
    //        {
    //            GameData.Instance.rate = true;
    //            // miss id
    //#if UNITY_ANDROID
    //            Application.OpenURL("market://details?id=" + Application.productName);
    //#elif UNITY_IPHONE
    // Application.OpenURL("itms-apps://itunes.apple.com/app/idYOUR_ID");
    //#endif
    //        }
    //    }

    public void CloseLevelPopupCallback()
    {
        if (GameCache.Instance.canShowAds() && AdManager.Instance.ShowInterstitial(() =>
            {
                switch (GameCache.Instance.mode)
                {
                    case 1:
                        sceneController.loadScene("SimpleLevel");
                        break;
                    case 2:
                        sceneController.loadScene("ChallengeLevel");
                        break;
                }
            }))
        {
            Debug.Log("Show Interstitial Ads");
        }
        else
        {
            switch (GameCache.Instance.mode)
            {
                case 1:
                    sceneController.loadScene("SimpleLevel");
                    break;
                case 2:
                    sceneController.loadScene("ChallengeLevel");
                    break;
            }
        }
    }

    public void NextLevelCallback()
    {
        if (GameCache.Instance.canShowAds())
        {
            if (!AdManager.Instance.ShowInterstitial(() => { nextLevel(); }))
            {
                nextLevel();
            }
            else
            {
                Debug.Log("Show Interstitial Ads");
            }
        }
        else if (GameCache.Instance.canShowRatePanel())
        {
            PopupManager.Instance.ShowPopup(PopupName.Rate, new Dictionary<PopupButtonEvent, Action>() { {PopupButtonEvent.ClosePressed, nextLevel} });
        }
        else
        {
            nextLevel();
        }
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveListener(EventID.OnCoinChange, onCoinChange);
        EventDispatcher.Instance.RemoveListener(EventID.OnLevelSelectChange, onLevelSelectChange);
        EventDispatcher.Instance.RemoveListener(EventID.OnPointChange, onPointChange);
        EventDispatcher.Instance.RemoveListener(EventID.PipeAnimationEnd, endGame);
        EventDispatcher.Instance.RemoveListener(EventID.PipeAnimationStart, AnimationStart);
    }
}