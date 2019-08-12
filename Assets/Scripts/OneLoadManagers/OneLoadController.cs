using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
public class OneLoadController : MonoBehaviour
{
    [SerializeField] Text txtLog;
    [SerializeField] GameObject scrollViewLog;
    [SerializeField] GameObject btn_Test;
    private string str;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        Input.multiTouchEnabled = false;
        SceneManager.LoadScene("MainMenu");
        GameData.Instance.LoadDataFromFile();
//#if ENV_PROD
//        btn_Test.SetActive(false);
//#else
//        btn_Test.SetActive(true);
//        Application.logMessageReceived += Application_logMessageReceived;
//#endif
    }

    private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
    {
        try
        {
            txtLog.text += "[" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "][" + type + "] : " + condition + "\n";
            if (type == LogType.Exception)
            {
                txtLog.text += stackTrace + "\n";
            }
        }
        catch (Exception e)
        {

        }
    }

   
    // Start is called before the first frame update
    void Start()
    {
        
        AudioManager.Instance.Initialize();
        //Debug.Log("Time: " + DateTime.Now.TimeOfDay + "--After Audio Initialize");
        //IAPManager.Instance.InitializePurchasing();
        //Debug.Log("Time: " + DateTime.Now.TimeOfDay + "--After IAP Initialize");
        FirebaseManager.Instance.Initialize();
        //Debug.Log("Time: " + DateTime.Now.TimeOfDay + "--After Firebase Initialize");
        FacebookManager.Instance.Initialize();
        //Debug.Log("Time: " + DateTime.Now.TimeOfDay + "--After Facebook Initialize");
        AdManager.Instance.Initialize();
        //Debug.Log("Time: " + DateTime.Now.TimeOfDay + "--After Admob Initialize");
    }
 
    private void Update()
    {
        
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Debug.Log("Application Pause: Save data to file");
            GameData.Instance.SaveDataToFile();
        }
    }

    private void OnApplicationQuit()
    {
        GameData.Instance.SaveDataToFile();
    }

    private void OnDestroy()
    {
        Debug.Log("Destroy Init Object");
    }

    public void BtnShowLogOnClick()
    {
        scrollViewLog.SetActive(!scrollViewLog.active);
    }

    public void BtnClearLogOnClick()
    {
        txtLog.text = "";
    }

    public void BtnAddCoinOnClick()
    {
        GameData.Instance.increaseCoin(100);
    }

    public void BtnAddPointOnClick()
    {
        GameData.Instance.increasePoint(1000);
    }

    public void BtnUnlock1LvOnClick()
    {
        if (GameData.Instance.listLevelStars.Count < 560)
        {
            GameData.Instance.listLevelStars[GameData.Instance.listLevelStars.Count - 1] = 3;
            GameData.Instance.listLevelStars.Add(0);
            GameData.Instance.unlockLevel++;
            GameCache.Instance.levelSelected = GameData.Instance.unlockLevel;
            GameData.Instance.unlockLvState.NewLevelState();
        }
    }

    public void BtnUnlock16LvOnClick()
    {
        if (GameData.Instance.listLevelStars.Count < 544)
        {
            GameData.Instance.listLevelStars[GameData.Instance.listLevelStars.Count - 1] = 3;
            for (int i = 0; i < 15; i++) GameData.Instance.listLevelStars.Add(3);
            GameData.Instance.listLevelStars.Add(0);
            GameData.Instance.unlockLevel += 16;
            GameCache.Instance.levelSelected = GameData.Instance.unlockLevel;
            GameData.Instance.unlockLvState.NewLevelState();
        }
    }
}
