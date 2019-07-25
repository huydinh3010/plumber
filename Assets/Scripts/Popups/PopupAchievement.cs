using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PopupAchievement : MonoBehaviour
{
    [SerializeField] Button[] btn_Coins;
    [SerializeField] Button btn_Close;
    [SerializeField] Image[] image_Checks;
    [SerializeField] RectTransform content;
    private Action btn_Coin_Callback;
    private Action btn_Close_Callback;
    private Animator animator;
    private int[] achm_points = { 50, 100, 500, 1000, 2000, 5000, 10000, 20000, 50000, 100000 };
    private int[] achm_coins_reward = { 10, 20, 50, 100, 200, 500, 1000, 2000, 5000, 10000 };

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
        float delta = content.rect.height / achm_points.Length;
        content.anchoredPosition = new Vector2(content.anchoredPosition.x, delta * GameData.Instance.achievement_progress);
        for (int i = 0; i < achm_points.Length; i++)
        {
            if (i + 1 <= GameData.Instance.achievement_progress)
            {
                btn_Coins[i].interactable = false;
                btn_Coins[i].GetComponent<CanvasGroup>().alpha = 1;
                image_Checks[i].enabled = true;

            }
            else
            {
                if (GameData.Instance.points >= achm_points[i])
                {
                    btn_Coins[i].interactable = true;
                    btn_Coins[i].GetComponent<CanvasGroup>().alpha = 1;
                    image_Checks[i].enabled = false;
                }
                else
                {
                    btn_Coins[i].interactable = false;
                    btn_Coins[i].GetComponent<CanvasGroup>().alpha = 0.5f;
                    image_Checks[i].enabled = false;
                }
            }
        }
    }

    public void Show(Dictionary<PopupButtonName, Action> list_actions)
    {
        Setup();
        btn_Close_Callback = list_actions.ContainsKey(PopupButtonName.Close) ? list_actions[PopupButtonName.Close] : null;
        btn_Coin_Callback = list_actions.ContainsKey(PopupButtonName.CoinOnAchievement) ? list_actions[PopupButtonName.CoinOnAchievement] : null;
        animator.Play("Show");
    }

    public void Close()
    {
        animator.Play("Close");
    }

    public void BtnCoinOnClick(int k)
    {
        if(k == GameData.Instance.achievement_progress)
        {
            GameData.Instance.increaseCoin(achm_coins_reward[k]);
            GameData.Instance.achievement_progress++;
            btn_Coins[k].interactable = false;
            image_Checks[k].enabled = true;
        }
        if (btn_Coin_Callback != null) btn_Coin_Callback();
    }

    public void BtnCloseOnClick()
    {
        Close();
        if (btn_Close_Callback != null) btn_Close_Callback();
    }
}
