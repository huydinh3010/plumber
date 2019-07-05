using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneSimpleLevelController : MonoBehaviour
{
    public SceneController sceneController;
    public GameObject OneStarLv;
    public GameObject TwoStarsLv;
    public GameObject ThreeStarsLv;
    public GameObject UnlockLv;
    public GameObject LockLv;
    public GameObject ListLevels;
    private Vector2 mousePos;
    private void Awake()
    {
        sceneController.openScene();
        
        for(int p = 0; p < 35; p++)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    GameObject go;
                    switch (GameData.Instance.level_stars[p*16 + i * 4 + j])
                    {
                        case 0:
                            go = Instantiate(UnlockLv, new Vector3((p*750 + j * 150 - 225) / 90f, (175 - i * 150) / 90f, 0), Quaternion.identity, ListLevels.transform);
                            go.GetComponentInChildren<Text>().text = (p * 16 + i * 4 + j + 1).ToString();
                            break;
                        case 1:
                            go = Instantiate(OneStarLv, new Vector3((p*750 + j * 150 - 225) / 90f, (175 - i * 150) / 90f, 0), Quaternion.identity, ListLevels.transform);
                            go.GetComponentInChildren<Text>().text = (p * 16 + i * 4 + j + 1).ToString();
                            break;
                        case 2:
                            go = Instantiate(TwoStarsLv, new Vector3((p*750 + j * 150 - 225) / 90f, (175 - i * 150) / 90f, 0), Quaternion.identity, ListLevels.transform);
                            go.GetComponentInChildren<Text>().text = (p * 16 + i * 4 + j + 1).ToString();
                            break;
                        case 3:
                            go = Instantiate(ThreeStarsLv, new Vector3((p*750 + j * 150 - 225) / 90f, (175 - i * 150) / 90f, 0), Quaternion.identity, ListLevels.transform);
                            go.GetComponentInChildren<Text>().text = (p * 16 + i * 4 + j + 1).ToString();
                            break;
                        default:
                            go = Instantiate(LockLv, new Vector3((p*750 + j * 150 - 225) / 90f, (175 - i * 150) / 90f, 0), Quaternion.identity, ListLevels.transform);
                            break;
                    }
                }
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
        if (Input.GetMouseButtonDown(0))
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Debug.Log("mouse x = " + mousePos.x + " mouse y = " + mousePos.y);
            if (position == mousePos)
            {
                RaycastHit2D raycast = Physics2D.Raycast(mousePos, Vector2.zero);
                if (raycast.collider != null)
                {
                    Debug.Log(raycast.collider.GetComponentInChildren<Text>().text);
                    
                    GameData.Instance.level_selected = int.Parse(raycast.collider.GetComponentInChildren<Text>().text);
                    sceneController.loadScene("GamePlay");
                    // chuyen scene
                }
            }
        }
    }

    public void BtnBackOnClick()
    {
        sceneController.loadScene("MainMenu");
    }

    

    public void BtnAddCoinOnClick()
    {

    }
    
    

    private void OnDestroy()
    {
        
    }
}
