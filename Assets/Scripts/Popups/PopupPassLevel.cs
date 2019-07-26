using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class PopupPassLevel : MonoBehaviour, IPopup
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
    private int value;
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
        btn_Next.enabled = true;
        btn_Watch_Video.enabled = true;
    }

    public void OnClosed()
    {
        GetComponent<RectTransform>().gameObject.SetActive(false);
    }

    public void Close()
    {
        btn_Close.enabled = false;
        btn_Next.enabled = false;
        btn_Watch_Video.enabled = false;
        EventDispatcher.Instance.PostEvent(EventID.OnPopupClosed, this);
        GetComponent<Animator>().Play("Close");
    }

    public void Show(Dictionary<PopupButtonEvent, Action> list_actions, Dictionary<PopupSettingType, object> list_settings)
    {
        Setup();
        btn_Close_Callback = list_actions.ContainsKey(PopupButtonEvent.ClosePressed) ? list_actions[PopupButtonEvent.ClosePressed] : null;
        btn_Watch_Video_Callback = list_actions.ContainsKey(PopupButtonEvent.WatchVideo10TimesCoinPressed) ? list_actions[PopupButtonEvent.WatchVideo10TimesCoinPressed] : null;
        btn_Next_Callback = list_actions.ContainsKey(PopupButtonEvent.NextLevelPressed) ? list_actions[PopupButtonEvent.NextLevelPressed] : null;
        value = list_settings.ContainsKey(PopupSettingType.PassLevelImageType) ? Convert.ToInt32(list_settings[PopupSettingType.PassLevelImageType]) : 0;
        if(value >= 1 && value <= 3)
        {
            image_Coin.sprite = sp_Coins[value - 1];
            image_Point.sprite = sp_Points[value - 1];
        }
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
            GameData.Instance.increaseCoin(10 * value);
            btn_Watch_Video.interactable = false;
            Debug.Log("Rewarded 10 times coins");
        });
        FirebaseManager.Instance.LogEventRequestRewardedVideo("x10_coins", hasVideo, GameCache.Instance.level_selected);
        FacebookManager.Instance.LogEventRequestRewardedVideo("x10_coins", hasVideo, GameCache.Instance.level_selected);
        btn_Watch_Video_Callback?.Invoke();
    }

    public void BtnNextOnClick()
    {
        Close();
        btn_Next_Callback?.Invoke();
    }
}
