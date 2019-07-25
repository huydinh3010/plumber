using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class PopupRate : MonoBehaviour
{
    [SerializeField] Button btn_Close;
    [SerializeField] Button btn_Not_Now;
    [SerializeField] Button btn_Rate;
    private Action btn_Close_Callback;
    private Action btn_Not_Now_Callback;
    private Action btn_Rate_Callback;
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
        btn_Rate.interactable = true;
        btn_Not_Now.interactable = true;
    }

    public void Close()
    {
        btn_Close.interactable = false;
        btn_Rate.interactable = false;
        btn_Not_Now.interactable = false;
        animator.Play("Close");
    }

    public void Show(Dictionary<PopupButtonName, Action> list_actions)
    {
        Setup();
        btn_Close_Callback = list_actions.ContainsKey(PopupButtonName.Close) ? list_actions[PopupButtonName.Close] : null;
        btn_Not_Now_Callback = list_actions.ContainsKey(PopupButtonName.NotNowOnRate) ? list_actions[PopupButtonName.NotNowOnRate] : null;
        btn_Rate_Callback = list_actions.ContainsKey(PopupButtonName.RateOnRate) ? list_actions[PopupButtonName.RateOnRate] : null;
        animator.Play("Show");
    }

    public void BtnCloseOnClick()
    {
        Close();
        if (btn_Close_Callback != null) btn_Close_Callback();
    }

    public void BtnNotNowOnClick()
    {
        Close();
        if (btn_Not_Now_Callback != null) btn_Not_Now_Callback();
    }

    public void BtnRateOnClick()
    {
#if UNITY_ANDROID
        Application.OpenURL("market://details?id=" + Application.productName);
#elif UNITY_IPHONE
 Application.OpenURL("itms-apps://itunes.apple.com/app/idYOUR_ID");
#endif
        GameData.Instance.rate = false;
        if(btn_Rate_Callback != null) btn_Rate_Callback();
    }
}
