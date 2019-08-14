using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Events;
using System;
using UnityEngine.UI;

public abstract class GameController : MonoBehaviour
{
    [SerializeField] protected RectTransform playZone;
    [SerializeField] protected GameObject[] pipes;
    [SerializeField] protected int rotateSpeed;
    protected float durationSecs;
    public float DurationSecs{
        get
        {
            return durationSecs;
        }
    }

    protected int turnCount;
    public int TurnCount
    {
        get
        {
            return turnCount;
        }
    }
    protected int removePipeCount;

    public int RemovePipeCount
    {
        get
        {
            return removePipeCount;
        }
    }
    protected int constructPipeCount;
    public int ConstructPipeCount
    {
        get
        {
            return constructPipeCount;
        }
    }
    
    protected bool endConstructPipe;
    public bool EndConstructPipe
    {
        get
        {
            return endConstructPipe;
        }
    }

    protected bool stopTime;
    public bool StopTime
    {
        get
        {
            return stopTime;
        }
        set
        {
            stopTime = StopTime;
        }
    }

    protected GameObject valve;
    protected GameObject[,] m_Clones;
    protected int row;
    protected int col;
    protected string[] strResults;
    protected bool animPlaying;
    public abstract void loadLevelData();

    public abstract void setupLevel();

    protected void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!stopTime) durationSecs += Time.deltaTime;
    }

    public virtual void removeIncorrectPipes()
    {
        removePipeCount++;
        bool[,] tmp = new bool[row, col];
        for (int i = 0; i < strResults.Length; i++)
        {
            string[] pairs = strResults[i].Split(' ');
            tmp[int.Parse(pairs[0]), int.Parse(pairs[1])] = true;
        }
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (!tmp[i, j] && m_Clones[i, j] != null) StartCoroutine(removePipe(m_Clones[i, j]));
            }
        }
    }

    // ok
    public virtual IEnumerator removePipe(GameObject gameObject)
    {
        gameObject.GetComponent<Button>().interactable = false;
        float speed = 2f;
        float alpha = 1f;
        while (alpha > 0)
        {
            alpha -= speed * Time.deltaTime;
            gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, alpha > 0 ? alpha : 0);
            yield return null;
        }
        Destroy(gameObject);
    }

    public virtual bool constructPipes()
    {
        int k = constructPipeCount++;
        int c_len = (strResults.Length - 1) / 3 + 1;
        int i;
        for (i = k * c_len; i < c_len * (k + 1) && i < strResults.Length - 1; i++)
        {
            string[] pairs = strResults[i].Split(' ');
            int y = int.Parse(pairs[0]);
            int x = int.Parse(pairs[1]);
            int rotation = int.Parse(pairs[2]);
            int c_rotation = m_Clones[y, x].GetComponent<PipeProperties>().rotation;
            if (i == 0)
            {
                m_Clones[y, x].transform.Find("Valve").GetComponent<Animator>().Play("correct");
            }
            else
            {
                m_Clones[y, x].GetComponent<Button>().interactable = false;
                m_Clones[y, x].GetComponent<Animator>().Play("correct");
            } 
            StartCoroutine(rotatePipe(m_Clones[y, x], rotation - c_rotation, rotateSpeed * 2));
        }
        return endConstructPipe = i >= strResults.Length - 1;
    }

    public virtual IEnumerator rotatePipe(GameObject gameObject, int k, float speed)
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
    
    public virtual bool checkPipes(out List<GameObject> list_results, out List<int> list_ds)
    {
        list_results = new List<GameObject>();
        list_ds = new List<int>();
        // 0: up, 1: right, 2: down, 3: left
        int[] dx = { 0, 1, 0, -1 };
        int[] dy = { -1, 0, 1, 0 };
        int[] dd = { 2, 3, 0, 1 };
        GameObject go = valve;
        PipeProperties pp_t = go.GetComponent<PipeProperties>();
        int dir = pp_t.rotation;
        int x = pp_t.col;
        int y = pp_t.row;
        list_results.Add(valve);
        list_ds.Add(dir);
        do
        {
            // dir = input
            pp_t = go.GetComponent<PipeProperties>();
            dir = (4 + dir - pp_t.rotation) % 4;
            if (pp_t.line[dir] < 0)
            {
                list_results.Clear();
                list_ds.Clear();
                return false;
            }
            // dir = output
            dir = (pp_t.rotation + pp_t.line[dir]) % 4;
            x += dx[dir];
            y += dy[dir];
            dir = dd[dir];
            if (x < 0 || x >= col || y < 0 || y >= row || m_Clones[y, x] == null)
            {
                list_results.Clear();
                list_ds.Clear();
                return false;
            }
            go = m_Clones[y, x];
            list_results.Add(go);
            list_ds.Add(dir);
        } while (go.tag != "finish_pipe");
        return true;
    }

    protected void playAnimation(List<GameObject> list_results, List<int> list_ds)
    {
        EventDispatcher.Instance.PostEvent(EventID.PipeAnimationStart, this);
        AudioManager.Instance.PlayValveSound();
        AudioManager.Instance.Play("water");
        animPlaying = true;
        stopTime = true;
        for (int i = 0; i < list_results.Count - 1; i++)
        {
            PipeProperties pp = list_results[i].GetComponent<PipeProperties>();
            pp.next[pp.temp] = list_results[i + 1];
            pp.nextIn[pp.temp] = list_ds[i + 1];
            pp.temp++;
        }
        for (int i = 0; i < list_results.Count - 1; i++)
        {
            list_results[i].GetComponent<PipeProperties>().temp = 0;
            list_results[i + 1].GetComponent<Animator>().Play("Idle");
            Button button = list_results[i].GetComponent<Button>();
            if (button != null) button.interactable = true;
        }
        list_results[0].GetComponentsInChildren<Animator>()[0].Play("valvebg", -1, 1 - getStar() / 3f);
        list_results[0].GetComponentsInChildren<Animator>()[1].Play("valve");
    }

    protected virtual void OnPipeClick(GameObject go)
    {
        if (!animPlaying && !PopupManager.Instance.Showing)
        {
            AudioManager.Instance.PlayPipeSound();
            turnCount++;
            StartCoroutine(rotatePipe(go, 1, rotateSpeed));
        }
        else
        {
            EventDispatcher.Instance.PostEvent(EventID.SkipAnimation, this);
        }
    }

    protected virtual void OnValveClick()
    {
        if (!animPlaying && !PopupManager.Instance.Showing)
        {
            List<GameObject> list_results;
            List<int> list_dirs;
            if (checkPipes(out list_results, out list_dirs)) playAnimation(list_results, list_dirs);
        }
        else
        {
            EventDispatcher.Instance.PostEvent(EventID.SkipAnimation, this);
        }
    }

    public abstract int getStar();

    public virtual void resizeObjectWithPlayZone()
    {
        float pipe_size = Mathf.Min(playZone.rect.width * 4 / 1000 / col, playZone.rect.height * 6 / 1500 / row) * 250;
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (m_Clones[i, j] != null)
                {
                    Vector3 position;
                    position.z = 0;
                    position.y = ((row / 2 - i) - 0.5f) * pipe_size;
                    position.x = ((j - col / 2) + 0.5f) * pipe_size;
                    m_Clones[i, j].GetComponent<RectTransform>().anchoredPosition3D = position;
                    m_Clones[i, j].GetComponent<RectTransform>().sizeDelta = new Vector2(pipe_size, pipe_size);
                }
            }
        }
    }

    public virtual void destroy()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                Destroy(m_Clones[i, j]);
            }
        }
        enabled = false;
    }
    protected virtual void OnDestroy()
    {
        
    }
}
