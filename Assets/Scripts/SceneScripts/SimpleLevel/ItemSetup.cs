using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ItemSetup : MonoBehaviour
{
    [SerializeField] Text text;
    [SerializeField] Button button;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setup(int level, UnityEngine.Events.UnityAction action)
    {
        text.text = level.ToString();
        button.onClick.AddListener(action);
    }
}
