using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class PopupRate : MonoBehaviour, IPopup
{
    [SerializeField] Button btn_Close;
    [SerializeField] Button btn_Not_Now;
    [SerializeField] Button btn_Rate;
    private Action btn_Close_Callback;
    private Action btn_Not_Now_Callback;
    private Action btn_Rate_Callback;
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
        btn_Rate.enabled = true;
        btn_Not_Now.enabled = true;
    }

    public void OnClosed()
    {
        GetComponent<RectTransform>().gameObject.SetActive(false);
    }

    public void Close()
    {
        btn_Close.enabled = false;
        btn_Rate.enabled = false;
        btn_Not_Now.enabled = false;
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
        Close();
        btn_Close_Callback?.Invoke();
    }

    public void BtnNotNowOnClick()
    {
        Close();
        btn_Not_Now_Callback?.Invoke();
    }

    public void BtnRateOnClick()
    {
#if UNITY_ANDROID
        Application.OpenURL("market://details?id=" + Application.productName);
#elif UNITY_IPHONE
 Application.OpenURL("itms-apps://itunes.apple.com/app/idYOUR_ID");
#endif
        GameData.Instance.isRateOn = false;
        btn_Rate_Callback?.Invoke();
    }
}
