using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class PopupRate : MonoBehaviour, IPopup
{
    [SerializeField] Text content;
    private Action btn_Close_Callback;
    private Action btn_Not_Now_Callback;
    private Action btn_Rate_Callback;
    private bool isShow;
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_ANDROID
        content.text = "If you like our game, please press the rate button and rate it on Google Play.";
#elif UNITY_IPHONE
        content.text = "If you like our game, please press the rate button and rate it on AppStore.";
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Setup()
    {
        isShow = false;
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
        btn_Not_Now_Callback = list_actions.ContainsKey(PopupButtonEvent.NotNowOnRatePressed) ? list_actions[PopupButtonEvent.NotNowOnRatePressed] : null;
        btn_Rate_Callback = list_actions.ContainsKey(PopupButtonEvent.RateOnRatePressed) ? list_actions[PopupButtonEvent.RateOnRatePressed] : null;
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

    public void BtnNotNowOnClick()
    {
        if (isShow)
        {
            AudioManager.Instance.Play(AudioManager.SoundName.BUTTON);
            Close();
            btn_Not_Now_Callback?.Invoke();
        }
    }

    public void BtnRateOnClick()
    {
        if (isShow)
        {
#if UNITY_ANDROID
            Application.OpenURL("market://details?id=com.waterline.pipeman");
#elif UNITY_IPHONE
            Application.OpenURL("itms-apps://itunes.apple.com/app/com.waterline.pipeman");
#endif
            GameData.Instance.isRateOn = false;
            btn_Rate_Callback?.Invoke();
        }
    }
}
