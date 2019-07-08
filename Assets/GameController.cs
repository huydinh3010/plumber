using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Events;
using System;

public abstract class GameController : MonoBehaviour
{
    public Sprite[] s_valves;
    public GameObject[] pipes;
    public int rotate_speed;


    protected GameObject valve;
    protected GameObject[,] m_clones;

    protected int row;
    protected int col;

    protected float scale;
    protected string[] str_results;



    public abstract void loadLevelData();
    public virtual void stopDecreaseTime()
    {

    }


    public abstract void setupLevel();




    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

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

    public bool constructPipes(int k)
    {
        int c_len = (str_results.Length - 1) / 3 + 1;
        int i;
        Debug.Log(c_len);
        for (i = k * c_len; i < c_len * (k + 1) && i < str_results.Length - 1; i++)
        {
            Debug.Log(str_results[i]);
            string[] pairs = str_results[i].Split(' ');
            int y = int.Parse(pairs[0]);
            int x = int.Parse(pairs[1]);
            int rotation = int.Parse(pairs[2]);
            int c_rotation = m_clones[y, x].GetComponent<PipeProperties>().rotation;
            if (m_clones[y, x].tag != "valve")
            {
                m_clones[y, x].GetComponent<BoxCollider2D>().enabled = false;
                m_clones[y, x].GetComponent<Animator>().Play("correct");
            }
            else m_clones[y, x].transform.Find("Valve").GetComponent<Animator>().Play("correct");
            //if(m_clones[y,x].name != "CrossObject(Clone)") 
            StartCoroutine(rotatePipe(m_clones[y, x], rotation - c_rotation, 10f));
        }
        if (i == str_results.Length - 1) return true;
        return false;

    }

    public virtual IEnumerator removePipe(GameObject gameObject)
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        float speed = 2f;
        float alpha = 1f;
        while (true)
        {
            if (alpha > 0)
            {
                alpha -= speed * Time.deltaTime;
                gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, alpha);
                yield return 0;
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
                break;
            }
        }
        Destroy(gameObject);
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
                        gameObject.transform.eulerAngles -= new Vector3(0f, 0f, speed);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                    angle += speed;
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
                        gameObject.transform.eulerAngles += new Vector3(0f, 0f, speed);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                    angle -= speed;
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
