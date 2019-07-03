using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneSimpleLevelController : MonoBehaviour
{
    public GameObject OneStarLv;
    public GameObject TwoStarsLv;
    public GameObject ThreeStarsLv;
    public GameObject UnlockLv;
    public GameObject LockLv;
    public GameObject ListLevels;
    private int[] level_stars;
    private void Awake()
    {
        System.Random rd = new System.Random();
        level_stars = new int[560];
        int level_unlock = 200;
        for(int i = 0; i < 560; i++)
        {
            if (i > level_unlock) level_stars[i] = -1;
            else if (i == level_unlock) level_stars[i] = 0;
            else
            {
                level_stars[i] = rd.Next(1, 3);
            }
        }
        for(int p = 0; p < 35; p++)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    GameObject go;
                    switch (level_stars[p*16 + i * 4 + j])
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
        
    }

    public void BtnBackOnClick()
    {

    }

    

    public void BtnAddCoinOnClick()
    {

    }
    
    public void OnValueChanged()
    {
        Debug.Log("Value Changed");
        
    }

    private void OnDestroy()
    {
        
    }
}
