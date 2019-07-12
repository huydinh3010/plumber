using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class SimpleModeController : GameController
{
    
    private bool stopTime;
    private float timer;
    private float[] t_stars = { 0, 0, 0 };
    private int[,] m_pipes;
    private int star;
    private GameObject valvebg;

    public override void loadLevelData()
    {
        var textAsset = Resources.Load<TextAsset>("levels/simple/" + GameCache.Instance.level_selected);
        Debug.Log(textAsset.text);
        string[] arr = textAsset.text.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        int k = 0;
        timer = int.Parse(arr[k++]);

        row = int.Parse(arr[k++]);
        col = int.Parse(arr[k++]);
        m_pipes = new int[row, col];
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                m_pipes[i, j] = int.Parse(arr[k++]);
            }
        }
        int len = int.Parse(arr[k++]);
        str_results = new string[len];
        for (int i = 0; i < len; i++)
        {
            str_results[i] = arr[k++];
        }
    }

    public override void setupLevel()
    {
        star = 3;
        t_stars[1] = timer;
        t_stars[2] = timer * 2;
        timer = timer * 3;
        stopTime = false;
        m_clones = new GameObject[row, col];
        System.Random rd = new System.Random();
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (m_pipes[i, j] != 70)
                {
                    int index = m_pipes[i, j] / 10 % 7 - 1;
                    int angle = m_pipes[i, j] % 10;
                    
                    Vector3 position;
                    position.z = 0;
                    position.y = ((row / 2 - i) - 0.5f) * 1.4f;
                    position.x = ((j - col / 2) + 0.5f) * 1.4f;
                    Quaternion rotation;
                    if (index == 4 || index == 5)
                    {
                        rotation = Quaternion.Euler(0f, 0f, -angle * 90);
                        GameObject go = Instantiate(pipes[index], position, rotation, PlayZone.transform);
                       
                        go.GetComponent<PipeProperties>().row = i;
                        go.GetComponent<PipeProperties>().col = j;
                        go.GetComponent<PipeProperties>().rotation = angle;
                        m_clones[i, j] = go;
                        if (index == 4)
                        {
                            valve = go;
                            //go.transform.GetChild(1).eulerAngles += new Vector3(0f, 0f, angle * 90);
                            valvebg = go.transform.Find("Valve_bg").gameObject;
                            valvebg.transform.eulerAngles = Vector3.zero;
                        }
                    }
                    else
                    {
                        angle = rd.Next(0, 3);
                        rotation = Quaternion.Euler(0f, 0f, -angle * 90);
                        //StartCoroutine(rotatePipe(gameObject));
                        GameObject go = Instantiate(pipes[index], position, rotation,PlayZone.transform);
                        go.GetComponent<PipeProperties>().row = i;
                        go.GetComponent<PipeProperties>().col = j;
                        go.GetComponent<PipeProperties>().rotation = angle;
                        m_clones[i, j] = go;
                        StartCoroutine(rotatePipe(go, 1, rotate_speed));
                    }
                }
            }
        }
        scale = 4.0f / col;
        PlayZone.transform.localScale = new Vector3(scale, scale, scale);
        Debug.Log(scale);
    }

    public override void stopDecreaseTime()
    {
        stopTime = true;
    }

    private void Start()
    {
        
    }

    private void Awake()
    {
        
    }

    private void Update()
    {
        
        if (timer > 0 && !stopTime)
        {
            timer -= Time.deltaTime;
            if (timer < t_stars[2] && star == 3)
            {
                //Debug.Log("Change1");
                star = 2;
                valvebg.GetComponent<Animator>().Play("Idle1");
                
            }
            else if (timer < t_stars[1] && star == 2)
            {
               // Debug.Log("Change2");
                star = 1;
                valvebg.GetComponent<Animator>().Play("Idle2");
            }
           
        }
    }

    public override int getStar()
    {
        return star;
    }

    
}
