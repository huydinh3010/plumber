using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class SimpleLevelSceneController : MonoBehaviour
{
    [SerializeField] GameObject Grid;
    [SerializeField] GameObject OneStarLv;
    [SerializeField] GameObject TwoStarsLv;
    [SerializeField] GameObject ThreeStarsLv;
    [SerializeField] GameObject UnlockLv;
    [SerializeField] GameObject LockLv;
    [SerializeField] GameObject ContentObj;
    [SerializeField] Text txtCoins;
    [SerializeField] Text txtPoints;
    private void Awake()
    {
        LoadSceneManager.Instance.OpenScene();
        EventDispatcher.Instance.RegisterListener(EventID.OnCoinChange, OnCoinChange);
        EventDispatcher.Instance.RegisterListener(EventID.OnPointChange, OnPointChange);

        txtCoins.text = GameData.Instance.coins.ToString();
        txtPoints.text = GameData.Instance.points.ToString();
        for (int p = 0; p < GameConfig.NUMBER_OF_SIMPLE_LEVEL / 16; p++) 
        {
            GameObject goGridClone = Instantiate(Grid, Vector3.zero, Quaternion.identity, ContentObj.transform);
            goGridClone.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(-18700 + 1100 * p, 0, 0);
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    GameObject go = null;
                    if (p * 16 + i * 4 + j < GameData.Instance.listLevelStars.Count)
                    {
                        int level = p * 16 + i * 4 + j + 1;
                        switch (GameData.Instance.listLevelStars[p * 16 + i * 4 + j])
                        {
                            case 0:
                                go = Instantiate(UnlockLv, new Vector3(), Quaternion.identity, goGridClone.transform);
                                break;
                            case 1:
                                go = Instantiate(OneStarLv, new Vector3(), Quaternion.identity, goGridClone.transform);
                                break;
                            case 2:
                                go = Instantiate(TwoStarsLv, new Vector3(), Quaternion.identity, goGridClone.transform);
                                break;
                            case 3:
                                go = Instantiate(ThreeStarsLv, new Vector3(), Quaternion.identity, goGridClone.transform);
                                break;
                            default:
                                break;
                        }
                        go.GetComponentInChildren<Text>().text = level.ToString();
                        go.GetComponent<Button>().onClick.AddListener(() => { BtnLevelOnScrollViewOnClick(level); });
                    }
                    else
                    {
                        go = Instantiate(LockLv, new Vector3(), Quaternion.identity, goGridClone.transform);
                    }
                    go.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
                }
            }
        }
        if (GameCache.Instance.lastLevel)
        {
            PopupManager.Instance.ShowPopup(PopupName.LastLevel, null);
            GameCache.Instance.lastLevel = false;
        }
        
    }

    private void OnCoinChange(object param)
    {
        StartCoroutine(coinChangeEffect(txtCoins, Convert.ToInt32(param)));
    }

    private void OnPointChange(object param)
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BtnBackOnClick()
    {
        LoadSceneManager.Instance.LoadScene("MainMenu");
    }

    public void BtnLevelOnScrollViewOnClick(int level)
    {
        GameCache.Instance.levelSelected = level;
        GameCache.Instance.mode = 1;
        LoadSceneManager.Instance.LoadScene("GamePlay");
    }

    public void BtnAddCoinOnClick()
    {
        PopupManager.Instance.ShowPopup(PopupName.AddCoin, null);
    }
    
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveListener(EventID.OnCoinChange, OnCoinChange);
        EventDispatcher.Instance.RemoveListener(EventID.OnPointChange, OnPointChange);
        PopupManager.Instance.ForceClosePopup();
    }
}
