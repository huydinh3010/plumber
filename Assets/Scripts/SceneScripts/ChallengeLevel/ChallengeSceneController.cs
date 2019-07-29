using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class ChallengeSceneController : MonoBehaviour
{
    [SerializeField] Image[] image_levels;
    [SerializeField] Image image_pool;
    [SerializeField] Button btn_pool;
    [SerializeField] Sprite[] s_pools;
    [SerializeField] Sprite[] d_levels;
    [SerializeField] Text text_tutorial;
    [SerializeField] Text txtCoins;
    [SerializeField] Text txtPoints;
    private int total;
    private string[] str = {"Complete all levels and get 100 coins!", "Congratulations! You have completed daily challenge.Claim your reward.", "You claimed the reward!" };
    
    // Start is called before the first frame update
    private void Awake()
    {
        LoadSceneManager.Instance.OpenScene();
        EventDispatcher.Instance.RegisterListener(EventID.OnCoinChange, onCoinChange);
        EventDispatcher.Instance.RegisterListener(EventID.OnPointChange, onPointChange);
        txtCoins.text = GameData.Instance.coins.ToString();
        txtPoints.text = GameData.Instance.points.ToString();
        for (int i = 0; i < 8; i++)
        {
            if(GameData.Instance.completed[i] == 1)
            {
                total++;
                image_levels[i].sprite = d_levels[i];
            }
        }
        if(total > 0) image_pool.sprite = s_pools[total - 1];
        if (total == 8 && !GameData.Instance.clampChallengeReward)
        {
            text_tutorial.text = str[1];
        }
        else if(total == 8 && GameData.Instance.clampChallengeReward)
        {
            text_tutorial.text = str[2];
            btn_pool.interactable = false;
        }
        else text_tutorial.text = str[0];
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
        StartCoroutine(coinChangeEffect(txtCoins, Convert.ToInt32(param)));
    }

    private void onPointChange(object param)
    {
        StartCoroutine(coinChangeEffect(txtPoints, Convert.ToInt32(param)));
    }

    IEnumerator coinChangeEffect(Text text, int value)
    {
        int frame = 10;
        int delta = (Mathf.Abs(value) / frame) + 1;
        int text_value = int.Parse(text.text);
        if (value > 0)
        {
            while (value > 0)
            {
                value -= delta;
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
        GameCache.Instance.level_selected = k;
        LoadSceneManager.Instance.LoadScene("GamePlay");
    }

    public void btnPoolOnClick()
    {
        if (total == 8)
        {
            GameData.Instance.increaseCoin(100);
            btn_pool.interactable = false;
            GameData.Instance.clampChallengeReward = true;
            text_tutorial.text = str[2];
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
        PopupManager.Instance.ForceClosePopup();
    }
}
