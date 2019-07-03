using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneMainMenuController : MonoBehaviour
{
    public Texture2D texture;
    public GameObject btnDailyChallenge;
    public GameObject btnRemoveAds;
    public GameObject btnMoreGame;

    public GameObject panelRate;
    public GameObject panelPlayServices;
    public GameObject panelDailyReward;
    private bool transition;
    private float speed;
    private float alpha;
    private void Awake()
    {
        //if (GameData.Instance.firstMenuLoad)
        //{
            
        //    GameData.Instance.firstMenuLoad = false;
        //}
        //else
        //{

        //}
        
        
    }
    // Start is called before the first frame update
    void Start()
    {
        alpha = 1f;
        speed = 1f;
    }

    private void OnGUI()
    {
        if (transition)
        {
            alpha -= speed * Time.deltaTime;
            if (alpha < 0)
                GUI.depth = -10;
            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texture);
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BtnPlayOnClick()
    {
        SceneManager.LoadScene("SimpleLevel");

    }

    public void BtnDailyChallengeOnClick()
    {
        SceneManager.LoadScene("ChallengeLevel");
    }

    public void BtnRemoveAdsOnClick()
    {

    }

    public void BtnRateOnClick()
    {
        panelRate.GetComponent<Animator>().Play("Show");
    }


    public void BtnPlayServicesOnClick()
    {
        panelPlayServices.GetComponent<Animator>().Play("Show");
    }

    public void BtnHelpOnClick()
    {

    }

    public void BtnMoreGameOnClick()
    {

    }

    public void BtnCloseOnPanelOnClick(GameObject panel)
    {
        panel.GetComponent<Animator>().Play("Close");
    }

    public void BtnRateOnPanelOnClick()
    {
        
    }

    

    private void OnDestroy()
    {
        Debug.Log("Main Scene Destroyed");
    }


}
