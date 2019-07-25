using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class PopupDailyReward : MonoBehaviour
{
    [SerializeField] Button[] btn_Days;
    [SerializeField] Sprite[] sp_Days_Active;
    [SerializeField] Sprite[] sp_Days_Passed;
    private Action btn_Day_Callback;
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
        int i;
        for (i = 0; i < GameData.Instance.continueDay - 1; i++)
        {
            btn_Days[i].GetComponent<Image>().sprite = sp_Days_Passed[i];
        }
        btn_Days[i].GetComponent<Image>().sprite = sp_Days_Active[i];
        btn_Days[i].enabled = true;
    }

    public void Show(Dictionary<PopupButtonName, Action> list_actions)
    {
        Setup();
        btn_Day_Callback = list_actions.ContainsKey(PopupButtonName.DayOnDailyReward) ? list_actions[PopupButtonName.DayOnDailyReward] : null;
        GetComponent<Animator>().Play("Show");
    }

    public void Close()
    {
        GetComponent<Animator>().Play("Close");
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
        if(btn_Day_Callback != null) btn_Day_Callback();
    }

    private IEnumerator WaitForClosePanel()
    {
        yield return new WaitForSeconds(0.3f);
        Close();
    }
}
