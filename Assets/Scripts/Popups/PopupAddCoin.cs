 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class PopupAddCoin : MonoBehaviour, IPopup
{
    [SerializeField] Button btn_Share_Fb;
    private Action btn_Close_Callback;
    private Action btn_Watch_Video_Callback;
    private Action btn_Share_Fb_Callback;
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
                GameData.Instance.increaseCoin(GameConfig.REWARDED_VIDEO_COIN);
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
            FacebookManager.Instance.ShareWithFriends(() => {
                GameData.Instance.increaseCoin(GameConfig.SHARE_FB_COIN_REWARD);
                FirebaseManager.Instance.LogEventShareFacebook();
                FacebookManager.Instance.LogEventShareFacebook();
                btn_Share_Fb.interactable = false;
                GameData.Instance.lastFbShare = System.DateTime.Now.ToFileTime();
            });
            btn_Share_Fb_Callback?.Invoke();
        }
    }
}
