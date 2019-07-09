using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Init : MonoBehaviour
{
    [SerializeField] SceneController sceneController;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        GameData.Instance.LoadDataFromFile();
        SceneManager.LoadScene("MainMenu");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnApplicationPause(bool pause)
    {
        //GameData.Instance.SaveDataToFile();
        if (pause)
        {
            GameData.Instance.SaveDataToFile();
        }
    }

    private void OnApplicationQuit()
    {
        GameData.Instance.SaveDataToFile();
    }
}
