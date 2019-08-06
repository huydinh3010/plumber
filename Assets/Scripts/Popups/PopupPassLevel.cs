using System.Collections;
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
    private Action btn_Close_Callback;
    private Action btn_Watch_Video_Callback;
    private Action btn_Next_Callback;
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
        btn_Watch_Video.interactable = true;
        middleGroup.interactable = false;
        bottomGroup.interactable = false;
        middleGroup.alpha = 0f;
        bottomGroup.alpha = 0f;
    }

    public void OnDisplayed()
    {
        isShow = true;
        StartCoroutine(fadeInEffect(middleGroup, () =>
        {
            StartCoroutine(fadeInEffect(bottomGroup, () =>
            {
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
        float speed = 1f;
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
        value = list_settings.ContainsKey(PopupSettingType.PassLevelImageType) ? Convert.ToInt32(list_settings[PopupSettingType.PassLevelImageType]) : 0;
        if(value >= 1 && value <= 3)
        {
            text_Coin.text = "+" + GameConfig.PASS_LEVEL_COIN_REWARD[value-1].ToString();
            text_Point.text = "+" + GameConfig.PASS_LEVEL_POINT_REWARD[value-1].ToString();
        }
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
                int reward = 2 * GameConfig.PASS_LEVEL_COIN_REWARD[value - 1];
                GameData.Instance.increaseCoin(reward);
                btn_Watch_Video.interactable = false;
                PopupManager.Instance.ShowNotification("You are rewarded " + reward + " coins!", coin, 1.5f);
            });
            FirebaseManager.Instance.LogEventRequestRewardedVideo("x2_coins", hasVideo, GameCache.Instance.levelSelected);
            FacebookManager.Instance.LogEventRequestRewardedVideo("x2_coins", hasVideo, GameCache.Instance.levelSelected);
            btn_Watch_Video_Callback?.Invoke();
        }
    }

    public void BtnNextOnClick()
    {
        if (isShow)
        {
            Close();
            btn_Next_Callback?.Invoke();
        }
    }
}
