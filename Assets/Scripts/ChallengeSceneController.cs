using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeSceneController : MonoBehaviour
{
    public SceneController sceneController;
    public Image[] image_levels;
    public Image image_pool;
    public Button btn_pool;
    public Sprite[] s_pools;
    public Sprite[] d_levels;
    public Text text_tutorial;
    public Text txtCoins;
    private int total;
    private string[] str = {"Complete all levels and get 100 coins!", "Congratulations! You have completed daily challenge.Claim your reward.", "You clamped the reward!" };
    // Start is called before the first frame update
    private void Awake()
    {
        EventDispatcher.Instance.RegisterListener(EventID.OnCoinChange, onCoinChange);
        sceneController.openScene();
        for(int i = 0; i < 8; i++)
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
        txtCoins.text = GameData.Instance.coins.ToString();
    }

    void Start()
    {
        
    }

    public void btnBackOnClick()
    {
        sceneController.loadScene("MainMenu");
    }

    public void btnAddCoinOnClick()
    {

    }

    private void onCoinChange(object param)
    {
        txtCoins.text = GameData.Instance.coins.ToString();
    }

    public void btnLevelOnClick(int k)
    {
        GameCache.Instance.mode = 2;
        GameCache.Instance.level_selected = k;
        sceneController.loadScene("GamePlay");
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
        //onClickGameObject();
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveListener(EventID.OnCoinChange, onCoinChange);
    }
}
