using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GamePlaySceneController : MonoBehaviour
{
    public SceneController sceneController;
    public GameController game;
    public GameObject panelRate;
    public GameObject panelAddCoin;
    public GameObject panelPassedLevel;
    public GameObject panelNextLevel;
    public Button btnSound;
    public Button btnRemove;
    public Button btnConstruct;
    public Button btnWatchVideo10TimesCoin;
    public Image coinReward;
    public Image pointReward;
    public Text txtCoins;
    public Text txtPoints;
    public Text txtLevel;
    public Sprite[] s_coins;
    public Sprite[] s_points;
    public Sprite[] s_sounds;

    private bool gameover;
    private bool animPlaying;
    private int construct_part;
    private bool tutorial;
    private bool en_rmbtn;
    private bool en_construct_btn;
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
        btnRemove.interactable = GameData.Instance.coins >= 50;
        btnConstruct.interactable = GameData.Instance.coins >= 25;
        if (GameData.Instance.sound_on) btnSound.GetComponent<Image>().sprite = s_sounds[1];
        else btnSound.GetComponent<Image>().sprite = s_sounds[0];
        txtCoins.text = GameData.Instance.coins.ToString();
        txtPoints.text = GameData.Instance.points.ToString();
        txtLevel.text = "Level " + GameCache.Instance.level_selected.ToString();
        //Gamelogic
        gameover = animPlaying = false;
        en_rmbtn = true;
        en_construct_btn = true;
        construct_part = 0;
        game.loadLevelData();
        game.setupLevel();
        string[] str_type = { "tutorial", "simple","daily_challenge" };
        FirebaseManager.Instance.LogEventLevelStart(GameCache.Instance.level_selected, str_type[type], GameData.Instance.day);
        FacebookManager.Instance.LogEventLevelStart(GameCache.Instance.level_selected, str_type[type], GameData.Instance.day);
    }


    public void AnimationStart(object param)
    {
        btnConstruct.interactable = false;
        btnRemove.interactable = false;
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
            if (GameData.Instance.level_stars.Count > GameCache.Instance.level_selected)
            {
                int c_star = GameData.Instance.level_stars[GameCache.Instance.level_selected - 1];
                int n_star = game.getStar();
                if (n_star > c_star) GameData.Instance.level_stars[GameCache.Instance.level_selected - 1] = n_star;
                panelNextLevel.GetComponent<Animator>().Play("Show");
            }
            else
            {
                int star = game.getStar();
                GameData.Instance.level_stars[GameCache.Instance.level_selected - 1] = star;
                GameData.Instance.increaseCoin(star);
                GameData.Instance.increasePoint(star * 10);
                coinReward.sprite = s_coins[star - 1];
                pointReward.sprite = s_points[star - 1];
                if (GameData.Instance.unlock_level < 560)
                {
                    GameData.Instance.level_stars.Add(0);
                    GameData.Instance.unlock_level++;
                    unlock_level = true;
                }
                //else
                //{
                //    // vuot qua 560 level

                //}
                btnWatchVideo10TimesCoin.interactable = true;
                panelPassedLevel.GetComponent<Animator>().Play("Show");
            }
        }
        else if (GameCache.Instance.mode == 2)
        {
            if (GameData.Instance.completed[GameCache.Instance.level_selected - 1] == 1)
            {
                panelNextLevel.GetComponent<Animator>().Play("Show");
            }
            else
            {
                int star = game.getStar();
                GameData.Instance.completed[GameCache.Instance.level_selected - 1] = 1;
                GameData.Instance.increaseCoin(star);
                GameData.Instance.increasePoint(star * 10);
                coinReward.sprite = s_coins[star - 1];
                pointReward.sprite = s_points[star - 1];
                panelPassedLevel.GetComponent<Animator>().Play("Show");
            }
        }
        string type = tutorial ? "tutorial" : (GameCache.Instance.mode == 1 ? "simple" : "daily_challenge");
        int remove_pipe_count = en_rmbtn ? 0 : 1;
        int construct_pipe_count = construct_part;
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
        txtCoins.text = GameData.Instance.coins.ToString();
        btnRemove.interactable = GameData.Instance.coins >= 50;
        btnConstruct.interactable = GameData.Instance.coins >= 25;
        if (!en_rmbtn) btnRemove.interactable = false;
        if (!en_construct_btn) btnConstruct.interactable = false;
    }

    private void onPointChange(object param)
    {
        txtPoints.text = GameData.Instance.points.ToString();
    }

    private void nextLevel()
    {
        if (GameCache.Instance.mode == 1)
        {
            if (GameCache.Instance.level_selected == 560)
            {
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

    public void btnSoundOnClick()
    {
        GameData.Instance.sound_on = !GameData.Instance.sound_on;
        if (GameData.Instance.sound_on) btnSound.GetComponent<Image>().sprite = s_sounds[1];
        else btnSound.GetComponent<Image>().sprite = s_sounds[0];
    }

    public void btnRemoveOnClick()
    {
        if (!tutorial && GameData.Instance.decreaseCoin(50))
        {
            game.removeIncorrectPipes();
            btnRemove.interactable = false;
            en_rmbtn = true;
        }
    }

    public void btnConstructOnClick()
    {
        if (!tutorial && GameData.Instance.decreaseCoin(25))
        {
            if (game.constructPipes(construct_part++))
            {
                btnConstruct.interactable = false;
                en_construct_btn = false;
            }
        }
    }

    public void btnAddCoinOnClick()
    {
        if (!tutorial && !animPlaying)
        {
            panelAddCoin.GetComponent<Animator>().Play("Show");
        }
    }

    public void btnBackOnClick()
    {
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
    public void btnCloseOnPanelRateOnClick()
    {
        panelRate.GetComponent<Animator>().Play("Close");
        nextLevel();
    }

    public void btnClose1OnPanelOnClick(GameObject target)
    {
        target.GetComponent<Animator>().Play("Close");
    }

    public void btnClose2OnPanelOnClick(GameObject target)
    {
        target.GetComponent<Animator>().Play("Close");
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

    public void btnWatchVideo10TimesCoinOnPanelOnClick()
    {
        bool hasVideo = AdManager.Instance.ShowRewardVideo(() =>
        {
            GameData.Instance.increaseCoin(10 * game.getStar());
            btnWatchVideo10TimesCoin.interactable = false;
            Debug.Log("Rewarded 10 times coins");
        });
        FirebaseManager.Instance.LogEventRequestRewardedVideo("x10_coins", hasVideo, GameCache.Instance.level_selected);
        FacebookManager.Instance.LogEventRequestRewardedVideo("x10_coins", hasVideo, GameCache.Instance.level_selected);
    }

    public void btnWatchVideoMoreCoinOnPanelOnClick()
    {
        bool hasVideo = AdManager.Instance.ShowRewardVideo(() =>
        {
            GameData.Instance.increaseCoin(10);
            Debug.Log("Rewarded 10 coins");
        });
        FirebaseManager.Instance.LogEventRequestRewardedVideo("10_coins", hasVideo, GameCache.Instance.level_selected);
        FacebookManager.Instance.LogEventRequestRewardedVideo("10_coins", hasVideo, GameCache.Instance.level_selected);
    }

    public void btnShareFbOnPanelOnClick()
    {
        FacebookManager.Instance.ShareWithFriends(() => {
            GameData.Instance.increaseCoin(50);
            FirebaseManager.Instance.LogEventShareFacebook();
            FacebookManager.Instance.LogEventShareFacebook();
        });
    }

    public void btnNextOnPanelOnClick(GameObject target)
    {
        target.GetComponent<Animator>().Play("Close");
        if (GameCache.Instance.canShowAds())
        {
            if(!AdManager.Instance.ShowInterstitial(() => { nextLevel(); }))
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
            panelRate.GetComponent<Animator>().Play("Show");
        }
        else
        {
            nextLevel();
        }

    }

    private void OnDestroy()
    {
        Debug.Log("Destroy Play Scene");
        EventDispatcher.Instance.RemoveListener(EventID.OnCoinChange, onCoinChange);
        EventDispatcher.Instance.RemoveListener(EventID.OnLevelSelectChange, onLevelSelectChange);
        EventDispatcher.Instance.RemoveListener(EventID.OnPointChange, onPointChange);
        EventDispatcher.Instance.RemoveListener(EventID.PipeAnimationEnd, endGame);
        EventDispatcher.Instance.RemoveListener(EventID.PipeAnimationStart, AnimationStart);
    }
}