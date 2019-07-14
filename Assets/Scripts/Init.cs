using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
public class Init : MonoBehaviour
{
    [SerializeField] Text txtLog;
    [SerializeField] GameObject scrollViewLog;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        GameData.Instance.LoadDataFromFile();
        Application.logMessageReceived += Application_logMessageReceived;
        SceneManager.LoadScene("MainMenu");
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
        //AdManager.Instance.Initialize();   
    }

    // Update is called once per frame
    void Update()
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
