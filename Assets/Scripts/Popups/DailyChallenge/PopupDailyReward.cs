using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class PopupDailyReward : MonoBehaviour, IPopup
{
    [SerializeField] GameObject[] go_Days;
    private IDayUIDailyRewardSetup[] daysSetup;
    private Action btn_Day_Callback;
    private Action btn_Close_Callback;

    void Awake()
    {
        daysSetup = new IDayUIDailyRewardSetup[go_Days.Length];
        for (int i = 0; i < go_Days.Length; i++)
        {
            daysSetup[i] = go_Days[i].GetComponent<IDayUIDailyRewardSetup>();
        }
    }

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
        for(int i = 0; i < daysSetup.Length; i++)
        {
            int value = GameData.Instance.continueDay - 1;
            if (i < value) daysSetup[i].SetPassedState();
            else if (i == value) daysSetup[i].SetActiveState();
            else daysSetup[i].SetNormalState();
        }
    }

    public void OnDisplayed()
    {
        
    }

    public void OnClosed()
    {
        GetComponent<RectTransform>().gameObject.SetActive(false);
    }

    public void Show(Dictionary<PopupButtonEvent, Action> list_actions, Dictionary<PopupSettingType, object> list_settings)
    {
        Setup();
        btn_Day_Callback = list_actions.ContainsKey(PopupButtonEvent.DayOnDailyRewardPressed) ? list_actions[PopupButtonEvent.DayOnDailyRewardPressed] : null;
        btn_Close_Callback = list_actions.ContainsKey(PopupButtonEvent.ClosePressed) ? list_actions[PopupButtonEvent.ClosePressed] : null;
        GetComponent<Animator>().Play("Show");
    }

    public void Close()
    {
        EventDispatcher.Instance.PostEvent(EventID.OnPopupClosed, this);
        GetComponent<Animator>().Play("Close");
    }

    public void BtnDayOnClick(int k)
    {
        if (k == GameData.Instance.continueDay - 1 && !GameData.Instance.dailyRewardStatus)
        {
            GameData.Instance.dailyRewardStatus = true;
            int[] rewards = {10, 25, 50, 75, 100};
            daysSetup[k].SetPassedState();
            GameData.Instance.increaseCoin(rewards[k]);
            StartCoroutine(WaitForClosePanel());
        }
        btn_Day_Callback?.Invoke();
    }

    public void BtnCloseOnClick()
    {
        if (!GameData.Instance.dailyRewardStatus)
        {
            GameData.Instance.dailyRewardStatus = true;
            int[] rewards = { 10, 25, 50, 75, 100 };
            int k = GameData.Instance.continueDay - 1;
            daysSetup[k].SetPassedState();
            GameData.Instance.increaseCoin(rewards[k]);
            Close();
        }
        btn_Close_Callback?.Invoke();
    }

    private IEnumerator WaitForClosePanel()
    {
        yield return new WaitForSeconds(0.2f);
        Close();
    }

    
}
