using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class PopupPassLevel : MonoBehaviour
{
    [SerializeField] Button btn_Close;
    [SerializeField] Button btn_Next;
    [SerializeField] Button btn_Watch_Video;
    [SerializeField] Image image_Coin;
    [SerializeField] Image image_Point;
    [SerializeField] Sprite[] sp_Coins;
    [SerializeField] Sprite[] sp_Points;
    private Action btn_Close_Callback;
    private Action btn_Watch_Video_Callback;
    private Action btn_Next_Callback;
    private Animator animator;
    private int value;
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
        btn_Next.interactable = true;
        btn_Watch_Video.interactable = true;
    }

    public void Close()
    {
        btn_Close.interactable = false;
        btn_Next.interactable = false;
        btn_Watch_Video.interactable = false;
        animator.Play("Close");
    }

    public void Show(Dictionary<PopupButtonName, Action> list_actions, Dictionary<PopupSettingType, object> list_settings)
    {
        Setup();
        btn_Close_Callback = list_actions.ContainsKey(PopupButtonName.Close) ? list_actions[PopupButtonName.Close] : null;
        btn_Watch_Video_Callback = list_actions.ContainsKey(PopupButtonName.WatchVideo10TimesCoin) ? list_actions[PopupButtonName.WatchVideo10TimesCoin] : null;
        btn_Next_Callback = list_actions.ContainsKey(PopupButtonName.NextLevel) ? list_actions[PopupButtonName.NextLevel] : null;
        value = list_settings.ContainsKey(PopupSettingType.PassLevelImage) ? Convert.ToInt32(list_settings[PopupSettingType.PassLevelImage]) : 0;
        if(value >= 1 && value <= 3)
        {
            image_Coin.sprite = sp_Coins[value - 1];
            image_Point.sprite = sp_Points[value - 1];
        }
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
            GameData.Instance.increaseCoin(10 * value);
            btn_Watch_Video.interactable = false;
            Debug.Log("Rewarded 10 times coins");
        });
        FirebaseManager.Instance.LogEventRequestRewardedVideo("x10_coins", hasVideo, GameCache.Instance.level_selected);
        FacebookManager.Instance.LogEventRequestRewardedVideo("x10_coins", hasVideo, GameCache.Instance.level_selected);
        if (btn_Watch_Video_Callback != null) btn_Watch_Video_Callback();
    }

    public void BtnNextOnClick()
    {
        Close();
        if (btn_Next_Callback != null) btn_Next_Callback();
    }
}
