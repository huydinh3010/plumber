using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PopupAchievement : MonoBehaviour, IPopup
{
    [SerializeField] Button[] btn_Coins;
    [SerializeField] Button btn_Close;
    [SerializeField] Image[] image_Checks;
    [SerializeField] RectTransform content;
    [SerializeField] RectTransform scroll;

    private Action btn_Coin_Callback;
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
        btn_Close.enabled = false;
        float delta = content.rect.height / GameConfig.ACHIEVEMENT_CONDITION_POINT.Length;
        float pos_y = delta * (GameData.Instance.achievementProgress + 1);
        if (pos_y > scroll.rect.height)
        {
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, content.rect.height - scroll.rect.height);
        }
        else
        {
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, 0);
        }
        for (int i = 0; i < GameConfig.ACHIEVEMENT_CONDITION_POINT.Length; i++)
        {
            if (i + 1 <= GameData.Instance.achievementProgress)
            {
                btn_Coins[i].GetComponent<CanvasGroup>().alpha = 1;
                image_Checks[i].enabled = true;
            }
            else
            {
                if (GameData.Instance.points >= GameConfig.ACHIEVEMENT_CONDITION_POINT[i])
                {
                    btn_Coins[i].GetComponent<CanvasGroup>().alpha = 1;
                    image_Checks[i].enabled = false;
                }
                else
                {
                    btn_Coins[i].GetComponent<CanvasGroup>().alpha = 0.5f;
                    image_Checks[i].enabled = false;
                }
            }
        }
    }

    public void OnDisplayed()
    {
        btn_Close.enabled = true;
        for (int i = 0; i < GameConfig.ACHIEVEMENT_CONDITION_POINT.Length; i++)
        {
            if (i + 1 <= GameData.Instance.achievementProgress)
            {
                btn_Coins[i].interactable = false;
            }
            else
            {
                if (GameData.Instance.achievementProgress == i && GameData.Instance.points >= GameConfig.ACHIEVEMENT_CONDITION_POINT[i])
                {
                    btn_Coins[i].interactable = true;
                }
                else
                {
                    btn_Coins[i].interactable = false;
                }
            }
        }
    }

    public void OnClosed()
    {
        GetComponent<RectTransform>().gameObject.SetActive(false);
    }

    public void Show(Dictionary<PopupButtonEvent, Action> list_actions, Dictionary<PopupSettingType, object> list_settings)
    {
        Setup();
        btn_Close_Callback = list_actions.ContainsKey(PopupButtonEvent.ClosePressed) ? list_actions[PopupButtonEvent.ClosePressed] : null;
        btn_Coin_Callback = list_actions.ContainsKey(PopupButtonEvent.CoinOnAchievementPressed) ? list_actions[PopupButtonEvent.CoinOnAchievementPressed] : null;
        GetComponent<Animator>().Play("Show");
    }

    public void Close()
    {
        EventDispatcher.Instance.PostEvent(EventID.OnPopupClosed, this);
        btn_Close.enabled = false;
        GetComponent<Animator>().Play("Close");
    }

    public void BtnCoinOnClick(int k)
    {
        if (k == GameData.Instance.achievementProgress)
        {
            GameData.Instance.increaseCoin(GameConfig.ACHIEVEMENT_COIN_REWARD[k]);
            GameData.Instance.achievementProgress++;
            btn_Coins[k].interactable = false;
            image_Checks[k].enabled = true;
            if (k < 9)
            {
                float delta = content.rect.height / GameConfig.ACHIEVEMENT_CONDITION_POINT.Length;
                StartCoroutine(ScrollNextItem(delta * (k + 1), () => {
                    if (GameData.Instance.points >= GameConfig.ACHIEVEMENT_CONDITION_POINT[k + 1])
                        btn_Coins[k + 1].interactable = true; }));
            }
        }
        btn_Coin_Callback?.Invoke();
    }

    public void BtnCloseOnClick()
    {
        Close();
        btn_Close_Callback?.Invoke();
    }

    IEnumerator ScrollNextItem(float target_pos_y, Action end_Callback = null)
    {
        float offset = content.rect.height - scroll.rect.height;
        target_pos_y = target_pos_y < offset ? target_pos_y : offset;
        float delta = target_pos_y - content.anchoredPosition.y;
        float speed = 5 * delta;
        while (delta > 0)
        {
            delta -= speed * Time.deltaTime;
            content.anchoredPosition += new Vector2(0, speed * Time.deltaTime + (delta < 0 ? delta : 0));
            yield return 0;
        }
        end_Callback?.Invoke();
    }
}
