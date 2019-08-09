using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemAchievementSetup : MonoBehaviour
{
    [SerializeField] GameObject btn_Coin;
    [SerializeField] Text text_Point;
    [SerializeField] Text text_Coin;
    [SerializeField] GameObject image_Tick;
    [SerializeField] Animator anim_Button;
    
    public void setup(int point, int coin)
    {
        text_Point.text = "Get\n" + point.ToString() + "\npoints";
        text_Coin.text = "+" + coin.ToString();
    }

    public void setNormalState()
    {
        btn_Coin.GetComponent<Image>().color = new Color32(100, 100, 100, 150);
        btn_Coin.GetComponent<Button>().enabled = false;
        image_Tick.SetActive(false);
        anim_Button.Play("Idle");
    }

    public void setActiveState()
    {
        btn_Coin.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        btn_Coin.GetComponent<Button>().enabled = true;
        image_Tick.SetActive(false);
        anim_Button.Play("Scale");
    }

    public void setPassedState()
    {
        btn_Coin.GetComponent<Image>().color = new Color32(255, 255, 255, 200);
        btn_Coin.GetComponent<Button>().enabled = false;
        image_Tick.SetActive(true);
        anim_Button.Play("Idle");
    }
}
