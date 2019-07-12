using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class ChallengeModeController : GameController
{
    private int[,] m_pipes;
    

    public override int getStar()
    {
        return 3;
    }

    public override void loadLevelData()
    {
        var textAsset = Resources.Load<TextAsset>("levels/daily/" + GameCache.Instance.level_selected + "/" + GameData.Instance.day);
        Debug.Log(textAsset.text);
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
        m_clones = new GameObject[row, col];
        PlayZone.transform.localScale = new Vector3(1f, 1f, 1f);
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
                    position.y = ((row / 2 - i) - 0.5f) * 1.4f;
                    position.x = ((j - col / 2) + 0.5f) * 1.4f;
                    Quaternion rotation;
                    if (index == 4 || index == 5)
                    {
                        rotation = Quaternion.Euler(0f, 0f, -angle * 90);
                        GameObject go = Instantiate(pipes[index], position, rotation, PlayZone.transform);
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
                        GameObject go = Instantiate(pipes[index], position, rotation,PlayZone.transform);
                        go.GetComponent<PipeProperties>().row = i;
                        go.GetComponent<PipeProperties>().col = j;
                        go.GetComponent<PipeProperties>().rotation = angle;
                        m_clones[i, j] = go;
                        StartCoroutine(rotatePipe(go, 1, rotate_speed));
                    }
                }
            }
        }
        scale = 4.0f / col;
        PlayZone.transform.localScale = new Vector3(scale, scale, scale);

    }
}
