using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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
        string path = "Assets/Resources/levels/simple/" + GameData.Instance.level_selected + ".txt";
        StreamReader reader = new StreamReader(path, true);
        timer = int.Parse(reader.ReadLine());
        Debug.Log(timer);
        row = int.Parse(reader.ReadLine());
        col = int.Parse(reader.ReadLine());
        m_pipes = new int[row, col];
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                m_pipes[i, j] = int.Parse(reader.ReadLine());
            }
        }
        int len = int.Parse(reader.ReadLine());
        str_results = new string[len];
        for (int i = 0; i < len; i++)
        {
            str_results[i] = reader.ReadLine();
        }
        reader.Close();
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
                    scale = 6.0f / row;
                    Vector3 position;
                    position.z = 0;
                    position.y = ((row / 2 - i) - 0.5f) * 1.4f * scale;
                    position.x = ((j - col / 2) + 0.5f) * 1.4f * scale;
                    Quaternion rotation;
                    if (index == 4 || index == 5)
                    {
                        rotation = Quaternion.Euler(0f, 0f, -angle * 90);
                        GameObject go = Instantiate(pipes[index], position, rotation);
                        go.transform.localScale = new Vector3(scale, scale, scale);
                        go.GetComponent<PipeProperties>().row = i;
                        go.GetComponent<PipeProperties>().col = j;
                        go.GetComponent<PipeProperties>().rotation = angle;
                        m_clones[i, j] = go;
                        if (index == 4)
                        {
                            valve = go;
                            //go.transform.GetChild(1).eulerAngles += new Vector3(0f, 0f, angle * 90);
                            valvebg = go.transform.Find("Valve_bg").gameObject;
                            valvebg.transform.eulerAngles += new Vector3(0f, 0f, angle * 90);
                        }
                    }
                    else
                    {
                        angle = rd.Next(0, 3);
                        rotation = Quaternion.Euler(0f, 0f, -angle * 90);
                        //StartCoroutine(rotatePipe(gameObject));
                        GameObject go = Instantiate(pipes[index], position, rotation);
                        go.transform.localScale = new Vector3(scale, scale, scale);
                        go.GetComponent<PipeProperties>().row = i;
                        go.GetComponent<PipeProperties>().col = j;
                        go.GetComponent<PipeProperties>().rotation = angle;
                        m_clones[i, j] = go;
                        StartCoroutine(rotatePipe(go, 1, rotate_speed));
                    }
                }
            }
        }
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
                Debug.Log("Change1");
                star = 2;
                valvebg.GetComponent<Animator>().Play("Idle1");
                
            }
            else if (timer < t_stars[1] && star == 2)
            {
                Debug.Log("Change2");
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
