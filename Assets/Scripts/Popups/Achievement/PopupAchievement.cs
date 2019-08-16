using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PopupAchievement : MonoBehaviour, IPopup
{
    [SerializeField] GameObject[] go_Items;
    [SerializeField] RectTransform content;
    [SerializeField] RectTransform scroll;
    [SerializeField] Sprite coin;
    private ItemAchievementSetup[] itemsSetup;
    private Action btn_Coin_Callback;
    private Action btn_Close_Callback;
    private bool isShow;

    void Awake()
    {
        itemsSetup = new ItemAchievementSetup[go_Items.Length];
        for (int i = 0; i < go_Items.Length; i++)
        {
            itemsSetup[i] = go_Items[i].GetComponent<ItemAchievementSetup>();
            itemsSetup[i].setup(GameConfig.ACHIEVEMENT_CONDITION_POINT[i], GameConfig.ACHIEVEMENT_COIN_REWARD[i]);
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
        isShow = false;
        float delta = content.rect.height / go_Items.Length;
        float pos_y = delta * (GameData.Instance.achievementProgress - 1);
        pos_y = pos_y < 0 ? 0 : pos_y;
        float offset = content.rect.height - scroll.rect.height;
        pos_y = pos_y < offset ? pos_y : offset;
        content.anchoredPosition = new Vector2(0f, pos_y);
        for (int i = 0; i < go_Items.Length; i++)
        {
            if (i + 1 <= GameData.Instance.achievementProgress)
            {
                itemsSetup[i].setPassedState();
            }
            else
            {
                if (GameData.Instance.points >= GameConfig.ACHIEVEMENT_CONDITION_POINT[i])
                {
                    itemsSetup[i].setActiveState();
                }
                else
                {
                    itemsSetup[i].setNormalState();
                }
            }
        }
    }

    public void OnDisplayed()
    {
        isShow = true;
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
        isShow = false;
        EventDispatcher.Instance.PostEvent(EventID.OnPopupClosed, this);
        GetComponent<Animator>().Play("Close");
    }

    public void BtnCoinOnClick(int k)
    {
        if (isShow)
        {
            if (k == GameData.Instance.achievementProgress)
            {
                int reward = GameConfig.ACHIEVEMENT_COIN_REWARD[k];
                GameData.Instance.increaseCoin(reward);
                GameData.Instance.achievementProgress++;
                itemsSetup[k].setPassedState();
                PopupManager.Instance.ShowNotification("You are rewarded " + reward + " coins!", coin, 2f);
                if (k < go_Items.Length - 1 && GameData.Instance.points >= GameConfig.ACHIEVEMENT_CONDITION_POINT[k + 1])
                {
                    float delta = content.rect.height / GameConfig.ACHIEVEMENT_CONDITION_POINT.Length;
                    StartCoroutine(ScrollNextItem(delta * (k + 1)));
                }
            }
            btn_Coin_Callback?.Invoke();
        }
    }

    public void BtnCloseOnClick()
    {
        if (isShow)
        {
            AudioManager.Instance.Play(AudioManager.SoundName.BUTTON);
            Close();
            btn_Close_Callback?.Invoke();
        }
    }

    IEnumerator ScrollNextItem(float target_pos_y)
    {
        float offset = content.rect.height - scroll.rect.height;
        target_pos_y = target_pos_y < offset ? target_pos_y : offset;
        float delta = target_pos_y - content.anchoredPosition.y;
        float speed = 5 * delta;
        while (content.anchoredPosition.y <= target_pos_y)
        {
            delta = target_pos_y - content.anchoredPosition.y;
            content.anchoredPosition += new Vector2(0, speed * Time.deltaTime + (delta < 0 ? delta : 0));
            yield return 0;
        }
    }
}
