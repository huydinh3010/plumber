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
    private bool unlockLv;

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
        unlockLv = GameData.Instance.level_stars[GameCache.Instance.level_selected - 1] == 0;
        
        if (unlockLv && GameData.Instance.unlocklv_state.durations > 0)
        {
            duration_secs = GameData.Instance.unlocklv_state.durations;
            turn_count = GameData.Instance.unlocklv_state.turn_count;
            remove_pipe_count = GameData.Instance.unlocklv_state.remove_pipe;
            if (duration_secs <= star_time) star = 3;
            else if (duration_secs > star_time && duration_secs <= star_time * 2) star = 2;
            else star = 1;
            stop_time = false;
            animPlaying = false;
            endConstructPipe = false;
            m_clones = new GameObject[row, col];
            float pipe_size = Mathf.Min(PlayZone.rect.width / 1000, PlayZone.rect.height / 1500) * 250 * 4 / col;

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    int index = GameData.Instance.unlocklv_state.pipes_type[i * col + j] - 1;
                    int angle = GameData.Instance.unlocklv_state.pipes_rotation[i * col + j];
                    Debug.Log("index: " + index);
                    if (index >= 0)
                    {
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
                                if (star == 2) valvebg.GetComponent<Animator>().Play("Idle1");
                                else if (star == 1) valvebg.GetComponent<Animator>().Play("Idle2");
                            }
                        }
                        else
                        {
                            angle = (angle + 3) % 4;
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
            construct_pipe_count = 0;
            int count = GameData.Instance.unlocklv_state.construct_pipe;
            for(int i = 0; i < count; i++)
            {
                constructPipes();
            }
            
        }
        else
        {
            if(unlockLv)
            {
                GameData.Instance.unlocklv_state.pipes_type = new int[row * col];
                GameData.Instance.unlocklv_state.pipes_rotation = new int[row * col];
            }
            duration_secs = 0f;
            star = 3;
            turn_count = 0;
            stop_time = false;
            animPlaying = false;
            endConstructPipe = false;
            remove_pipe_count = 0;
            construct_pipe_count = 0;
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
                                //if (star == 2) valvebg.GetComponent<Animator>().Play("Idle1");
                                //else if (star == 1) valvebg.GetComponent<Animator>().Play("Idle2");
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
                            angle = (angle + 1) % 4;
                        }
                        if (unlockLv)
                        {
                            GameData.Instance.unlocklv_state.pipes_type[i * col + j] = index + 1;
                            GameData.Instance.unlocklv_state.pipes_rotation[i * col + j] = angle;
                        }
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
            if (star == 3 && duration_secs > star_time && duration_secs <= star_time * 2)
            {
                star = 2;
                valvebg.GetComponent<Animator>().Play("Idle1");
            }
            else if (star == 2 && duration_secs > star_time * 2)
            {
                star = 1;
                valvebg.GetComponent<Animator>().Play("Idle2");
            }
            if (unlockLv)
            {
                GameData.Instance.unlocklv_state.durations = duration_secs;
            }
        }

    }

    public override int getStar()
    {        
        return star;
    }

    public override IEnumerator rotatePipe(GameObject gameObject, int k, float speed)
    {
        if (k < 4 && k > -4)
        {
            if (k == 3) k = -1;
            else if (k == -3) k = 1;
            float angle = -90 * k;
            if (angle < 0)
            {
                PipeProperties pp = gameObject.GetComponent<PipeProperties>();
                pp.rotation += k;
                pp.rotation %= 4;
                if (unlockLv)
                {
                    GameData.Instance.unlocklv_state.pipes_rotation[pp.row * this.col + pp.col] = pp.rotation;
                }
                while (angle < 0)
                {
                    try
                    {
                        angle += speed * Time.deltaTime;
                        if (angle < 0)
                        {
                            gameObject.transform.eulerAngles -= new Vector3(0f, 0f, speed * Time.deltaTime);
                        }
                        else
                        {
                            gameObject.transform.eulerAngles -= new Vector3(0f, 0f, speed * Time.deltaTime - angle);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                    yield return 0;
                }
            }
            else
            {
                PipeProperties pp = gameObject.GetComponent<PipeProperties>();
                pp.rotation += k + 4;
                pp.rotation %= 4;
                if (unlockLv)
                {
                    GameData.Instance.unlocklv_state.pipes_rotation[pp.row * this.col + pp.col] = pp.rotation;
                    
                }
                while (angle > 0)
                {
                    try
                    {
                        angle -= speed * Time.deltaTime;
                        if (angle > 0)
                        {
                            gameObject.transform.eulerAngles += new Vector3(0f, 0f, speed * Time.deltaTime);
                        }
                        else
                        {
                            gameObject.transform.eulerAngles += new Vector3(0f, 0f, speed * Time.deltaTime + angle);
                        }

                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                    yield return 0;
                }
            }
        }
    }

    public override IEnumerator removePipe(GameObject gameObject)
    {
        if (unlockLv)
        {
            PipeProperties pp = gameObject.GetComponent<PipeProperties>();
            GameData.Instance.unlocklv_state.pipes_type[pp.row * this.col + pp.col] = 0;
            GameData.Instance.unlocklv_state.pipes_rotation[pp.row * this.col + pp.col] = 0;
        }
        return base.removePipe(gameObject);
    }

    protected override void OnPipeClick(GameObject go)
    {
        base.OnPipeClick(go);
        if (unlockLv)
        {
            GameData.Instance.unlocklv_state.turn_count = turn_count;
        }
    }

    public override bool constructPipes()
    {
        bool b = base.constructPipes();
        GameData.Instance.unlocklv_state.construct_pipe = construct_pipe_count;
        return b;
    }

    public override void removeIncorrectPipes()
    {
        base.removeIncorrectPipes();
        GameData.Instance.unlocklv_state.remove_pipe = remove_pipe_count;
    }
}
