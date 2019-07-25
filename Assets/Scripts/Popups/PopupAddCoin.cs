using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class PopupAddCoin : MonoBehaviour
{
    [SerializeField] Button btn_Close;
    [SerializeField] Button btn_Share_Fb;
    [SerializeField] Button btn_Watch_Video;
    private Action btn_Close_Callback;
    private Action btn_Watch_Video_Callback;
    private Action btn_Share_Fb_Callback;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Setup()
    {
        btn_Close.interactable = true;
        btn_Share_Fb.interactable = true;
        btn_Watch_Video.interactable = true;
    }

    public void Close()
    {
        btn_Close.interactable = false;
        btn_Share_Fb.interactable = false;
        btn_Watch_Video.interactable = false;
        animator.Play("Close");
    }

    public void Show(Dictionary<PopupButtonName, Action> list_actions)
    {
        Setup();
        btn_Close_Callback = list_actions.ContainsKey(PopupButtonName.Close) ? list_actions[PopupButtonName.Close] : null;
        btn_Watch_Video_Callback = list_actions.ContainsKey(PopupButtonName.WatchVideoMoreCoin) ? list_actions[PopupButtonName.WatchVideoMoreCoin] : null;
        btn_Share_Fb_Callback = list_actions.ContainsKey(PopupButtonName.ShareFacebook) ? list_actions[PopupButtonName.ShareFacebook] : null;
        animator.Play("Show");
    }

    public void BtnCloseOnClick()
    {
        Close();
        if (btn_Close_Callback != null) btn_Close_Callback();
    }

    public void BtnWatchVideoOnClick()
    {
        bool hasVideo = AdManager.Instance.ShowRewardVideo(() =>
        {
            GameData.Instance.increaseCoin(10);
        });
        FirebaseManager.Instance.LogEventRequestRewardedVideo("10_coins", hasVideo, GameCache.Instance.level_selected);
        FacebookManager.Instance.LogEventRequestRewardedVideo("10_coins", hasVideo, GameCache.Instance.level_selected);
        if (btn_Watch_Video_Callback != null) btn_Watch_Video_Callback();
    }

    public void BtnShareFbOnClick()
    {
        FacebookManager.Instance.ShareWithFriends(() => {
            GameData.Instance.increaseCoin(50);
            FirebaseManager.Instance.LogEventShareFacebook();
            FacebookManager.Instance.LogEventShareFacebook();
        });
        if (btn_Share_Fb_Callback != null) btn_Share_Fb_Callback();
    }
}
