using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using ImoSysSDK.SocialPlatforms;
using Facebook.Unity;

public class PopupPlayServices : MonoBehaviour, IPopup
{
    [SerializeField] RectTransform scroll;
    [SerializeField] GameObject item;
    [SerializeField] GameObject content;
    [SerializeField] GameObject mask;
    [SerializeField] Image fillerImage;
    [SerializeField] Text textPlayerName;
    [SerializeField] Text textPlayerScore;
    [SerializeField] Image imagePlayerFlag;
    [SerializeField] Text textPlayerRank;
    [SerializeField] GameObject btn_LoginFb;
    [SerializeField] GameObject avatar;
    [SerializeField] Sprite error;
    [SerializeField] Sprite fb;
    private List<GameObject> go_items = new List<GameObject>();
    private LeaderboardItem[] leaderboardItems;
    private Action btn_Close_Callback;
    private bool isShow;
    private const int LEADER_BROAD_LIMIT_ITEM = 10;
    // Start is called before the first frame update
    void Start()
    {

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
            leaderboardItems = items;
            Action action = () =>
            {
                foreach (GameObject go in go_items) Destroy(go);
                int i = 0;
                Debug.Log("GameServices.PlayerId = " + GameServices.Instance.PlayerId);
                float item_height = item.GetComponent<RectTransform>().rect.height;
                float content_height = (items.Length < LEADER_BROAD_LIMIT_ITEM ? items.Length : LEADER_BROAD_LIMIT_ITEM) * item_height;
                content_height = content_height < scroll.rect.height ? scroll.rect.height : content_height;
                content.GetComponent<RectTransform>().sizeDelta = new Vector2(content.GetComponent<RectTransform>().sizeDelta.x, content_height);
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
                        go.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, i * -item_height);
                        go.GetComponent<ItemLeaderBroadSetup>().setup(items[i].rank, items[i].name, items[i].score, items[i].countryCode);
                        go_items.Add(go);
                    }
                    if (items[i].playerId == GameServices.Instance.PlayerId)
                    {
                        textPlayerName.text = items[i].name;
                        textPlayerScore.text = items[i].score.ToString();
                        imagePlayerFlag.sprite = Resources.Load<Sprite>("Flags/" + items[i].countryCode);
                        int rank = items[i].rank;
                        if (rank <= 0) textPlayerRank.text = "";
                        else if (rank < 1000) textPlayerRank.text = rank.ToString();
                        else if (rank < 1000000) textPlayerRank.text = rank / 1000 + "K";
                        else if (rank < 1000000000) textPlayerRank.text = rank / 1000000 + "M";
                        if (FacebookHelper.Instance.IsLoggedIn)
                        {
                            StartCoroutine(loadAvatar(items[i].avatarUrl));
                        }
                    }
                }
            };
            StartCoroutine(loadingEffect(action));
        }
        else
        {
            PopupManager.Instance.ShowNotification("Cannot load leaderbroad. Make sure you are connected to the internet and try again!", error, 1.5f);
        }
    }

    IEnumerator loadAvatar(string url)
    {
        WWW www = new WWW(url);
        yield return www;
        Texture2D texture = www.texture;
        Debug.Log("__________________texture != null: " + texture != null);
        //url.LoadImageIntoTexture(textFb2);
        avatar.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        www.Dispose();
        www = null;
    }

    private void Setup()
    {
        isShow = false;
        content.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        if (FacebookHelper.Instance.IsLoggedIn)
        {
            btn_LoginFb.GetComponent<Mask>().enabled = true;
            avatar.SetActive(true);
        }
        else
        {
            btn_LoginFb.GetComponent<Mask>().enabled = false;
            avatar.SetActive(false);
        }
        GameServices.Instance.UpdateScore(GameConfig.LEADERBROAD_ID, GameData.Instance.points, null);
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
        if (isShow && !FacebookHelper.Instance.IsLoggedIn)
        {
            FacebookHelper.Instance.Login(OnLoginFBFailed, OnLoginFBSuccess);
        }

    }

    private void OnLoginFBSuccess()
    {
        btn_LoginFb.GetComponent<Mask>().enabled = true;
        avatar.SetActive(true);
        GameServices.Instance.FetchLeaderboard(GameConfig.LEADERBROAD_ID, GameServices.LeaderboardTypes.LifeTime, LEADER_BROAD_LIMIT_ITEM, setupLeaderBroad);
        PopupManager.Instance.ShowNotification("Login Facebook completed!", fb, 1.5f);
    }

    private void OnLoginFBFailed(string message)
    {
        PopupManager.Instance.ShowNotification("Login Facebook failed!", fb, 1.5f);
    }
}
