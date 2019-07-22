using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;

public class SimpleModeController : GameController
{
    private float star_time;
    private int[,] m_pipes;
    private GameObject valvebg;
    private int star;

    public override void loadLevelData()
    {
        var textAsset = Resources.Load<TextAsset>("levels/simple/" + GameCache.Instance.level_selected);
        string[] arr = textAsset.text.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        int k = 0;
        star_time = int.Parse(arr[k++]);

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

    // ok
    public override void setupLevel()
    {
        if(GameData.Instance.level_stars[GameCache.Instance.level_selected - 1] == 0)
        {
            duration_secs = GameData.Instance.level_durations;
        }
        else
        {
            duration_secs = 0f;
        }
        turn_count = 0;
        stop_time = false;
        animPlaying = false;
        m_clones = new GameObject[row, col];
        float pipe_size = Mathf.Min(PlayZone.rect.width / 1000, PlayZone.rect.height / 1500) * 250 * 4 / col;
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
                    position.y = ((row / 2 - i) - 0.5f) * pipe_size;
                    position.x = ((j - col / 2) + 0.5f) * pipe_size;

                    if (index == 4 || index == 5)
                    {
                        GameObject go = Instantiate(pipes[index], Vector3.zero, Quaternion.Euler(0f, 0f, -angle * 90), PlayZone.transform);
                        go.GetComponent<RectTransform>().anchoredPosition3D = position;
                        go.GetComponent<RectTransform>().sizeDelta = new Vector2(pipe_size, pipe_size);
                        go.GetComponent<PipeProperties>().row = i;
                        go.GetComponent<PipeProperties>().col = j;
                        go.GetComponent<PipeProperties>().rotation = angle;
                        m_clones[i, j] = go;
                        if (index == 4)
                        {
                            valve = go;
                            valvebg = go.transform.Find("Valve_bg").gameObject;
                            valvebg.transform.eulerAngles = Vector3.zero;
                            go.transform.Find("Valve").gameObject.GetComponent<Button>().onClick.AddListener(() => { OnValveClick(); });
                        }
                    }
                    else
                    {
                        angle = rd.Next(0, 3);
                        GameObject go = Instantiate(pipes[index], Vector3.zero, Quaternion.Euler(0f, 0f, -angle * 90), PlayZone.transform);
                        go.GetComponent<RectTransform>().anchoredPosition3D = position;
                        go.GetComponent<RectTransform>().sizeDelta = new Vector2(pipe_size, pipe_size);
                        go.GetComponent<PipeProperties>().row = i;
                        go.GetComponent<PipeProperties>().col = j;
                        go.GetComponent<PipeProperties>().rotation = angle;
                        go.GetComponent<Button>().onClick.AddListener(() => { OnPipeClick(go); });
                        m_clones[i, j] = go;
                        StartCoroutine(rotatePipe(go, 1, rotate_speed));
                    }
                }
            }
        }
    }

    private void Start()
    {
        
    }

    private void Awake()
    {
        
    }

    protected override void Update()
    {
        if (!stop_time)
        {
            duration_secs += Time.deltaTime;
            if (duration_secs > star_time && duration_secs <= star_time * 2)
            {
                star = 2;
                valvebg.GetComponent<Animator>().Play("Idle1");
            }
            else if (duration_secs > star_time * 2)
            {
                star = 1;
                valvebg.GetComponent<Animator>().Play("Idle2");
            }

           
        }

    }

    public override int getStar()
    {        
        return star;
    }

    private void OnDestroy()
    {
        if (GameData.Instance.level_stars[GameCache.Instance.level_selected - 1] == 0)
        {
            GameData.Instance.level_durations = duration_secs;
        }
    }

}
