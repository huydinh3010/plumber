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
    private string str;
    private void Awake()
    {
        str = "Start at: " + DateTime.Now.TimeOfDay;
        DontDestroyOnLoad(this);
        Input.multiTouchEnabled = false;

        Application.logMessageReceived += Application_logMessageReceived;
        Debug.Log(str);
        Debug.Log("Time: " + DateTime.Now.TimeOfDay + "--Start Init Awake");
        SceneManager.LoadScene("MainMenu");
        Debug.Log("Time: " + DateTime.Now.TimeOfDay + "--After LoadScene MainMenu function");
        GameData.Instance.LoadDataFromFile();
        Debug.Log("Time: " + DateTime.Now.TimeOfDay + "--After Load Data function");
    }

    private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
    {
        txtLog.text += "[" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "][" + type + "] : " + condition + "\n";
        if(type == LogType.Exception)
        {
            txtLog.text += stackTrace + "\n";
        }   
    }

    public void BtnShowLogOnClick()
    {
        scrollViewLog.SetActive(!scrollViewLog.active);
    }

    public void BtnClearLogOnClick()
    {
        txtLog.text = "";
    }

    // Start is called before the first frame update
    void Start()
    {
        
        AudioManager.Instance.Initialize();
        Debug.Log("Time: " + DateTime.Now.TimeOfDay + "--After Audio Initialize");
        //IAPManager.Instance.InitializePurchasing();
        //Debug.Log("Time: " + DateTime.Now.TimeOfDay + "--After IAP Initialize");
        FirebaseManager.Instance.Initialize();
        Debug.Log("Time: " + DateTime.Now.TimeOfDay + "--After Firebase Initialize");
        FacebookManager.Instance.Initialize();
        Debug.Log("Time: " + DateTime.Now.TimeOfDay + "--After Facebook Initialize");
        AdManager.Instance.Initialize();
        Debug.Log("Time: " + DateTime.Now.TimeOfDay + "--After Admob Initialize");
    }

    private void OnDisable()
    {
        
    }

    private void OnEnable()
    {
        
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
}
