﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class PopupPassLevel : MonoBehaviour, IPopup
{
    [SerializeField] Button btn_Next;
    [SerializeField] Button btn_Watch_Video;
    [SerializeField] Text text_Coin;
    [SerializeField] Text text_Point;
    [SerializeField] CanvasGroup middleGroup;
    [SerializeField] CanvasGroup bottomGroup;
    [SerializeField] Sprite coin;
    [SerializeField] Text timer_Watch_Video;
    private Action btn_Close_Callback;
    private Action btn_Watch_Video_Callback;
    private Action btn_Next_Callback;
    private Action on_Displayed;
    private int value;
    private bool isShow;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Setup()
    {
        isShow = false;
        btn_Watch_Video.interactable = (GameData.Instance.watchVideoRemain > 0) && ((System.DateTime.Now - GameData.Instance.lastWatchVideo).Minutes >= 3);
        middleGroup.interactable = false;
        bottomGroup.interactable = false;
        middleGroup.alpha = 0f;
        bottomGroup.alpha = 0f;
        setWatchVideoTimer();
    }

    private void setWatchVideoTimer()
    {
        timer_Watch_Video.enabled = false;
        if (!btn_Watch_Video.interactable && GameData.Instance.watchVideoRemain > 0)
        {
            int delta = (int)(System.DateTime.Now - GameData.Instance.lastWatchVideo).TotalSeconds;
            if (delta >= 0)
            {

                int second_remain = 3 * 60 - delta;
                StartCoroutine(countDown(timer_Watch_Video, second_remain, btn_Watch_Video));
            }
        }
    }

    IEnumerator countDown(Text text_change, int second, Button active_btn)
    {
        text_change.enabled = true;
        while (second > 0)
        {
            second--;
            TimeSpan time = TimeSpan.FromSeconds(second);
            text_change.text = time.Hours > 0 ? string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds) : string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);
            yield return new WaitForSeconds(1f);
        }
        active_btn.interactable = true;
        text_change.enabled = false;
    }

    public void OnDisplayed()
    {
        on_Displayed?.Invoke();
        StartCoroutine(fadeInEffect(middleGroup, () =>
        {
            StartCoroutine(fadeInEffect(bottomGroup, () =>
            {
                isShow = true;
                middleGroup.interactable = true;
                bottomGroup.interactable = true;
            }));
        }));
    }

    public void OnClosed()
    {
        GetComponent<RectTransform>().gameObject.SetActive(false);
    }

    IEnumerator fadeInEffect(CanvasGroup target, Action EndCallback)
    {
        float speed = 4f;
        while (target.alpha < 1)
        {
            target.alpha = (target.alpha + speed * Time.deltaTime) < 1 ? target.alpha + speed * Time.deltaTime : 1;
            yield return null;
        }
        EndCallback();
    }

    public void Close()
    {
        isShow = false;
        EventDispatcher.Instance.PostEvent(EventID.OnPopupClosed, this);
        GetComponent<Animator>().Play("Close");
    }

    public void Show(Dictionary<PopupButtonEvent, Action> list_actions, Dictionary<PopupSettingType, object> list_settings)
    {
        Setup();
        btn_Close_Callback = list_actions.ContainsKey(PopupButtonEvent.ClosePressed) ? list_actions[PopupButtonEvent.ClosePressed] : null;
        btn_Watch_Video_Callback = list_actions.ContainsKey(PopupButtonEvent.WatchVideo10TimesCoinPressed) ? list_actions[PopupButtonEvent.WatchVideo10TimesCoinPressed] : null;
        btn_Next_Callback = list_actions.ContainsKey(PopupButtonEvent.NextLevelPressed) ? list_actions[PopupButtonEvent.NextLevelPressed] : null;
        on_Displayed = list_actions.ContainsKey(PopupButtonEvent.OnPopupDisplayed) ? list_actions[PopupButtonEvent.OnPopupDisplayed] : null;
        value = list_settings.ContainsKey(PopupSettingType.PassLevelImageType) ? Convert.ToInt32(list_settings[PopupSettingType.PassLevelImageType]) : 0;
        if (value >= 1 && value <= 3)
        {
            text_Coin.text = "+" + GameConfig.PASS_LEVEL_COIN_REWARD[value - 1].ToString();
            text_Point.text = "+" + GameConfig.PASS_LEVEL_POINT_REWARD[value - 1].ToString();
        }
        GetComponent<Animator>().Play("Show");
    }

    public void BtnCloseOnClick()
    {
        if (isShow)
        {
            AudioManager.Instance.Play(AudioManager.SoundName.BUTTON);
            Close();
            btn_Close_Callback?.Invoke();
        }
    }

    public void BtnWatchVideoOnClick()
    {
        if (isShow)
        {
            bool hasVideo = AdManager.Instance.ShowRewardVideo(() =>
            {
                int reward = 5 * GameConfig.PASS_LEVEL_COIN_REWARD[value - 1];
                GameData.Instance.increaseCoin(reward);
                btn_Watch_Video.interactable = false;
                GameData.Instance.watchVideoRemain--;
                GameData.Instance.lastWatchVideo = System.DateTime.Now;
                string content = "";
                if (GameData.Instance.watchVideoRemain > 0) content = "You are rewarded " + reward + " coins!";
                else content = "You are rewarded " + reward + " coins. You have watched all video of today!";
                PopupManager.Instance.ShowNotification(content, coin, 2f);
            });
            FirebaseManager.Instance.LogEventRequestRewardedVideo("x5_coins", hasVideo, GameCache.Instance.mode == 2 ? GameCache.Instance.challengeLevelSelected : GameCache.Instance.simpleLevelSelected);
            FacebookManager.Instance.LogEventRequestRewardedVideo("x5_coins", hasVideo, GameCache.Instance.mode == 2 ? GameCache.Instance.challengeLevelSelected : GameCache.Instance.simpleLevelSelected);
            btn_Watch_Video_Callback?.Invoke();
        }
    }

    public void BtnNextOnClick()
    {
        if (isShow)
        {
            AudioManager.Instance.Play(AudioManager.SoundName.BUTTON);
            Close();
            btn_Next_Callback?.Invoke();
        }
    }
}
