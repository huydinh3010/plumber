using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Init : MonoBehaviour
{
    private void Awake()
    {
        //GameData.Instance.LoadDataFromFile();

        //Application.LoadLevelAdditive("MainMenu");

    }

   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        //GameData.Instance.SaveDataToFile();
        //Debug.Log("OneLoad Destroyed");
        
    }
}
