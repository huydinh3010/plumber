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
    public Text text;
    private GameObject selected;
    private string[] str = {"Complete all levels and get 100 coins!", "Congratulations! You have completed daily challenge.Claim your reward." };
    // Start is called before the first frame update
    private void Awake()
    {
        sceneController.openScene();
        int total = 0;
        for(int i = 0; i < 8; i++)
        {
            if(GameData.Instance.completed[i] == 1)
            {
                total++;
                go_levels[i].GetComponent<SpriteRenderer>().sprite = d_levels[i];
            }
        }
        go_pool.GetComponent<SpriteRenderer>().sprite = s_pools[total];
        if (total == 8) text.text = str[1];
        else text.text = str[0];
    }

    void Start()
    {
        
    }

    public void btnBackOnClick()
    {

    }

    public void btnAddCoinOnClick()
    {

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
                Debug.Log(selected.name);
            }
            selected = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        onClickGameObject();
    }
}
