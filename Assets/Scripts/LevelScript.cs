using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelScript : MonoBehaviour
{
    
    public static int[] data;
    private int page;
    public Sprite sp_lock;
    public Sprite sp_low;
    public Sprite sp_mid;
    public Sprite sp_high;
    public TextMeshProUGUI txt_pack;

    public Button[] btn_lvs;

    public void showMenu()
    {
        for (int i = page * 16; i < page * 16 + 16; i++)
        {
            switch (data[i])
            {
                case 1:
                    btn_lvs[i - page * 16].GetComponent<Image>().sprite = sp_low;
                    break;
                case 2:
                    btn_lvs[i - page * 16].GetComponent<Image>().sprite = sp_mid;
                    break;
                case 3:
                    btn_lvs[i - page * 16].GetComponent<Image>().sprite = sp_high;
                    break;
                default:
                    btn_lvs[i - page * 16].GetComponent<Image>().sprite = sp_lock;
                    break;
            }
        }
    }

    public void selectLevel(int level)
    {
        // chuyen scene sang lv tuong ung
    }

    public void transitionPageLevel(int arrow)
    {
        if(arrow > 0 && page < 34)
        {
            page++;
            txt_pack.text = "PACK " + (page + 1) + " / 35";
        }
        else if (arrow < 0 && page > 0)
        {
            page--;
            txt_pack.text = "PACK " + (page + 1) + " / 35";
        }
    }

    private void Awake()
    {
        showMenu();
        Debug.Log("Show menu page 0");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Debug.Log("Mouse Left Click");
        }
        if (Input.GetMouseButtonUp(0))
        {
           
            
        }

        Debug.Log("Level Scene update");
    }
    
    
}
