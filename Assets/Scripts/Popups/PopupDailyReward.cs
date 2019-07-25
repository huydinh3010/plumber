using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class PopupDailyReward : MonoBehaviour, IPopup
{
    [SerializeField] Button[] btn_Days;
    [SerializeField] Sprite[] sp_Days_Active;
    [SerializeField] Sprite[] sp_Days_Passed;
    private Action btn_Day_Callback;
    private int enable_index;
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
        int i;
        for (i = 0; i < GameData.Instance.continueDay - 1; i++)
        {
            btn_Days[i].GetComponent<Image>().sprite = sp_Days_Passed[i];
        }
        btn_Days[i].GetComponent<Image>().sprite = sp_Days_Active[i];
        enable_index = i;
    }

    public void OnDisplayed()
    {
        btn_Days[enable_index].enabled = true;
    }

    public void OnClosed()
    {
        
    }

    public void Show(Dictionary<PopupButtonEvent, Action> list_actions, Dictionary<PopupSettingType, object> list_settings)
    {
        Setup();
        btn_Day_Callback = list_actions.ContainsKey(PopupButtonEvent.DayOnDailyRewardPressed) ? list_actions[PopupButtonEvent.DayOnDailyRewardPressed] : null;
        animator.Play("Show");
    }

    private void Close()
    {
        EventDispatcher.Instance.PostEvent(EventID.OnPopupClosed, this);
        animator.Play("Close");
    }

    public void BtnDayOnClick(int k)
    {
        if (k == GameData.Instance.continueDay - 1)
        {
            int[] rewards = {10, 25, 50, 75, 100};
            btn_Days[k].GetComponent<Image>().sprite = sp_Days_Passed[k];
            GameData.Instance.increaseCoin(rewards[k]);
            btn_Days[k].enabled = false;
            GameData.Instance.clampDailyReward = true;
            StartCoroutine(WaitForClosePanel());
        }
        btn_Day_Callback?.Invoke();
    }

    private IEnumerator WaitForClosePanel()
    {
        yield return new WaitForSeconds(0.3f);
        Close();
    }

    
}
