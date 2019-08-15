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
    [SerializeField] Text title;
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
        btn_Share_Fb.interactable = (System.DateTime.Now - GameData.Instance.lastFbShare).Hours >= 1;
        btn_Watch_Video.interactable = (GameData.Instance.watchVideoRemain > 0) && ((System.DateTime.Now - GameData.Instance.lastWatchVideo).Minutes >= 3);
        setShareFbTimer();
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

    private void setShareFbTimer()
    {
        timer_Share_Fb.enabled = false;
        if (!btn_Share_Fb.interactable)
        {
            int delta = (int)(System.DateTime.Now - GameData.Instance.lastFbShare).TotalSeconds;
            if (delta >= 0)
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
            text_change.text = time.Hours > 0 ? string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds) : string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);
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
        if (list_settings != null && list_settings.ContainsKey(PopupSettingType.AddCoinType))
        {
            title.text = "Your coin is less than " + list_settings[PopupSettingType.AddCoinType].ToString() + ", not enough to use hint. Get more coins to use.";
        }
        else if(GameData.Instance.coins < GameConfig.CONSTRUCT_PIPE_COST)
        {
            title.text = "Your coin is less than " + GameConfig.CONSTRUCT_PIPE_COST + ", not enough to use hint. Get more coins to use.";
        }
        else if (GameData.Instance.coins < GameConfig.REMOVE_PIPE_COST)
        {
            title.text = "Your coin is less than " + GameConfig.REMOVE_PIPE_COST + ", not enough to use hint. Get more coins to use.";
        }
        else
        {
            title.text = "Get more coins here!";
        }
        GameCache.Instance.showAddCoinCount = 0;
        GetComponent<Animator>().Play("Show");
    }

    public void BtnCloseOnClick()
    {
        if (isShow)
        {
            AudioManager.Instance.Play("button_sound");
            Close();
            btn_Close_Callback?.Invoke();
        }
    }

    public void BtnShopOnClick()
    {
        if (isShow)
        {
            AudioManager.Instance.Play("button_sound");
            Close();
            PopupManager.Instance.ShowPopup(PopupName.Shop, null);
        }
    }

    public void BtnWatchVideoOnClick()
    {
        if (isShow)
        {
            AudioManager.Instance.Play("button_sound");
            bool hasVideo = AdManager.Instance.ShowRewardVideo(() =>
            {
                int reward = GameConfig.REWARDED_VIDEO_COIN;
                GameData.Instance.increaseCoin(reward);
                btn_Watch_Video.interactable = false;
                GameData.Instance.watchVideoRemain--;
                GameData.Instance.lastWatchVideo = System.DateTime.Now;
                string content = "";
                if (GameData.Instance.watchVideoRemain > 0) content = "You are rewarded " + reward + " coins!";
                else content = "You are rewarded " + reward + " coins. You have watched all video of today!";
                PopupManager.Instance.ShowNotification(content, coin, 2f);
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
            AudioManager.Instance.Play("button_sound");
            FacebookManager.Instance.ShareWithFriends(() =>
            {
                int reward = GameConfig.SHARE_FB_COIN_REWARD;
                GameData.Instance.increaseCoin(reward);
                PopupManager.Instance.ShowNotification("Share Facebook completed. You are rewarded " + reward + " coins!", coin, 2f);
                FirebaseManager.Instance.LogEventShareFacebook();
                FacebookManager.Instance.LogEventShareFacebook();
                btn_Share_Fb.interactable = false;
                GameData.Instance.lastFbShare = System.DateTime.Now;
                setShareFbTimer();
            }, () => {
                PopupManager.Instance.ShowNotification("Share Facebook failed!", null, 1.5f);
            });
            btn_Share_Fb_Callback?.Invoke();
        }
    }
}
