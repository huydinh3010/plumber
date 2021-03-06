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
    [SerializeField] GameObject textContent;
    private List<GameObject> go_items = new List<GameObject>();
    private LeaderboardItem[] leaderboardItems;
    private Action btn_Close_Callback;
    private bool isShow;
    private const int LEADER_BROAD_LIMIT_ITEM = 10;
    private bool textEffectStop;
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

    IEnumerator textLoadingEffect(Text text)
    {
        textEffectStop = false;
        string temp = text.text;
        int k = 0;
        while (!textEffectStop)
        {
            k++;
            text.text = k < 4 ? text.text + "." : temp;
            k = k % 4;
            yield return new WaitForSeconds(0.3f);
        }
    }

    private void setupLeaderBroad(bool success, LeaderboardResponse res)
    {
        if (res != null)
        {
            LeaderboardItem[] items = res.items;
            leaderboardItems = items;
            Action action = () =>
            {
                foreach (GameObject go in go_items) Destroy(go);
                int i = 0;
                float item_height = item.GetComponent<RectTransform>().rect.height;
                float content_height = (items.Length < LEADER_BROAD_LIMIT_ITEM ? items.Length : LEADER_BROAD_LIMIT_ITEM) * item_height;
                content_height = content_height < scroll.rect.height ? scroll.rect.height : content_height;
                content.GetComponent<RectTransform>().sizeDelta = new Vector2(content.GetComponent<RectTransform>().sizeDelta.x, content_height);
                content.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                for (i = 0; i < items.Length; i++)
                {
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
                        Sprite flag = Resources.Load<Sprite>("Flags/" + items[i].countryCode);
                        if (flag == null) flag = Resources.Load<Sprite>("Flags/zz");
                        imagePlayerFlag.sprite = flag;
                        int rank = items[i].rank;
                        if (rank <= 0) textPlayerRank.text = "";
                        else if (rank < 1000) textPlayerRank.text = rank.ToString();
                        else if (rank < 1000000) textPlayerRank.text = rank / 1000 + "K";
                        else if (rank < 1000000000) textPlayerRank.text = rank / 1000000 + "M";
                        if (!GameCache.Instance.avatarLoaded)
                        {
                            StartCoroutine(loadAvatar(items[i].avatarUrl));
                        }
                    }
                }
                GameCache.Instance.connectedToServer = true;
            };
            try
            {
                if(transform.gameObject.active)
                {
                    textEffectStop = true;
                    textContent.SetActive(false);
                    StartCoroutine(loadingEffect(action));
                }  
            }
            catch
            {
                
            }
        }
        else
        {
            if (!GameCache.Instance.connectedToServer)
            {
                textEffectStop = true;
                textContent.SetActive(true);
                textContent.GetComponent<Text>().text = "Cannot connect to server. Try again later!";
            }
            //PopupManager.Instance.ShowNotification("Cannot load leaderbroad. Make sure you are connected to the internet and try again!", error, 1.5f);
        }
    }

    IEnumerator loadAvatar(string url)
    {
        WWW www = new WWW(url);
        yield return www;
        Texture2D texture = www.texture;
        if (texture != null)
        {
            avatar.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            GameCache.Instance.avatarLoaded = true;
            btn_LoginFb.GetComponent<Mask>().enabled = true;
            avatar.SetActive(true);
        }
        www.Dispose();
        www = null;
    }

    IEnumerator wait(float seconds, Action callback)
    {
        yield return new WaitForSeconds(seconds);
        callback();
    }

    private void Setup()
    {
        isShow = false;
        mask.SetActive(false);
        if (!GameCache.Instance.connectedToServer)
        {
            textContent.SetActive(true);
            textContent.GetComponent<Text>().text = "Connecting to server";
            StartCoroutine(textLoadingEffect(textContent.GetComponent<Text>()));
        }
        else
        {
            textContent.SetActive(false);
        }
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
        try
        {
            //GameServices.Instance.UpdateScore(GameConfig.LEADERBROAD_ID, GameData.Instance.points, null);
            GameServices.Instance.FetchLeaderboard(GameConfig.LEADERBROAD_ID, GameServices.LeaderboardTypes.LifeTime, LEADER_BROAD_LIMIT_ITEM, 0, setupLeaderBroad);
        }
        catch
        {

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
            AudioManager.Instance.Play(AudioManager.SoundName.BUTTON);
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
        try
        {
            //
            GameServices.Instance.UpdateScore(GameConfig.LEADERBROAD_ID, GameData.Instance.points, null);
            //
            StartCoroutine(wait(1f, () => {
                GameServices.Instance.FetchLeaderboard(GameConfig.LEADERBROAD_ID, GameServices.LeaderboardTypes.LifeTime, LEADER_BROAD_LIMIT_ITEM, 0, setupLeaderBroad);    
            }));
        }
        catch
        {

        }
        PopupManager.Instance.ShowNotification("Login Facebook completed!", fb, 1.5f);
    }

    private void OnLoginFBFailed(string message)
    {
        PopupManager.Instance.ShowNotification("Login Facebook failed!", fb, 1.5f);
    }
}
