using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class HelpLevelController : GameController
{
    public GameObject hand;
    public Text h_text;

    private int h_len;
    private int[] pos_x;
    private int[] pos_y;
    private int[,] m_pipes;
    private string[] text_content = { "Touch the pipes to turn them.", "Make a water path from value to container.", "Open the valve!", "Good job!" };


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

    public override void setupLevel()
    {
        duration_secs = 0f;
        stop_time = false;
        h_len = 0;
        pos_x = new int[3];
        pos_y = new int[3];
        pos_y[0] = 2; pos_x[0] = 0;
        pos_y[1] = 2; pos_x[1] = 2;
        pos_y[2] = 0; pos_x[2] = 0;

        
        m_clones = new GameObject[row, col];
        // hand
        hand = Instantiate(hand, new Vector3(((pos_x[0] - col / 2) + 0.5f) * 1.4f, ((row / 2 - pos_y[0]) - 0.5f) * 1.4f, 0), Quaternion.identity);
        //
        h_text.enabled = true;
        h_text.text = text_content[0];

        PlayZone.transform.localScale = new Vector3(1f, 1f, 1f);
        for (int i = 0; i < str_results.Length; i++)
        {
            string[] pairs = str_results[i].Split(' ');
            int y = int.Parse(pairs[0]);
            int x = int.Parse(pairs[1]);
            int angle = int.Parse(pairs[2]);
            Vector3 position;
            position.z = 0;
            position.y = ((row / 2 - y) - 0.5f) * 1.4f;
            position.x = ((x - col / 2) + 0.5f) * 1.4f;
            m_clones[y, x] = Instantiate(pipes[m_pipes[y, x] / 10 % 7 - 1], position, Quaternion.identity, PlayZone.transform);
            m_clones[y, x].transform.eulerAngles = new Vector3(0f, 0f, -angle * 90);
            if (m_clones[y, x].GetComponent<BoxCollider2D>() != null)
            {
                m_clones[y, x].GetComponent<BoxCollider2D>().enabled = false;
            }
            m_clones[y, x].GetComponent<PipeProperties>().row = y;
            m_clones[y, x].GetComponent<PipeProperties>().col = x;
            m_clones[y, x].GetComponent<PipeProperties>().rotation = angle;
            if (i == 0)
            {
                valve = m_clones[y, x];
                valve.GetComponentsInChildren<Transform>()[2].eulerAngles = Vector3.zero;
            }
        }
        for (int i = 0; i < pos_x.Length - 1; i++)
        {
            m_clones[pos_y[i], pos_x[i]].transform.eulerAngles += new Vector3(0f, 0f, 90f);
        }
        m_clones[pos_y[0], pos_x[0]].GetComponent<BoxCollider2D>().enabled = true;
        scale = 4.0f / col;
        PlayZone.transform.localScale = new Vector3(scale, scale, scale);
    }

    public override IEnumerator rotatePipe(GameObject gameObject, int k, float speed)
    {
        if (k == 1)
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            h_text.text = text_content[++h_len];
            hand.transform.position = new Vector3(((pos_x[h_len] - col / 2) + 0.5f) * 1.4f, ((row / 2 - pos_y[h_len]) - 0.5f) * 1.4f, 0);
            m_clones[pos_y[h_len], pos_x[h_len]].GetComponent<BoxCollider2D>().enabled = true;
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
        h_text.text = text_content[++h_len];
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
        h_text.enabled = false;
    }

    
}
