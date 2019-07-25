using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class PopupNextLevel : MonoBehaviour
{
    [SerializeField] Button btn_Close;
    [SerializeField] Button btn_Next;
    private Action btn_Close_Callback;
    private Action btn_Next_Callback;
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
        btn_Next.interactable = true;
    }

    public void Close()
    {
        btn_Close.interactable = false;
        btn_Next.interactable = false;
        animator.Play("Close");
    }

    public void Show(Dictionary<PopupButtonName, Action> list_actions)
    {
        Setup();
        btn_Close_Callback = list_actions.ContainsKey(PopupButtonName.Close) ? list_actions[PopupButtonName.Close] : null;
        btn_Next_Callback = list_actions.ContainsKey(PopupButtonName.NextLevel) ? list_actions[PopupButtonName.NextLevel] : null;
        animator.Play("Show");
    }

    public void BtnCloseOnClick()
    {
        Close();
        if (btn_Close_Callback != null) btn_Close_Callback();
    }

    public void BtnNextOnClick()
    {
        Close();
        if (btn_Next_Callback != null) btn_Next_Callback();
    }
}
