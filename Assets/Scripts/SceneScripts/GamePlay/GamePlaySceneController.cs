using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GamePlaySceneController : MonoBehaviour
{
    [SerializeField] Button btnBack;
    [SerializeField] Button btnSound;
    [SerializeField] Button btnRemove;
    [SerializeField] Button btnConstruct;
    [SerializeField] Button btnAddCoin;
    [SerializeField] Text txtCoins;
    [SerializeField] Text txtPoints;
    [SerializeField] Text txtLevel;
    [SerializeField] Sprite[] s_Sounds;
    private GameController game;
    private bool gameover;
    private bool animPlaying;
    private bool tutorial;
    private bool en_RemoveBtn;
    private bool en_ConstructBtn;
    private void Awake()
    {   
        LoadSceneManager.Instance.OpenScene();
        EventDispatcher.Instance.RegisterListener(EventID.OnCoinChange, onCoinChange);
        EventDispatcher.Instance.RegisterListener(EventID.OnPointChange, onPointChange);
        EventDispatcher.Instance.RegisterListener(EventID.OnLevelSelectChange, onLevelSelectChange);
        EventDispatcher.Instance.RegisterListener(EventID.PipeAnimationEnd, endGame);
        EventDispatcher.Instance.RegisterListener(EventID.PipeAnimationStart, AnimationStart);

        if (GameCache.Instance.mode == 0 || (GameCache.Instance.levelSelected == 1 && GameCache.Instance.mode == 1)) // tutorial
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
            game = GetComponent<TutorialModeController>();
            game.enabled = true;
            tutorial = true;
        }
        else if(type == 1)
        {
            game = GetComponent<SimpleModeController>();
            game.enabled = true;
            tutorial = false;
        }
        else
        {
            game = GetComponent<ChallengeModeController>();
            game.enabled = true;
            tutorial = false;
        }
        btnBack.interactable = true;
        btnAddCoin.interactable = true;
        btnSound.GetComponent<Image>().sprite = GameData.Instance.isSoundOn ? s_Sounds[1] : s_Sounds[0];
        txtCoins.text = GameData.Instance.coins.ToString();
        txtPoints.text = GameData.Instance.points.ToString();
        txtLevel.text = GameCache.Instance.levelSelected.ToString();
        //Gamelogic
        gameover = animPlaying = false;
        game.loadLevelData();
        game.setupLevel();
        en_RemoveBtn = game.RemovePipeCount == 0;
        en_ConstructBtn = !game.EndConstructPipe;
        btnRemove.interactable = en_RemoveBtn && GameData.Instance.coins >= 50;
        btnConstruct.interactable = en_ConstructBtn && GameData.Instance.coins >= 25;
        string[] str_type = { "tutorial", "simple","daily_challenge" };
        FirebaseManager.Instance.LogEventLevelStart(GameCache.Instance.levelSelected, str_type[type], GameData.Instance.day);
        FacebookManager.Instance.LogEventLevelStart(GameCache.Instance.levelSelected, str_type[type], GameData.Instance.day);
    }

    public void AnimationStart(object param)
    {
        animPlaying = true;
        btnBack.interactable = false;
        btnConstruct.interactable = false;
        btnRemove.interactable = false;
        btnAddCoin.interactable = false;
        en_RemoveBtn = false;
        en_ConstructBtn = false;
    }

    private void endGame(object param)
    {
        gameover = true;
        bool unlock_level = false;
        if (GameCache.Instance.mode == 0)
        {
            LoadSceneManager.Instance.LoadScene("MainMenu");
        }
        else if (GameCache.Instance.mode == 1)
        {
            if (GameData.Instance.listLevelStars[GameCache.Instance.levelSelected - 1] > 0)
            {
                int c_star = GameData.Instance.listLevelStars[GameCache.Instance.levelSelected - 1];
                int n_star = game.getStar();
                if (n_star > c_star) GameData.Instance.listLevelStars[GameCache.Instance.levelSelected - 1] = n_star;
                Action action = () => { PopupManager.Instance.ShowPopup(PopupName.NextLevel, new Dictionary<PopupButtonEvent, Action>() { { PopupButtonEvent.ClosePressed, CloseLevelPopupCallback }, { PopupButtonEvent.NextLevelPressed, NextLevelCallback } }); };
                if(!tutorial && GameCache.Instance.canShowAds2() && AdManager.Instance.ShowInterstitial(action))
                {
                    Debug.Log("Show Intersitial Ads");
                }
                else
                {
                    action();
                }
            }
            else
            {
                int star = game.getStar();
                GameData.Instance.listLevelStars[GameCache.Instance.levelSelected - 1] = star;
                GameData.Instance.increaseCoin(star);
                GameData.Instance.increasePoint(star * 10);
                if (GameData.Instance.unlockLevel < 560)
                {
                    GameData.Instance.listLevelStars.Add(0);
                    GameData.Instance.unlockLevel++;
                    unlock_level = true;
                }
                GameData.Instance.unlockLvState.NewLevelState();
                Action action = () => { PopupManager.Instance.ShowPopup(PopupName.PassLevel, new Dictionary<PopupButtonEvent, Action>() { { PopupButtonEvent.ClosePressed, CloseLevelPopupCallback }, { PopupButtonEvent.NextLevelPressed, NextLevelCallback } }, new Dictionary<PopupSettingType, object>() { { PopupSettingType.PassLevelImageType, star } }); };
                if (!tutorial && GameCache.Instance.canShowAds2() && AdManager.Instance.ShowInterstitial(action))
                {
                    Debug.Log("Show Intersitial Ads");
                }
                else
                {
                    action();
                }
            }
            
        }
        else if (GameCache.Instance.mode == 2)
        {
            if (GameData.Instance.dailyChallengeProgess[GameCache.Instance.levelSelected - 1] == 1)
            {
                Action action = () => { PopupManager.Instance.ShowPopup(PopupName.NextLevel, new Dictionary<PopupButtonEvent, Action>() { { PopupButtonEvent.ClosePressed, CloseLevelPopupCallback }, { PopupButtonEvent.NextLevelPressed, NextLevelCallback } }); };
                if (GameCache.Instance.canShowAds2() && AdManager.Instance.ShowInterstitial(action))
                {
                    Debug.Log("Show Intersitial Ads");
                }
                else
                {
                    action();
                }
            }
            else
            {
                int star = game.getStar();
                GameData.Instance.dailyChallengeProgess[GameCache.Instance.levelSelected - 1] = 1;
                GameData.Instance.increaseCoin(star);
                GameData.Instance.increasePoint(star * 10);
                Action action = () => { PopupManager.Instance.ShowPopup(PopupName.PassLevel, new Dictionary<PopupButtonEvent, Action>() { { PopupButtonEvent.ClosePressed, CloseLevelPopupCallback }, { PopupButtonEvent.NextLevelPressed, NextLevelCallback } }, new Dictionary<PopupSettingType, object>() { { PopupSettingType.PassLevelImageType, star } }); };
                if (GameCache.Instance.canShowAds2() && AdManager.Instance.ShowInterstitial(action))
                {
                    Debug.Log("Show Intersitial Ads");
                }
                else
                {
                    action();
                }
            }
        }
        string type = tutorial ? "tutorial" : (GameCache.Instance.mode == 1 ? "simple" : "daily_challenge");
        int remove_pipe_count = game.RemovePipeCount;
        int construct_pipe_count = game.ConstructPipeCount;
        Debug.Log("Duration: " + game.DurationSecs);
        Debug.Log("Turn: " + game.TurnCount);
        FirebaseManager.Instance.LogEventLevelEnd(GameCache.Instance.levelSelected, type, GameData.Instance.day, game.DurationSecs, game.TurnCount, remove_pipe_count, construct_pipe_count);
        FacebookManager.Instance.LogEventLevelEnd(GameCache.Instance.levelSelected, type, GameData.Instance.day, game.DurationSecs, game.TurnCount, remove_pipe_count, construct_pipe_count);
        if(unlock_level) FirebaseManager.Instance.SetUserProperties(GameData.Instance.unlockLevel);
    }

    private void onLevelSelectChange(object param)
    {
        txtLevel.text = GameCache.Instance.levelSelected.ToString();
    }

    private void onCoinChange(object param)
    {
        StartCoroutine(coinChangeEffect(txtCoins, Convert.ToInt32(param)));
        btnRemove.interactable = GameData.Instance.coins >= 50;
        btnConstruct.interactable = GameData.Instance.coins >= 25;
        if (!en_RemoveBtn) btnRemove.interactable = false;
        if (!en_ConstructBtn) btnConstruct.interactable = false;
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
            if (GameCache.Instance.levelSelected == 560)
            {
                GameCache.Instance.lastLevel = true;
                LoadSceneManager.Instance.LoadScene("SimpleLevel");
            }
            else
            {
                GameCache.Instance.levelSelected++;
                game.destroy();
                newGameLevel(1);
            }
        }
        else if (GameCache.Instance.mode == 2)
        {
            if (GameCache.Instance.levelSelected == 8)
            {
                LoadSceneManager.Instance.LoadScene("ChallengeLevel");
            }
            else
            {
                GameCache.Instance.levelSelected++;
                game.destroy();
                newGameLevel(2);
            }
        }
    }

    public void btnSoundOnClick()
    {
        GameData.Instance.isSoundOn = !GameData.Instance.isSoundOn;
        AudioManager.Instance.soundVolume(GameData.Instance.isSoundOn ? 1 : 0);
        AudioManager.Instance.Play("button_sound");
        if (GameData.Instance.isSoundOn)
        {
            btnSound.GetComponent<Image>().sprite = s_Sounds[1];
        }
        else btnSound.GetComponent<Image>().sprite = s_Sounds[0];
    }

    public void btnRemoveOnClick()
    {
        if (!tutorial && !animPlaying && en_RemoveBtn && GameData.Instance.decreaseCoin(50))
        {
            game.removeIncorrectPipes();
            btnRemove.interactable = false;
            en_RemoveBtn = false;
        }
    }

    public void btnConstructOnClick()
    {
        if (!tutorial && !animPlaying && en_ConstructBtn && GameData.Instance.decreaseCoin(25))
        {
            if (game.constructPipes())
            {
                btnConstruct.interactable = false;
                en_ConstructBtn = false;
            }
        }
    }

    public void btnAddCoinOnClick()
    {
        if(!tutorial && !animPlaying)
        {
            PopupManager.Instance.ShowPopup(PopupName.AddCoin, null);
        }
    }

    public void btnBackOnClick()
    {
        game.StopTime = true;
        AudioManager.Instance.Stop("water");
        switch (GameCache.Instance.mode)
        {
            case 0:
                LoadSceneManager.Instance.LoadScene("MainMenu");
                break;
            case 1:
                LoadSceneManager.Instance.LoadScene("SimpleLevel");
                break;
            case 2:
                LoadSceneManager.Instance.LoadScene("ChallengeLevel");
                break;
        }
    }

    public void CloseLevelPopupCallback()
    {
        Action action = () => {
            switch (GameCache.Instance.mode)
            {
                case 1:
                    LoadSceneManager.Instance.LoadScene("SimpleLevel");
                    break;
                case 2:
                    LoadSceneManager.Instance.LoadScene("ChallengeLevel");
                    break;
            }
        };
        if (GameCache.Instance.canShowAds1() && AdManager.Instance.ShowInterstitial(action))
        {
            Debug.Log("Show Interstitial Ads");
        }
        else
        {
            action();
        }
    }

    public void NextLevelCallback()
    {
        Action action = () => {
            if (GameCache.Instance.canShowRatePanel())
            {
                PopupManager.Instance.ShowPopup(PopupName.Rate, new Dictionary<PopupButtonEvent, Action>() { { PopupButtonEvent.ClosePressed, nextLevel }, { PopupButtonEvent.NotNowOnRatePressed, nextLevel } });
            }
            else
            {
                nextLevel();
            }
        };
        if (GameCache.Instance.canShowAds1() && AdManager.Instance.ShowInterstitial(action))
        {
            Debug.Log("Show Interstitial Ads");
        }
        else
        {
            action();
        }
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveListener(EventID.OnCoinChange, onCoinChange);
        EventDispatcher.Instance.RemoveListener(EventID.OnLevelSelectChange, onLevelSelectChange);
        EventDispatcher.Instance.RemoveListener(EventID.OnPointChange, onPointChange);
        EventDispatcher.Instance.RemoveListener(EventID.PipeAnimationEnd, endGame);
        EventDispatcher.Instance.RemoveListener(EventID.PipeAnimationStart, AnimationStart);
        PopupManager.Instance.ForceClosePopup();
    }
}