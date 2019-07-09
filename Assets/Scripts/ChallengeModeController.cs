using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ChallengeModeController : GameController
{
    private int[,] m_pipes;
    

    public override int getStar()
    {
        return 3;
    }

    public override void loadLevelData()
    {
        string path = "Assets/Resources/levels/daily/" + GameCache.Instance.level_selected + "/" + GameData.Instance.day + ".txt";
        //Debug.Log(path);
        StreamReader reader = new StreamReader(path, true);
        int.Parse(reader.ReadLine());
        
        row = int.Parse(reader.ReadLine());
        col = int.Parse(reader.ReadLine());
        m_pipes = new int[row, col];
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                m_pipes[i, j] = int.Parse(reader.ReadLine());
            }
        }
        int len = int.Parse(reader.ReadLine());
        str_results = new string[len];
        for (int i = 0; i < len; i++)
        {
            str_results[i] = reader.ReadLine();
        }
        reader.Close();
    }

    

    public override void setupLevel()
    {
        m_clones = new GameObject[row, col];
        System.Random rd = new System.Random();
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (m_pipes[i, j] != 70)
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
                        if (index == 4)
                        {
                            valve = go;
                            valve.GetComponentsInChildren<Transform>()[2].eulerAngles = Vector3.zero;
                        }
                    }
                    else
                    {
                        angle = rd.Next(0, 3);
                        rotation = Quaternion.Euler(0f, 0f, -angle * 90);
                        //StartCoroutine(rotatePipe(gameObject));
                        GameObject go = Instantiate(pipes[index], position, rotation);
                        go.transform.localScale = new Vector3(scale, scale, scale);
                        go.GetComponent<PipeProperties>().row = i;
                        go.GetComponent<PipeProperties>().col = j;
                        go.GetComponent<PipeProperties>().rotation = angle;
                        m_clones[i, j] = go;
                        StartCoroutine(rotatePipe(go, 1, rotate_speed));
                    }
                }
            }
        }
    }
}
