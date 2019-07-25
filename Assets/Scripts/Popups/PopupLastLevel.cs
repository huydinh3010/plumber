using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class PopupLastLevel : MonoBehaviour
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
        btn_Close.interactable = true;
    }

    public void Close()
    {
        btn_Close.interactable = false;
        animator.Play("Close");
    }

    public void Show(Dictionary<PopupButtonName, Action> list_actions)
    {
        Setup();
        btn_Close_Callback = list_actions.ContainsKey(PopupButtonName.Close) ? list_actions[PopupButtonName.Close] : null;
        animator.Play("Show");
    }

    public void BtnCloseOnClick()
    {
        Close();
        if (btn_Close_Callback != null) btn_Close_Callback();
    }
}
