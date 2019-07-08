using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeSceneController : MonoBehaviour
{
    public SceneController sceneController;
    public GameObject[] go_levels;
    public GameObject go_pool;
    public Sprite[] s_pools;
    public Sprite[] d_levels;
    public Text text_tutorial;
    public Text txtCoins;
    private int total;
    private GameObject selected;
    private string[] str = {"Complete all levels and get 100 coins!", "Congratulations! You have completed daily challenge.Claim your reward." };
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
                go_levels[i].GetComponent<SpriteRenderer>().sprite = d_levels[i];
            }
        }
        if(total > 0) go_pool.GetComponent<SpriteRenderer>().sprite = s_pools[total - 1];
        if (total == 8) text_tutorial.text = str[1];
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

    public void onClickGameObject()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 postion = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D raycast = Physics2D.Raycast(postion, Vector2.zero);
            if (raycast.collider != null) selected = raycast.collider.gameObject;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Vector2 postion = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D raycast = Physics2D.Raycast(postion, Vector2.zero);
            if (raycast.collider != null && raycast.collider.gameObject == selected)
            {
                if(selected.name == "Lv1")
                {
                    GameData.Instance.mode = 2;
                    GameData.Instance.level_selected = 1;
                    sceneController.loadScene("GamePlay");
                }
                else if(selected.name == "Lv2")
                {
                    GameData.Instance.mode = 2;
                    GameData.Instance.level_selected = 2;
                    sceneController.loadScene("GamePlay");
                }
                else if (selected.name == "Lv3")
                {
                    GameData.Instance.mode = 2;
                    GameData.Instance.level_selected = 3;
                    sceneController.loadScene("GamePlay");
                }
                else if (selected.name == "Lv4")
                {
                    GameData.Instance.mode = 2;
                    GameData.Instance.level_selected = 4;
                    sceneController.loadScene("GamePlay");
                }
                else if (selected.name == "Lv5")
                {
                    GameData.Instance.mode = 2;
                    GameData.Instance.level_selected = 5;
                    sceneController.loadScene("GamePlay");
                }
                else if (selected.name == "Lv6")
                {
                    GameData.Instance.mode = 2;
                    GameData.Instance.level_selected = 6;
                    sceneController.loadScene("GamePlay");
                }
                else if (selected.name == "Lv7")
                {
                    GameData.Instance.mode = 2;
                    GameData.Instance.level_selected = 7;
                    sceneController.loadScene("GamePlay");
                }
                else if (selected.name == "Lv8")
                {
                    GameData.Instance.mode = 2;
                    GameData.Instance.level_selected = 8;
                    sceneController.loadScene("GamePlay");
                }
                else if (selected.name == "Pool")
                {
                    if(total == 8)
                    {
                        GameData.Instance.increaseCoin(100);
                        // x
                        go_pool.GetComponent<BoxCollider2D>().enabled = false;
                        //
                    }
                }
            }
            selected = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        onClickGameObject();
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveListener(EventID.OnCoinChange, onCoinChange);
    }
}
