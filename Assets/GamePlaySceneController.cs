using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GamePlaySceneController : MonoBehaviour
{
    public SceneController sceneController;
    public GameController game;
    public GameObject panelRate;
    public GameObject panelAddCoin;
    public GameObject panelPassedLevel;
    public GameObject panelNextLevel;
    public Button btnSound;
    public Button btnRemove;
    public Button btnConstruct;
    //public Button btnWatchVideoMoreCoin;
    public Button btnWatchVideo10TimesCoin;
    public Text txtCoins;
    public Text txtPoints;
    public Text txtLevel;
    public Sprite[] s_coins;
    public Sprite[] s_points;
    public Sprite[] s_sounds;
    public GameObject hand;

   
    private bool gameover;
    private bool animPlaying;
    private bool panelShowing;
    private int construct_part;
    private bool tutorial;
    private bool en_rmbtn;
    private void Awake()
    {
        
        sceneController.openScene();
        EventDispatcher.Instance.RegisterListener(EventID.OnCoinChange, onCoinChange);
        EventDispatcher.Instance.RegisterListener(EventID.OnPointChange, onPointChange);
        EventDispatcher.Instance.RegisterListener(EventID.OnLevelSelectChange, onLevelSelectChange);
        EventDispatcher.Instance.RegisterListener(EventID.PipeAnimationEnd, endGame);

        if (GameCache.Instance.mode == 0 || (GameCache.Instance.level_selected == 1 && GameCache.Instance.mode == 1)) // help
        {
            newGameLevel(GetComponent<HelpLevelController>());
            tutorial = true;
        }
        else if (GameCache.Instance.mode == 1) // simple
        {
            newGameLevel(GetComponent<SimpleModeController>());
        }
        else if (GameCache.Instance.mode == 2) // challenge
        {
            //game = new ChallengeModeController();
            newGameLevel(GetComponent<ChallengeModeController>());
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        checkPipesClicked();
    }

    public void checkPipesClicked()
    {
        if (Input.GetMouseButtonDown(0) && !animPlaying && !gameover && !panelShowing)
        {
            Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D raycastHit = Physics2D.Raycast(position, Vector2.zero);
            if (raycastHit.collider != null)
            {
                if (raycastHit.collider.tag == "pipe")
                {
                    GameObject go = raycastHit.collider.gameObject;
                    StartCoroutine(game.rotatePipe(go, 1, game.rotate_speed));
                }
                else if (raycastHit.collider.tag == "valve")
                {
                    List<GameObject> list_results;
                    List<int> list_ds;
                    if (game.checkPipes(out list_results, out list_ds)) playAnimation(list_results, list_ds);
                }
            }
        }
    }

    private void newGameLevel(GameController game)
    {
        // UI
        this.game = game;
        btnRemove.interactable = GameData.Instance.coins >= 50;
        btnConstruct.interactable = GameData.Instance.coins >= 25;
        if (GameData.Instance.sound_on) btnSound.GetComponent<Image>().sprite = s_sounds[1];
        else btnSound.GetComponent<Image>().sprite = s_sounds[0];
        txtCoins.text = GameData.Instance.coins.ToString();
        txtPoints.text = GameData.Instance.points.ToString();
        txtLevel.text = "Level " + GameCache.Instance.level_selected.ToString();
        // Game logic
        gameover = animPlaying = tutorial = false;
        en_rmbtn = true;
        construct_part = 0;
        game.loadLevelData();
        game.setupLevel();
    }

    private void playAnimation(List<GameObject> list_results, List<int> list_ds)
    {
        animPlaying = true;
        btnConstruct.interactable = false;
        btnRemove.interactable = false;
        game.stopDecreaseTime();
        for (int i = 0; i < list_results.Count - 1; i++)
        {

            PipeProperties pp = list_results[i].GetComponent<PipeProperties>();
            pp.next[pp.i] = list_results[i + 1];
            pp.next_in[pp.i] = list_ds[i + 1];
            pp.i++;
        }
        for (int i = 0; i < list_results.Count - 1; i++)
        {
            list_results[i].GetComponent<PipeProperties>().i = 0;
            list_results[i + 1].GetComponent<Animator>().Play("Idle");
        }
        list_results[0].GetComponentsInChildren<Animator>()[0].Play("valve");
        list_results[0].GetComponentsInChildren<Animator>()[1].Play("valvebg", -1, 1 - game.getStar() / 3f);
    }

    private void endGame(object param)
    {
        gameover = true;
        if (GameCache.Instance.mode == 0)
        {
            sceneController.loadScene("MainMenu");
        }
        else if (GameCache.Instance.mode == 1)
        {
            if (GameData.Instance.level_stars.Count > GameCache.Instance.level_selected)
            {
                int c_star = GameData.Instance.level_stars[GameCache.Instance.level_selected - 1];
                int n_star = game.getStar();
                if (n_star > c_star) GameData.Instance.level_stars[GameCache.Instance.level_selected - 1] = n_star;
                panelNextLevel.GetComponent<Animator>().Play("Show");
                panelShowing = true;
            }
            else
            {
                int star = game.getStar();
                GameData.Instance.level_stars[GameCache.Instance.level_selected - 1] = star;
                GameData.Instance.increaseCoin(star);
                GameData.Instance.increasePoint(star * 10);
                panelPassedLevel.transform.Find("Coins").GetComponent<Image>().sprite = s_coins[star - 1];
                panelPassedLevel.transform.Find("Points").GetComponent<Image>().sprite = s_points[star - 1];
                if (GameData.Instance.unlock_level < 560)
                {
                    GameData.Instance.level_stars.Add(0);
                    GameData.Instance.unlock_level++;
                }
                //else
                //{
                //    // vuot qua 560 level

                //}
                btnWatchVideo10TimesCoin.interactable = true;
                panelPassedLevel.GetComponent<Animator>().Play("Show");
                panelShowing = true;
            }
        }
        else if (GameCache.Instance.mode == 2)
        {
            if (GameData.Instance.completed[GameCache.Instance.level_selected - 1] == 1)
            {
                panelNextLevel.GetComponent<Animator>().Play("Show");
                panelShowing = true;
            }
            else
            {
                int star = game.getStar();
                GameData.Instance.completed[GameCache.Instance.level_selected - 1] = 1;
                GameData.Instance.increaseCoin(star);
                GameData.Instance.increasePoint(star * 10);
                panelPassedLevel.transform.Find("Coins").GetComponent<Image>().sprite = s_coins[star - 1];
                panelPassedLevel.transform.Find("Points").GetComponent<Image>().sprite = s_points[star - 1];
                panelPassedLevel.GetComponent<Animator>().Play("Show");
                panelShowing = true;
            }
        }
    }

    private void onLevelSelectChange(object param)
    {
        txtLevel.text = "Level " + GameCache.Instance.level_selected.ToString();
    }

    private void onCoinChange(object param)
    {
        txtCoins.text = GameData.Instance.coins.ToString();
        btnRemove.interactable = GameData.Instance.coins >= 50;
        btnConstruct.interactable = GameData.Instance.coins >= 25;
        if (!en_rmbtn) btnRemove.interactable = false;
        if (construct_part < 0) btnConstruct.interactable = false;
    }

    private void onPointChange(object param)
    {
        txtPoints.text = GameData.Instance.points.ToString();
    }

    private void nextLevel()
    {
        if (GameCache.Instance.mode == 1)
        {
            if (GameCache.Instance.level_selected == 560)
            {
                sceneController.loadScene("SimpleLevel");
            }
            else
            {
                if (GameCache.Instance.canShowAds())
                {
                    Debug.Log("Ads showing"); // show Ads
                    AdManager.Instance.ShowInterstitial();
                }
                else if (GameCache.Instance.canShowRatePanel())
                {
                    panelRate.GetComponent<Animator>().Play("Show");
                    panelShowing = true;
                }
                else
                {
                    GameCache.Instance.level_selected++;
                    game.destroy();
                    newGameLevel(GetComponent<SimpleModeController>());
                }
            }
        }
        else if (GameCache.Instance.mode == 2)
        {
            if (GameCache.Instance.level_selected == 8)
            {
                sceneController.loadScene("ChallengeLevel");
            }
            else
            {
                if (GameCache.Instance.canShowAds())
                {
                    Debug.Log("Ads showing"); // show Ads
                    AdManager.Instance.ShowInterstitial();
                }
                else if (GameCache.Instance.canShowRatePanel())
                {
                    panelRate.GetComponent<Animator>().Play("Show");
                    panelShowing = true;
                }
                else
                {
                    GameCache.Instance.level_selected++;
                    game.destroy();
                    newGameLevel(GetComponent<ChallengeModeController>());
                }
            }
        }
    }

    public void btnSoundOnClick()
    {
        GameData.Instance.sound_on = !GameData.Instance.sound_on;
        if (GameData.Instance.sound_on) btnSound.GetComponent<Image>().sprite = s_sounds[1];
        else btnSound.GetComponent<Image>().sprite = s_sounds[0];
    }

    public void btnRemoveOnClick()
    {
        if (!tutorial && GameData.Instance.decreaseCoin(50))
        {
            game.removeIncorrectPipes();
            btnRemove.interactable = false;
            en_rmbtn = true;
        }
    }

    public void btnConstructOnClick()
    {
        if (!tutorial && GameData.Instance.decreaseCoin(25))
        {
            if (game.constructPipes(construct_part++))
            {
                btnConstruct.interactable = false;
                construct_part = -1;
            }
        }
    }

    public void btnAddCoinOnClick()
    {
        if (!tutorial && !animPlaying)
        {
            panelAddCoin.GetComponent<Animator>().Play("Show");
            panelShowing = true;
        }
    }

    public void btnBackOnClick()
    {
        switch (GameCache.Instance.mode)
        {
            case 0:
                sceneController.loadScene("MainMenu");
                break;
            case 1:
                sceneController.loadScene("SimpleLevel");
                break;
            case 2:
                sceneController.loadScene("ChallengeLevel");
                break;
        }

    }
    public void btnCloseOnPanelRateOnClick()
    {
        panelRate.GetComponent<Animator>().Play("Close");
        panelShowing = false;
        GameCache.Instance.level_selected++;
        game.destroy();
        if (GameCache.Instance.mode == 1) newGameLevel(GetComponent<SimpleModeController>());
        else if (GameCache.Instance.mode == 2) newGameLevel(GetComponent<ChallengeModeController>());
    }

    public void btnClose1OnPanelOnClick(GameObject target)
    {
        target.GetComponent<Animator>().Play("Close");
        panelShowing = false;
    }

    public void btnClose2OnPanelOnClick(GameObject target)
    {
        target.GetComponent<Animator>().Play("Close");
        panelShowing = false;
        switch (GameCache.Instance.mode)
        {
            case 1:
                sceneController.loadScene("SimpleLevel");
                break;
            case 2:
                sceneController.loadScene("ChallengeLevel");
                break;
        }
    }

    public void OnAdsInterstitialClose(object param)
    {
        GameCache.Instance.level_selected++;
        game.destroy();
        if (GameCache.Instance.mode == 1) newGameLevel(GetComponent<SimpleModeController>());
        else if (GameCache.Instance.mode == 2) newGameLevel(GetComponent<ChallengeModeController>());
    }
    
    public void btnWatchVideo10TimesCoinOnPanelOnClick()
    {
        AdManager.Instance.ShowRewardVideo(() => {
            GameData.Instance.increaseCoin(10 * game.getStar());
            
        });
    }

    public void btnWatchVideoMoreCoinOnPanelOnClick()
    {

        AdManager.Instance.ShowRewardVideo(() => {
            GameData.Instance.increaseCoin(10);
            panelRate.GetComponent<Animator>().Play("Show");
            Debug.Log("Rewarded 10 coins");
        });
    }

    public void btnShareFbOnPanelOnClick()
    {

    }

    public void btnNextOnPanelOnClick(GameObject target)
    {
        target.GetComponent<Animator>().Play("Close");
        nextLevel();
        panelShowing = false;
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveListener(EventID.OnCoinChange, onCoinChange);
        EventDispatcher.Instance.RemoveListener(EventID.OnLevelSelectChange, onLevelSelectChange);
        EventDispatcher.Instance.RemoveListener(EventID.OnPointChange, onPointChange);
        EventDispatcher.Instance.RemoveListener(EventID.PipeAnimationEnd, endGame);
    }
}