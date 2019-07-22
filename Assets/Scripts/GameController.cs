using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Events;
using System;
using UnityEngine.UI;

public abstract class GameController : MonoBehaviour
{
    
    public RectTransform PlayZone;
    public Sprite[] s_valves;
    public GameObject[] pipes;
    public int rotate_speed;
    public float duration_secs;
    public int turn_count;
    public bool panelShowing;

    protected GameObject valve;
    protected GameObject[,] m_clones;

    protected int row;
    protected int col;


    protected string[] str_results;

    protected bool stop_time;
    protected bool animPlaying;
    public abstract void loadLevelData();

    //public void stopTime()
    //{
    //    stop_time = true;
    //}


    public abstract void setupLevel();


    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!stop_time) duration_secs += Time.deltaTime;
    }

    public void removeIncorrectPipes()
    {
        bool[,] tmp = new bool[row, col];
        for (int i = 0; i < str_results.Length; i++)
        {
            string[] pairs = str_results[i].Split(' ');
            tmp[int.Parse(pairs[0]), int.Parse(pairs[1])] = true;
        }
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (!tmp[i, j] && m_clones[i, j] != null) StartCoroutine(removePipe(m_clones[i, j]));
            }
        }
    }

    // ok
    public virtual IEnumerator removePipe(GameObject gameObject)
    {
        gameObject.GetComponent<Button>().interactable = false;
        float speed = 2f;
        float alpha = 1f;
        while (true)
        {
            if (alpha > 0)
            {
                alpha -= speed * Time.deltaTime;
                gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, alpha);
                yield return 0;
            }
            else
            {
                gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
                break;
            }
        }
        Destroy(gameObject);
    }

    // ok
    public bool constructPipes(int k)
    {
        int c_len = (str_results.Length - 1) / 3 + 1;
        int i;
        for (i = k * c_len; i < c_len * (k + 1) && i < str_results.Length - 1; i++)
        {
            string[] pairs = str_results[i].Split(' ');
            int y = int.Parse(pairs[0]);
            int x = int.Parse(pairs[1]);
            int rotation = int.Parse(pairs[2]);
            int c_rotation = m_clones[y, x].GetComponent<PipeProperties>().rotation;
            if (i == 0)
            {
                m_clones[y, x].transform.Find("Valve").GetComponent<Animator>().Play("correct");
            }
            else
            {
                m_clones[y, x].GetComponent<Button>().interactable = false;
                m_clones[y, x].GetComponent<Animator>().Play("correct");
            } 
            StartCoroutine(rotatePipe(m_clones[y, x], rotation - c_rotation, rotate_speed * 2));
        }
        if (i >= str_results.Length - 1) return true;
        return false;

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
                        if(angle < 0)
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
                while (angle > 0)
                {
                    try
                    {
                        angle -= speed * Time.deltaTime;
                        if(angle > 0)
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
    
    // hold
    public virtual bool checkPipes(out List<GameObject> list_results, out List<int> list_ds)
    {
        list_results = new List<GameObject>();
        list_ds = new List<int>();
        // 0: up, 1: right, 2: down, 3: left
        int[] dx = { 0, 1, 0, -1 };
        int[] dy = { -1, 0, 1, 0 };
        int[] dd = { 2, 3, 0, 1 };
        GameObject go = valve;
        int dir = 0;
        int x = go.GetComponent<PipeProperties>().col;
        int y = go.GetComponent<PipeProperties>().row;
        list_results.Add(valve);
        list_ds.Add(dir);
        do
        {
            // dir = dau vao
            PipeProperties pp = go.GetComponent<PipeProperties>();
            dir = (4 + dir - pp.rotation) % 4;
            if (pp.line[dir] < 0)
            {
                list_results.Clear();
                list_ds.Clear();
                return false;
            }
            // dir = dau ra
            dir = (pp.rotation + pp.line[dir]) % 4;
            x += dx[dir];
            y += dy[dir];
            dir = dd[dir];
            if (x < 0 || x >= col || y < 0 || y >= row || m_clones[y, x] == null)
            {
                list_results.Clear();
                list_ds.Clear();
                return false;
            }
            go = m_clones[y, x];
            list_results.Add(go);
            list_ds.Add(dir);
        } while (go.tag != "finish_pipe");
        return true;
    }

    protected void playAnimation(List<GameObject> list_results, List<int> list_ds)
    {
        EventDispatcher.Instance.PostEvent(EventID.PipeAnimationStart, this);
        animPlaying = true;
        stop_time = true;
        for (int i = 0; i < list_results.Count - 1; i++)
        {
            PipeProperties pp = list_results[i].GetComponent<PipeProperties>();
            pp.next[pp.i] = list_results[i + 1];
            pp.next_in[pp.i] = list_ds[i + 1];
            pp.i++;
        }
        for (int i = 0; i < list_results.Count - 1; i++)
        {
            list_results[i].GetComponent<PipeProperties>().i = 0;
            list_results[i + 1].GetComponent<Animator>().Play("Idle");
            Button button = list_results[i].GetComponent<Button>();
            if (button != null) button.interactable = true;
        }
        list_results[0].GetComponentsInChildren<Animator>()[0].Play("valvebg", -1, 1 - getStar() / 3f);
        list_results[0].GetComponentsInChildren<Animator>()[1].Play("valve");
    }

    protected virtual void OnPipeClick(GameObject go)
    {
        if (!animPlaying && !panelShowing)
        {
            turn_count++;
            StartCoroutine(rotatePipe(go, 1, rotate_speed));
        }
        else
        {
            EventDispatcher.Instance.PostEvent(EventID.StopAnimation, this);
            EventDispatcher.Instance.PostEvent(EventID.PipeAnimationEnd, this);
        }
    }

    protected virtual void OnValveClick()
    {
        if (!animPlaying && !panelShowing)
        {
            List<GameObject> list_results;
            List<int> list_dirs;
            if (checkPipes(out list_results, out list_dirs)) playAnimation(list_results, list_dirs);
        }
        else
        {
            EventDispatcher.Instance.PostEvent(EventID.StopAnimation, this);
            EventDispatcher.Instance.PostEvent(EventID.PipeAnimationEnd, this);
        }
    }

    public abstract int getStar();

    public virtual void destroy()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                Destroy(m_clones[i, j]);
            }
        }
    }
}
