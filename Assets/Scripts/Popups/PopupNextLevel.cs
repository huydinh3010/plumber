﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class PopupNextLevel : MonoBehaviour, IPopup
{
    [SerializeField] Button btn_Close;
    [SerializeField] Button btn_Next;
    [SerializeField] CanvasGroup middleGroup;
    [SerializeField] CanvasGroup bottomGroup;
    private Action btn_Close_Callback;
    private Action btn_Next_Callback;
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
        btn_Close.enabled = false;
        btn_Next.enabled = false;
        middleGroup.alpha = 0f;
        bottomGroup.alpha = 0f;
        middleGroup.interactable = false;
        bottomGroup.interactable = false;
    }

    public void OnDisplayed()
    {
        btn_Close.enabled = true;
        btn_Next.enabled = true;

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
        while(target.alpha < 1)
        {
            target.alpha = (target.alpha + speed * Time.deltaTime) < 1 ? target.alpha + speed * Time.deltaTime : 1;
            yield return null; 
        }
        EndCallback();
    }

    public void Close()
    {
        btn_Close.enabled = false;
        btn_Next.enabled = false;
        EventDispatcher.Instance.PostEvent(EventID.OnPopupClosed, this);
        GetComponent<Animator>().Play("Close");
    }

    public void Show(Dictionary<PopupButtonEvent, Action> list_actions, Dictionary<PopupSettingType, object> list_settings)
    {
        Setup();
        btn_Close_Callback = list_actions.ContainsKey(PopupButtonEvent.ClosePressed) ? list_actions[PopupButtonEvent.ClosePressed] : null;
        btn_Next_Callback = list_actions.ContainsKey(PopupButtonEvent.NextLevelPressed) ? list_actions[PopupButtonEvent.NextLevelPressed] : null;
        GetComponent<Animator>().Play("Show");
    }

    public void BtnCloseOnClick()
    {
        Close();
        btn_Close_Callback?.Invoke();
    }

    public void BtnNextOnClick()
    {
        Close();
        btn_Next_Callback?.Invoke();
    }
}
