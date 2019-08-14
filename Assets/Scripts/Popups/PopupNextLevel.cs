using System.Collections;
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
    private Action on_Displayed;
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
        middleGroup.alpha = 0f;
        bottomGroup.alpha = 0f;
        middleGroup.interactable = false;
        bottomGroup.interactable = false;
    }

    public void OnDisplayed()
    {
        isShow = true;
        on_Displayed?.Invoke();
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
        float speed = 4f;
        while(target.alpha < 1)
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
        btn_Next_Callback = list_actions.ContainsKey(PopupButtonEvent.NextLevelPressed) ? list_actions[PopupButtonEvent.NextLevelPressed] : null;
        on_Displayed = list_actions.ContainsKey(PopupButtonEvent.OnPopupDisplayed) ? list_actions[PopupButtonEvent.OnPopupDisplayed] : null;
        GetComponent<Animator>().Play("Show");
    }

    public void BtnCloseOnClick()
    {
        if (isShow)
        {
            AudioManager.Instance.Play("button_sound");
            Close();
            btn_Close_Callback?.Invoke();
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
