 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class PopupAddCoin : MonoBehaviour, IPopup
{
    [SerializeField] Button btn_Close;
    [SerializeField] Button btn_Share_Fb;
    [SerializeField] Button btn_Watch_Video;
    private Action btn_Close_Callback;
    private Action btn_Watch_Video_Callback;
    private Action btn_Share_Fb_Callback;
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
        
    }

    public void OnDisplayed()
    {
        btn_Close.enabled = true;
        btn_Share_Fb.enabled = true;
        btn_Watch_Video.enabled = true;
    }

    public void OnClosed()
    {
        GetComponent<RectTransform>().gameObject.SetActive(false);
    }

    public void Close()
    {
        btn_Close.enabled = false;
        btn_Share_Fb.enabled = false;
        btn_Watch_Video.enabled = false;
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
        Close();
        btn_Close_Callback?.Invoke();
    }

    public void BtnWatchVideoOnClick()
    {
        bool hasVideo = AdManager.Instance.ShowRewardVideo(() =>
        {
            GameData.Instance.increaseCoin(10);
        });
        FirebaseManager.Instance.LogEventRequestRewardedVideo("10_coins", hasVideo, GameCache.Instance.level_selected);
        FacebookManager.Instance.LogEventRequestRewardedVideo("10_coins", hasVideo, GameCache.Instance.level_selected);
        btn_Watch_Video_Callback?.Invoke();
    }

    public void BtnShareFbOnClick()
    {
        FacebookManager.Instance.ShareWithFriends(() => {
            GameData.Instance.increaseCoin(50);
            FirebaseManager.Instance.LogEventShareFacebook();
            FacebookManager.Instance.LogEventShareFacebook();
        });
        btn_Share_Fb_Callback?.Invoke();
    }
}
