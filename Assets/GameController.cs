using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Events;

public class GameController : MonoBehaviour
{
    public SpriteRenderer[] star_valve;
    
    public GameObject[] pipes;
    private GameObject valve;
    private GameObject[,] m_clones;
    public int level;
    public int rotate_speed;
    public bool game_over;
    public int star;
    private int str_index;
    private int part;
    private int coins;
    private int points;
    private bool streaming;
    private int row;
    private int col;
    private float timer;
    private float time_remain;
    private float scale;
    private int[,] m_pipes;
    public List<GameObject> result = new List<GameObject>();
    public List<int> list_dir = new List<int>();
    public int index;
    private string[] str;

    void loadLevelData()
    {
        string path = "Assets/Resources/levels/simple/" + level + ".txt";
        Debug.Log(path);
        StreamReader reader = new StreamReader(path, true);
        timer = int.Parse(reader.ReadLine());
        time_remain = timer;
        row = int.Parse(reader.ReadLine());
        col = int.Parse(reader.ReadLine());
        m_pipes = new int[row,col];
        for(int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                m_pipes[i, j] = int.Parse(reader.ReadLine());
            }
        }
        int len = int.Parse(reader.ReadLine());
        str = new string[len];
        for (int i = 0; i < len; i++)
        {
            str[i] = reader.ReadLine();
        }
    }

    private void createPipes()
    {
        m_clones = new GameObject[row, col];
        System.Random rd = new System.Random();
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if(m_pipes[i, j] != 70)
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
                        if(index == 4)
                        {
                            valve = go;
                            go.transform.GetChild(1).eulerAngles += new Vector3(0f, 0f, angle * 90);
                            Debug.Log(angle * 90);
                        }
                    }
                    else
                    {
                        angle = rd.Next(0, 3);
                        rotation = Quaternion.Euler(0f, 0f, -angle * 90);
                        //m_pipes[i, j] = (m_pipes[i, j] / 10) * 10 + angle;
                        StartCoroutine(rotatePipe(gameObject));
                        GameObject go = Instantiate(pipes[index], position, rotation);
                        go.transform.localScale = new Vector3(scale, scale, scale);
                        go.GetComponent<PipeProperties>().row = i;
                        go.GetComponent<PipeProperties>().col = j;
                        go.GetComponent<PipeProperties>().rotation = (angle + 1) % 4;  
                        m_clones[i, j] = go;
                        StartCoroutine(rotatePipe(go));
                    }         
                }           
            }
        }
    }

    private void Awake()
    {
        loadLevelData();
        createPipes();
        part = str.Length / 3 + 1;
        star = 3;
    }   

    // Start is called before the first frame update
    void Start()
    {
       
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!streaming && !game_over && timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                star--;
                valve.GetComponentsInChildren<SpriteRenderer>()[1].sprite = star_valve[2 - star].sprite;
                if (star > 1) timer = time_remain;
            }
        }
        onClickPipe();

    }



    void onClickPipe() // xu ly su kien bam vao pipes
    {
        if (Input.GetMouseButtonDown(0))
        {
           
            Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition); // lay vi tri chuot, chuyen thanh worldposition
            RaycastHit2D raycastHit = Physics2D.Raycast(position, Vector2.zero);
            if(raycastHit.collider != null)
            {
                if (raycastHit.collider.tag == "pipe" && !streaming)
                {
                    GameObject gameObject = raycastHit.collider.gameObject;
                    gameObject.GetComponent<PipeProperties>().rotation = (gameObject.GetComponent<PipeProperties>().rotation + 1) % 4;
                    StartCoroutine(rotatePipe(gameObject));
                }
                else if (raycastHit.collider.tag == "valve" && !streaming)
                {
                    if (!checkPipes())
                    {
                        result.Clear();
                        list_dir.Clear();
                    }
                    else
                    {
                        streaming = true;
                    }
                }
            }
        }
    }

    IEnumerator rotatePipe(GameObject gameObject)
    { 
        float angle = -90f;
        while (angle < 0) 
        {
            gameObject.transform.eulerAngles -= new Vector3(0f, 0f, rotate_speed);
            angle += rotate_speed;
            yield return 0;
        }
    }
    
    bool checkPipes()
    {
        // 0: up, 1: right, 2: down, 3: left
        int[] dx = { 0, 1, 0, -1 };
        int[] dy = { -1, 0, 1, 0 };
        int[] dd = { 2, 3, 0, 1 };
        GameObject go = valve;
        int dir = 0;
        int x = go.GetComponent<PipeProperties>().col;
        int y = go.GetComponent<PipeProperties>().row;
        list_dir.Add(0);
        result.Add(valve);
        do
        {
            // dir = dau vao
            PipeProperties pp = go.GetComponent<PipeProperties>();
            dir = (4 + dir - pp.rotation) % 4;
            if (pp.line[dir] < 0) return false;
            // dir = dau ra
            dir = (pp.rotation + pp.line[dir]) % 4;
            x += dx[dir];
            y += dy[dir];
            dir = dd[dir];
            if (x < 0 || x >= col || y < 0 || y >= row || m_clones[y, x] == null) return false;
            go = m_clones[y, x];
            result.Add(go);
            list_dir.Add(dir);
        } while (go.tag != "finish_pipe");
        
        valve.GetComponentsInChildren<Animator>()[0].enabled = true;
        valve.GetComponentsInChildren<Animator>()[1].Play("valvebg", -1, (3 - star) / 3f);
        valve.GetComponentsInChildren<Animator>()[1].enabled = true;

        return true;
    }

    public void buttonBackOnClick()
    {
        
    }
    public void buttonSoundOnClick()
    {

    }
    public void buttonRemoveOnClick()
    {

    }
    public void buttonConstructOnClick()
    {
        int i;
        for(i = str_index; i < part && i < str.Length; i++)
        {
            string[] tmp = str[i].Split(' ');
            int y = int.Parse(tmp[0]);
            int x = int.Parse(tmp[1]);
            int rotation = int.Parse(tmp[2]);
            GameObject go = m_clones[y, x];
            go.GetComponent<PipeProperties>().rotation = rotation;
            

        }
        str_index = i;
        part += part;
    }
    public void buttonAddCoinOnClick()
    {

    }

}
