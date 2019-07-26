using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class PopupPlayServices : MonoBehaviour, IPopup
{
    [SerializeField] Button btn_Close;
    private Action btn_Close_Callback;
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
    }

    public void OnClosed()
    {
        GetComponent<RectTransform>().gameObject.SetActive(false);
    }

    public void Show(Dictionary<PopupButtonEvent, Action> list_actions, Dictionary<PopupSettingType, object> list_settings)
    {
        Setup();
        btn_Close_Callback = list_actions.ContainsKey(PopupButtonEvent.ClosePressed) ? list_actions[PopupButtonEvent.ClosePressed] : null;
        GetComponent<Animator>().Play("Show");
    }

    public void Close()
    {
        btn_Close.enabled = false;
        EventDispatcher.Instance.PostEvent(EventID.OnPopupClosed, this);
        GetComponent<Animator>().Play("Close");
    }

    public void BtnCloseOnClick()
    {
        Close();
        btn_Close_Callback?.Invoke();
    }
}
