using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class TutorialModeController : GameController
{
    [SerializeField] GameObject hand;
    [SerializeField] GameObject textTutorial;
    [SerializeField] GameObject handOnAddCoinBtn;
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
        strResults = new string[len];
        for (int i = 0; i < len; i++)
        {
            strResults[i] = arr[k++];
        }
    }

    public override void setupLevel()
    {
        turnCount = 0;
        durationSecs = 0f;
        stopTime = false;
        animPlaying = false;
        removePipeCount = 0;
        constructPipeCount = 0;
        endConstructPipe = false;
        h_len = 0;
        pos_x = new int[3];
        pos_y = new int[3];
        pos_y[0] = 2; pos_x[0] = 0;
        pos_y[1] = 2; pos_x[1] = 2;
        pos_y[2] = 0; pos_x[2] = 0;
        m_Clones = new GameObject[row, col];
        pipe_size = Mathf.Min(playZone.rect.width * 4 / 1000 / col, playZone.rect.height * 6 / 1500 / row) * 250;
        for (int i = 0; i < strResults.Length; i++)
        {
            string[] pairs = strResults[i].Split(' ');
            int y = int.Parse(pairs[0]);
            int x = int.Parse(pairs[1]);
            int angle = int.Parse(pairs[2]);
            Vector3 position;
            position.z = 0;
            position.y = ((row / 2 - y) - 0.5f) * pipe_size;
            position.x = ((x - col / 2) + 0.5f) * pipe_size;
            GameObject go = Instantiate(pipes[m_pipes[y, x] / 10 % 7 - 1], Vector3.zero, Quaternion.Euler(0f, 0f, -angle * 90), playZone.transform);
            go.GetComponent<RectTransform>().anchoredPosition3D = position;
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(pipe_size, pipe_size);
            if (go.GetComponent<Button>() != null)
            {
                go.GetComponent<Button>().onClick.AddListener(() => { OnPipeClick(go); });
                go.GetComponent<Button>().interactable = false;
            }
            go.GetComponent<PipeProperties>().row = y;
            go.GetComponent<PipeProperties>().col = x;
            go.GetComponent<PipeProperties>().rotation = angle;
            if (i == 0)
            {
                valve = go;
                valve.transform.Find("Valve_bg").eulerAngles = Vector3.zero;
                valve.GetComponentInChildren<Button>().interactable = false;
                valve.GetComponentInChildren<Button>().onClick.AddListener(()=> { OnValveClick(); });
            }
            m_Clones[y, x] = go;
        }
        for (int i = 0; i < pos_x.Length - 1; i++)
        {
            m_Clones[pos_y[i], pos_x[i]].transform.eulerAngles += new Vector3(0f, 0f, 90f);
        }
        m_Clones[pos_y[0], pos_x[0]].GetComponent<Button>().interactable = true;
        // hand
        hand = Instantiate(hand, Vector3.zero, Quaternion.identity, playZone.transform);
        hand.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(((pos_x[0] - col / 2) + 0.5f) * pipe_size, ((row / 2 - pos_y[0]) - 0.5f) * pipe_size, 0);
        hand.GetComponent<RectTransform>().sizeDelta = new Vector2(pipe_size, pipe_size);
        handOnAddCoinBtn.SetActive(true); 
        //
        textTutorial.SetActive(true);
        textTutorial.GetComponent<Text>().text = text_content[0];
    }

    public override IEnumerator rotatePipe(GameObject gameObject, int k, float speed)
    {
        if (k == 1)
        {
            gameObject.GetComponent<Button>().interactable = false;
            textTutorial.GetComponent<Text>().text = text_content[++h_len];
            GameObject next = m_Clones[pos_y[h_len], pos_x[h_len]];
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
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public override bool checkPipes(out List<GameObject> list_results, out List<int> list_ds)
    {
        textTutorial.GetComponent<Text>().text = text_content[++h_len];
        Destroy(hand);
        return base.checkPipes(out list_results, out list_ds);
    }

    public override void resizeObjectWithPlayZone()
    {
        base.resizeObjectWithPlayZone();
        hand.GetComponent<RectTransform>().anchoredPosition3D = m_Clones[pos_y[h_len], pos_x[h_len]].GetComponent<RectTransform>().anchoredPosition3D;
    }

    public override void destroy()
    {
        textTutorial.SetActive(false);
        handOnAddCoinBtn.SetActive(false);
        base.destroy();
    }
}
