using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class ChallengeLevelSceneController : MonoBehaviour
{
    [SerializeField] Image[] imageLevels;
    [SerializeField] Image imageState;
    [SerializeField] Button btnPool;
    [SerializeField] Sprite[] s_States;
    [SerializeField] Sprite d_Level;
    [SerializeField] Text textTutorial;
    [SerializeField] Text textCoin;
    [SerializeField] Text textPoint;
    [SerializeField] Text textCoinReward;
    [SerializeField] GameObject middle;
    [SerializeField] GameObject hand;
    private int total;
    private string[] str = {"Complete all levels and get reward coins!", "Congratulations! You have completed daily challenge.Claim your reward.", "You claimed the reward!" };
    
    // Start is called before the first frame update
    private void Awake()
    {
        LoadSceneManager.Instance.OpenScene();
        EventDispatcher.Instance.RegisterListener(EventID.OnCoinChange, onCoinChange);
        EventDispatcher.Instance.RegisterListener(EventID.OnPointChange, onPointChange);

        float aspect_ratio = (float)Screen.height / Screen.width;
        if (aspect_ratio < 1.5f)
        {
            float scale = 1 - ((1.5f - aspect_ratio));
            middle.GetComponent<RectTransform>().sizeDelta *= scale; 
        }

        textCoin.text = GameData.Instance.coins.ToString();
        textPoint.text = GameData.Instance.points.ToString();
        textCoinReward.text = "+" + GameConfig.DAILY_CHALLENGE_COIN_REWARD.ToString();
        for (int i = 0; i < 8; i++)
        {
            if(GameData.Instance.dailyChallengeProgess[i] == 1)
            {
                total++;
                imageLevels[i].sprite = d_Level;
            }
        }
        if(total > 0) imageState.sprite = s_States[total - 1];
        if (total == 8 && !GameData.Instance.challengeRewardStatus)
        {
            textTutorial.text = str[1];
            hand.SetActive(true);
        }
        else if (total == 8 && GameData.Instance.challengeRewardStatus)
        {
            textTutorial.text = str[2];
            btnPool.interactable = false;
        }
        else
        {
            textTutorial.text = str[0];
        }
    }

    void Start()
    {
        
    }

    public void btnBackOnClick()
    {
        LoadSceneManager.Instance.LoadScene("MainMenu");
    }

    

    private void onCoinChange(object param)
    {
        StartCoroutine(coinChangeEffect(textCoin, Convert.ToInt32(param)));
    }

    private void onPointChange(object param)
    {
        StartCoroutine(coinChangeEffect(textPoint, Convert.ToInt32(param)));
        if (GameCache.Instance.unlockAchievementProgress < GameConfig.ACHIEVEMENT_CONDITION_POINT.Length && GameData.Instance.points >= GameConfig.ACHIEVEMENT_CONDITION_POINT[GameCache.Instance.unlockAchievementProgress])
        {
            PopupManager.Instance.ShowNotification("Unlock achievement. Go back Menu to get " + GameConfig.ACHIEVEMENT_COIN_REWARD[GameCache.Instance.unlockAchievementProgress] + " coins", null, 3f);
            GameCache.Instance.unlockAchievementProgress++;
        }
    }

    IEnumerator coinChangeEffect(Text text, int value)
    {
        int frame = 10;
        int delta = (Mathf.Abs(value) / frame) + 1;
        if (value > 0)
        {
            while (value > 0)
            {
                value -= delta;
                int text_value = int.Parse(text.text);
                if (value < 0) text_value += delta + value;
                else text_value += delta;
                text.text = (text_value).ToString();
                yield return 1;
            }
        }
        else
        {
            while (value < 0)
            {
                value += delta;
                int text_value = int.Parse(text.text);
                if (value > 0) text_value = text_value - delta + value;
                else text_value -= delta;
                text.text = (text_value).ToString();
                yield return 1;
            }
        }
    }

    public void btnLevelOnClick(int k)
    {
        GameCache.Instance.mode = 2;
        GameCache.Instance.levelSelected = k;
        LoadSceneManager.Instance.LoadScene("GamePlay");
    }

    public void btnPoolOnClick()
    {
        if (total == 8)
        {
            GameData.Instance.increaseCoin(GameConfig.DAILY_CHALLENGE_COIN_REWARD);
            btnPool.interactable = false;
            GameData.Instance.challengeRewardStatus = true;
            textTutorial.text = str[2];
            hand.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void btnAddCoinOnClick()
    {
        PopupManager.Instance.ShowPopup(PopupName.AddCoin, null);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveListener(EventID.OnCoinChange, onCoinChange);
        EventDispatcher.Instance.RemoveListener(EventID.OnPointChange, onPointChange);
        try
        {
            PopupManager.Instance.ForceClosePopup();
        }
        catch (Exception e)
        {

        }
    }
}
