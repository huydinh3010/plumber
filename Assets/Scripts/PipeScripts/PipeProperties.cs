using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeProperties : MonoBehaviour
{
    public int type;
    [HideInInspector] public int row;
    [HideInInspector] public int col;
    [HideInInspector] public int rotation;
    [HideInInspector] public int n_Line;
    public int[] line;
    public string[] animState;
    public int[] animRotation;
    [HideInInspector] public int temp;
    [HideInInspector] public GameObject[] next = new GameObject[2];
    [HideInInspector] public int[] nextIn = new int[2];
}

