using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using ImoSysSDK.SocialPlatforms;
public class PopupPlayServices : MonoBehaviour, IPopup
{
    [SerializeField] GameObject item;
    [SerializeField] GameObject content;
    [SerializeField] GameObject mask;
    [SerializeField] Image fillerImage;
    [SerializeField] Text textPlayerName;
    [SerializeField] Text textPlayerScore;
    [SerializeField] Image imagePlayerFlag;
    [SerializeField] Text textPlayerRank;
    [SerializeField] Sprite error;
    private Action btn_Close_Callback;
    private bool isShow;
    private const int LEADER_BROAD_LIMIT_ITEM = 10;
    // Start is called before the first frame update
    void Start()
    {
        GameServices.Instance.UpdateScore(GameConfig.LEADERBROAD_ID, GameData.Instance.points, null);
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator loadingEffect(Action callback)
    {
        mask.SetActive(true);
        fillerImage.fillAmount = 0f;
        float speed = 1f;
        while (fillerImage.fillAmount < 1f)
        {
            fillerImage.fillAmount += speed * Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.3f);
        mask.SetActive(false);
        callback();
    }

    private void setupLeaderBroad(bool success, LeaderboardItem[] items)
    {

        Debug.Log("IMO get leaderboard: " + success);
        if (items != null)
        {
            Action action = () => {
                int i = 0;
                Debug.Log("GameServices.PlayerId = " + GameServices.Instance.PlayerId);
                float contentHeight = (items.Length < LEADER_BROAD_LIMIT_ITEM ? items.Length : LEADER_BROAD_LIMIT_ITEM) * -200;
                content.GetComponent<RectTransform>().sizeDelta = new Vector2(content.GetComponent<RectTransform>().sizeDelta.x, contentHeight);
                content.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                for (i = 0; i < items.Length; i++)
                {
                    Debug.Log("Item " + i + ":");
                    Debug.Log("PlayerId = " + items[i].playerId);
                    Debug.Log("PlayerName = " + items[i].name);
                    Debug.Log("PlayerScore = " + items[i].score);
                    Debug.Log("PlayerRank = " + items[i].rank);
                    Debug.Log("PlayerCountry = " + items[i].countryCode);
                    Debug.Log("----------------------------end");
                    if (i < LEADER_BROAD_LIMIT_ITEM)
                    {
                        GameObject go = Instantiate(item, Vector3.zero, Quaternion.identity, content.transform);
                        go.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, i * 200);
                        go.GetComponent<ItemLeaderBroadSetup>().setup(items[i].rank + 1, items[i].name, items[i].score, items[i].countryCode);
                    }
                    if (items[i].playerId == GameServices.Instance.PlayerId)
                    {
                        textPlayerName.text = items[i].name;
                        textPlayerScore.text = items[i].score.ToString();
                        imagePlayerFlag.sprite = Resources.Load<Sprite>("Flags/" + items[i].countryCode);
                        int rank = items[i].rank + 1;
                        if (rank < 1000) textPlayerRank.text = rank.ToString();
                        else if (rank < 1000000) textPlayerRank.text = rank / 1000 + "K";
                        else if (rank < 1000000000) textPlayerRank.text = rank / 1000000 + "M";

                    }
                }
            };
            StartCoroutine(loadingEffect(action));
        }
        else
        {
            PopupManager.Instance.ShowNotification("Cannot load leaderbroad. Make sure you are connected to the internet and try again!", error, 2f);
        }
    }

    private void Setup()
    {
        isShow = false;
        GameServices.Instance.FetchLeaderboard(GameConfig.LEADERBROAD_ID, GameServices.LeaderboardTypes.LifeTime, LEADER_BROAD_LIMIT_ITEM, setupLeaderBroad);
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
        GetComponent<Animator>().Play("Show");
    }

    public void Close()
    {
        isShow = false;
        EventDispatcher.Instance.PostEvent(EventID.OnPopupClosed, this);
        GetComponent<Animator>().Play("Close");
    }

    public void BtnCloseOnClick()
    {
        if (isShow)
        {
            Close();
            btn_Close_Callback?.Invoke();
        }
    }

    public void BtnLoginFBOnClick()
    {
        if (isShow)
        {
            FacebookHelper.Instance.Login(OnLoginFBFailed, OnLoginFBSuccess);
        }

    }

    private void OnLoginFBSuccess()
    {
        if (FacebookHelper.Instance.IsLoggedIn)
        {
            GameServices.Instance.FetchLeaderboard(GameConfig.LEADERBROAD_ID, GameServices.LeaderboardTypes.LifeTime, LEADER_BROAD_LIMIT_ITEM, setupLeaderBroad);
        }
    }

    private void OnLoginFBFailed(string message)
    {
        Debug.Log("login failed: " + message);
    }
}
