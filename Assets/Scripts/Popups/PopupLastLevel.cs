﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class PopupLastLevel : MonoBehaviour, IPopup
{
    [SerializeField] Button btn_Close;
    private Action btn_Close_Callback;
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
        //btn_Close.interactable = true;
    }

    public void OnDisplayed()
    {
        btn_Close.enabled = true;
    }

    public void OnClosed()
    {

    }
    private void Close()
    {
        btn_Close.enabled = false;
        EventDispatcher.Instance.PostEvent(EventID.OnPopupClosed, this);
        animator.Play("Close");
    }

    public void Show(Dictionary<PopupButtonEvent, Action> list_actions, Dictionary<PopupSettingType, object> list_settings)
    {
        Setup();
        btn_Close_Callback = list_actions.ContainsKey(PopupButtonEvent.ClosePressed) ? list_actions[PopupButtonEvent.ClosePressed] : null;
        animator.Play("Show");
    }

    public void BtnCloseOnClick()
    {
        Close();
        btn_Close_Callback?.Invoke();
    }
}
