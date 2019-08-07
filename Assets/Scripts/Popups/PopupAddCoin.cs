using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class PopupAddCoin : MonoBehaviour, IPopup
{
    [SerializeField] Button btn_Share_Fb;
    [SerializeField] Sprite coin;
    [SerializeField] Button btn_Watch_Video;
    [SerializeField] Text timer_Watch_Video;
    [SerializeField] Text timer_Share_Fb;
    private Action btn_Close_Callback;
    private Action btn_Watch_Video_Callback;
    private Action btn_Share_Fb_Callback;
    private float watchVideoTimeRemain;
    private float shareFbTimeRemain;
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
        btn_Share_Fb.interactable = (System.DateTime.Now - System.DateTime.FromFileTime(GameData.Instance.lastFbShare)).Hours >= 1;
        btn_Watch_Video.interactable = (GameData.Instance.watchVideoRemain > 0) && ((System.DateTime.Now - System.DateTime.FromFileTime(GameData.Instance.lastWatchVideo)).Minutes >= 3);
        setShareFbTimer();
        setWatchVideoTimer();
    }

    private void setWatchVideoTimer()
    {
        timer_Watch_Video.enabled = false;
        if (GameData.Instance.watchVideoRemain > 0)
        {
            if (!btn_Watch_Video.interactable && GameData.Instance.watchVideoRemain > 0)
            {
                int delta = (int)(System.DateTime.Now - System.DateTime.FromFileTime(GameData.Instance.lastWatchVideo)).TotalSeconds;
                if (delta >= 0)
                {
                    int second_remain = 3 * 60 - delta;
                    Debug.Log("______second: " + second_remain);
                    StartCoroutine(countDown(timer_Watch_Video, second_remain, btn_Watch_Video));
                }
            }
        }
    }

    private void setShareFbTimer()
    {
        timer_Share_Fb.enabled = false;
        if (!btn_Share_Fb.interactable)
        {
            int delta = (int)(System.DateTime.Now - System.DateTime.FromFileTime(GameData.Instance.lastFbShare)).TotalSeconds;
            if(delta >= 0)
            {
                int second_remain = 60 * 60 - delta;
                StartCoroutine(countDown(timer_Share_Fb, second_remain, btn_Share_Fb));
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
            text_change.text = time.Hours > 0 ? string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds): string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);
            yield return new WaitForSeconds(1f);
        }
        active_btn.interactable = true;
        text_change.enabled = false;
    }

    public void OnDisplayed()
    {
        isShow = true;
    }

    public void OnClosed()
    {
        GetComponent<RectTransform>().gameObject.SetActive(false);
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
        btn_Watch_Video_Callback = list_actions.ContainsKey(PopupButtonEvent.WatchVideoMoreCoinPressed) ? list_actions[PopupButtonEvent.WatchVideoMoreCoinPressed] : null;
        btn_Share_Fb_Callback = list_actions.ContainsKey(PopupButtonEvent.ShareFacebookPressed) ? list_actions[PopupButtonEvent.ShareFacebookPressed] : null;
        GetComponent<Animator>().Play("Show");
    }

    public void BtnCloseOnClick()
    {
        if (isShow)
        {
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
                int reward = GameConfig.REWARDED_VIDEO_COIN;
                GameData.Instance.increaseCoin(reward);
                btn_Watch_Video.interactable = false;
                GameData.Instance.watchVideoRemain--;
                GameData.Instance.lastWatchVideo = System.DateTime.Now.ToFileTime();
                string content = "";
                if (GameData.Instance.watchVideoRemain > 0) content = "You are rewarded " + reward + " coins. Please wait 3 minutes for the next time!";
                else content = "You are rewarded " + reward + " coins. You have watched all video of today!";
                PopupManager.Instance.ShowNotification(content, coin, 1.5f);
                setWatchVideoTimer();
            });
            FirebaseManager.Instance.LogEventRequestRewardedVideo("4_coins", hasVideo, GameCache.Instance.levelSelected);
            FacebookManager.Instance.LogEventRequestRewardedVideo("4_coins", hasVideo, GameCache.Instance.levelSelected);
            btn_Watch_Video_Callback?.Invoke();
        }
    }

    public void BtnShareFbOnClick()
    {
        if (isShow)
        {
            FacebookManager.Instance.ShareWithFriends(() =>
            {
                int reward = GameConfig.SHARE_FB_COIN_REWARD;
                GameData.Instance.increaseCoin(reward);
                PopupManager.Instance.ShowNotification("Share Facebook completed. You are rewarded " + reward + " coins. Please wait 1 hour for the next time!", coin, 1.5f);
                FirebaseManager.Instance.LogEventShareFacebook();
                FacebookManager.Instance.LogEventShareFacebook();
                btn_Share_Fb.interactable = false;
                GameData.Instance.lastFbShare = System.DateTime.Now.ToFileTime();
                setShareFbTimer();
            });
            btn_Share_Fb_Callback?.Invoke();
        }
    }
}
