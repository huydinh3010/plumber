using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class HelpLevelController : GameController
{
    public GameObject hand;
    public Text text_tutorial;

    private int h_len;
    private int[] pos_x;
    private int[] pos_y;
    private int[,] m_pipes;
    private string[] text_content = { "Touch the pipes to turn them.", "Make a water path from value to container.", "Open the valve!", "Good job!" };
    private float pipe_size;

    public override int getStar()
    {
        return 3;
    }

    public override void loadLevelData()
    {
        var textAsset = Resources.Load<TextAsset>("levels/simple/1") ;
        string[] arr = textAsset.text.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        int k = 0;
        int timer = int.Parse(arr[k++]);

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

    //ok
    public override void setupLevel()
    {
        turn_count = 0;
        duration_secs = 0f;
        stop_time = false;
        animPlaying = false;
        remove_pipe_count = 0;
        construct_pipe_count = 0;
        endConstructPipe = false;
        h_len = 0;
        pos_x = new int[3];
        pos_y = new int[3];
        pos_y[0] = 2; pos_x[0] = 0;
        pos_y[1] = 2; pos_x[1] = 2;
        pos_y[2] = 0; pos_x[2] = 0;
        m_clones = new GameObject[row, col];
        pipe_size = Mathf.Min(PlayZone.rect.width / 1000, PlayZone.rect.height / 1500) * 250 * 4 / col;
        for (int i = 0; i < str_results.Length; i++)
        {
            string[] pairs = str_results[i].Split(' ');
            int y = int.Parse(pairs[0]);
            int x = int.Parse(pairs[1]);
            int angle = int.Parse(pairs[2]);
            Vector3 position;
            position.z = 0;
            position.y = ((row / 2 - y) - 0.5f) * pipe_size;
            position.x = ((x - col / 2) + 0.5f) * pipe_size;
            m_clones[y, x] = Instantiate(pipes[m_pipes[y, x] / 10 % 7 - 1], Vector3.zero, Quaternion.Euler(0f, 0f, -angle * 90), PlayZone.transform);
            m_clones[y, x].GetComponent<RectTransform>().anchoredPosition3D = position;
            if (m_clones[y, x].GetComponent<Button>() != null)
            {
                m_clones[y, x].GetComponent<Button>().onClick.AddListener(() => { OnPipeClick(m_clones[y, x]); });
                m_clones[y, x].GetComponent<Button>().interactable = false;
            }
            m_clones[y, x].GetComponent<PipeProperties>().row = y;
            m_clones[y, x].GetComponent<PipeProperties>().col = x;
            m_clones[y, x].GetComponent<PipeProperties>().rotation = angle;
            if (i == 0)
            {
                valve = m_clones[y, x];
                valve.transform.Find("Valve_bg").eulerAngles = Vector3.zero;
                valve.GetComponentInChildren<Button>().interactable = false;
                valve.GetComponentInChildren<Button>().onClick.AddListener(()=> { OnValveClick(); });
            }
        }
        for (int i = 0; i < pos_x.Length - 1; i++)
        {
            m_clones[pos_y[i], pos_x[i]].transform.eulerAngles += new Vector3(0f, 0f, 90f);
        }
        m_clones[pos_y[0], pos_x[0]].GetComponent<Button>().interactable = true;

        // hand
        hand = Instantiate(hand, Vector3.zero, Quaternion.identity, PlayZone.transform);
        hand.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(((pos_x[0] - col / 2) + 0.5f) * pipe_size, ((row / 2 - pos_y[0]) - 0.5f) * pipe_size, 0);
        hand.GetComponent<RectTransform>().sizeDelta = new Vector2(pipe_size, pipe_size);
         //
        text_tutorial.enabled = true;
        text_tutorial.text = text_content[0];
    }

    //ok
    public override IEnumerator rotatePipe(GameObject gameObject, int k, float speed)
    {
        Debug.Log("Help rotate");
        if (k == 1)
        {
            gameObject.GetComponent<Button>().interactable = false;
            text_tutorial.text = text_content[++h_len];
            GameObject next = m_clones[pos_y[h_len], pos_x[h_len]];
            hand.GetComponent<RectTransform>().anchoredPosition3D = next.GetComponent<RectTransform>().anchoredPosition3D;
            if(next.GetComponent<Button>() != null)
            {
                next.GetComponent<Button>().interactable = true;
            }
            else
            {
                next.GetComponentInChildren<Button>().interactable = true;
            }
            float angle = -90 * k;
            while (angle < 0)
            {
                angle += speed * Time.deltaTime;
                try
                {
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
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public override bool checkPipes(out List<GameObject> list_results, out List<int> list_ds)
    {
        text_tutorial.text = text_content[++h_len];
        Destroy(hand);
        return base.checkPipes(out list_results, out list_ds);
    }

    public override void destroy()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                Destroy(m_clones[i, j]);
            }
        }
        text_tutorial.enabled = false;
    }

    
}
