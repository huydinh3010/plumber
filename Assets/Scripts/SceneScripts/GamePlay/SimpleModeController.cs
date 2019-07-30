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
        var textAsset = Resources.Load<TextAsset>("levels/simple/" + GameCache.Instance.levelSelected);
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
        strResults = new string[len];
        for (int i = 0; i < len; i++)
        {
            strResults[i] = arr[k++];
        }
    }

    public override void setupLevel()
    {
        unlockLv = GameData.Instance.listLevelStars[GameCache.Instance.levelSelected - 1] == 0;
        if (unlockLv && GameData.Instance.unlockLvState.durationSecs > 0)
        {
            durationSecs = GameData.Instance.unlockLvState.durationSecs;
            turnCount = GameData.Instance.unlockLvState.turnCount;
            removePipeCount = GameData.Instance.unlockLvState.removePipeCount;
            if (durationSecs <= star_time) star = 3;
            else if (durationSecs > star_time && durationSecs <= star_time * 2) star = 2;
            else star = 1;
            stopTime = false;
            animPlaying = false;
            endConstructPipe = false;
            m_Clones = new GameObject[row, col];
            float pipe_size = Mathf.Min(playZone.rect.width / 1000, playZone.rect.height / 1500) * 250 * 4 / col;
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    int index = GameData.Instance.unlockLvState.listPipeTypes[i * col + j] - 1;
                    int angle = GameData.Instance.unlockLvState.listPipeRotations[i * col + j];
                    if (index >= 0)
                    {
                        Vector3 position;
                        position.z = 0;
                        position.y = ((row / 2 - i) - 0.5f) * pipe_size;
                        position.x = ((j - col / 2) + 0.5f) * pipe_size;
                        if (index == 4 || index == 5)
                        {
                            GameObject go = Instantiate(pipes[index], Vector3.zero, Quaternion.Euler(0f, 0f, -angle * 90), playZone.transform);
                            go.GetComponent<RectTransform>().anchoredPosition3D = position;
                            go.GetComponent<RectTransform>().sizeDelta = new Vector2(pipe_size, pipe_size);
                            go.GetComponent<PipeProperties>().row = i;
                            go.GetComponent<PipeProperties>().col = j;
                            go.GetComponent<PipeProperties>().rotation = angle;
                            m_Clones[i, j] = go;
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
                            GameObject go = Instantiate(pipes[index], Vector3.zero, Quaternion.Euler(0f, 0f, -angle * 90), playZone.transform);
                            go.GetComponent<RectTransform>().anchoredPosition3D = position;
                            go.GetComponent<RectTransform>().sizeDelta = new Vector2(pipe_size, pipe_size);
                            go.GetComponent<PipeProperties>().row = i;
                            go.GetComponent<PipeProperties>().col = j;
                            go.GetComponent<PipeProperties>().rotation = angle;
                            go.GetComponent<Button>().onClick.AddListener(() => { OnPipeClick(go); });
                            m_Clones[i, j] = go;
                            StartCoroutine(rotatePipe(go, 1, rotateSpeed));
                        }
                    }
                }
            }
            constructPipeCount = 0;
            int count = GameData.Instance.unlockLvState.constructPipeCount;
            for(int i = 0; i < count; i++)
            {
                constructPipes();
            }
        }
        else
        {
            if(unlockLv)
            {
                GameData.Instance.unlockLvState.listPipeTypes = new int[row * col];
                GameData.Instance.unlockLvState.listPipeRotations = new int[row * col];
            }
            durationSecs = 0f;
            star = 3;
            turnCount = 0;
            stopTime = false;
            animPlaying = false;
            endConstructPipe = false;
            removePipeCount = 0;
            constructPipeCount = 0;
            m_Clones = new GameObject[row, col];
            float pipe_size = Mathf.Min(playZone.rect.width / 1000, playZone.rect.height / 1500) * 250 * 4 / col;
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
                            GameObject go = Instantiate(pipes[index], Vector3.zero, Quaternion.Euler(0f, 0f, -angle * 90), playZone.transform);
                            go.GetComponent<RectTransform>().anchoredPosition3D = position;
                            go.GetComponent<RectTransform>().sizeDelta = new Vector2(pipe_size, pipe_size);
                            go.GetComponent<PipeProperties>().row = i;
                            go.GetComponent<PipeProperties>().col = j;
                            go.GetComponent<PipeProperties>().rotation = angle;
                            m_Clones[i, j] = go;
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
                            GameObject go = Instantiate(pipes[index], Vector3.zero, Quaternion.Euler(0f, 0f, -angle * 90), playZone.transform);
                            go.GetComponent<RectTransform>().anchoredPosition3D = position;
                            go.GetComponent<RectTransform>().sizeDelta = new Vector2(pipe_size, pipe_size);
                            go.GetComponent<PipeProperties>().row = i;
                            go.GetComponent<PipeProperties>().col = j;
                            go.GetComponent<PipeProperties>().rotation = angle;
                            go.GetComponent<Button>().onClick.AddListener(() => { OnPipeClick(go); });
                            m_Clones[i, j] = go;
                            StartCoroutine(rotatePipe(go, 1, rotateSpeed));
                            angle = (angle + 1) % 4;
                        }
                        if (unlockLv)
                        {
                            GameData.Instance.unlockLvState.listPipeTypes[i * col + j] = index + 1;
                            GameData.Instance.unlockLvState.listPipeRotations[i * col + j] = angle;
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
        if (!stopTime)
        {
            durationSecs += Time.deltaTime;
            if (star == 3 && durationSecs > star_time && durationSecs <= star_time * 2)
            {
                star = 2;
                valvebg.GetComponent<Animator>().Play("Idle1");
            }
            else if (star == 2 && durationSecs > star_time * 2)
            {
                star = 1;
                valvebg.GetComponent<Animator>().Play("Idle2");
            }
            if (unlockLv)
            {
                GameData.Instance.unlockLvState.durationSecs = durationSecs;
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
                    GameData.Instance.unlockLvState.listPipeRotations[pp.row * this.col + pp.col] = pp.rotation;
                }
                while (angle < 0)
                {
                    try
                    {
                        angle += speed * Time.deltaTime;
                        gameObject.transform.eulerAngles -= new Vector3(0f, 0f, speed * Time.deltaTime - (angle > 0 ? angle : 0));
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
                    GameData.Instance.unlockLvState.listPipeRotations[pp.row * this.col + pp.col] = pp.rotation;
                }
                while (angle > 0)
                {
                    try
                    {
                        angle -= speed * Time.deltaTime;
                        gameObject.transform.eulerAngles += new Vector3(0f, 0f, speed * Time.deltaTime + (angle < 0 ? angle : 0));

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
            GameData.Instance.unlockLvState.listPipeTypes[pp.row * this.col + pp.col] = 0;
            GameData.Instance.unlockLvState.listPipeRotations[pp.row * this.col + pp.col] = 0;
        }
        return base.removePipe(gameObject);
    }

    protected override void OnPipeClick(GameObject go)
    {
        base.OnPipeClick(go);
        if (unlockLv)
        {
            GameData.Instance.unlockLvState.turnCount = turnCount;
        }
    }

    public override bool constructPipes()
    {
        bool b = base.constructPipes();
        if (unlockLv)
        {
            GameData.Instance.unlockLvState.constructPipeCount = constructPipeCount;
        }
        return b;
    }

    public override void removeIncorrectPipes()
    {
        base.removeIncorrectPipes();
        if (unlockLv)
        {
            GameData.Instance.unlockLvState.removePipeCount = removePipeCount;
        }
    }
}
