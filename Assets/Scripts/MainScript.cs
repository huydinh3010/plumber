using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScript : MonoBehaviour
{
    public GameObject o_level;

    private void Awake()
    {
        LevelScript.data = SaveLevelData.LoadData();
        if(LevelScript.data == null)
        {
            LevelScript.data = new int[560];
            Debug.Log("New data");
        }
    }

    

    // Start is called before the first frame update
    void Start()
    {
        //Quaternion rotation = new Quaternion(0f, 0f, 0f, 0f);
        //Vector3 position = new Vector3(0f, 0f);
        //Instantiate(o_level, position, rotation);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Menu Scene update");
    }

 

    private void OnDestroy()
    {
        SaveLevelData.SaveData(LevelScript.data);
    }
}
