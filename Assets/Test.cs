using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    public GameObject go;
    // Start is called before the first frame update
    void Start()
    {

        Debug.Log("width: " + go.GetComponent<RectTransform>().rect.width + " & height: " + go.GetComponent<RectTransform>().rect.height);
    }

    // Update is called once per frame
    void Update()
    {
        go.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(120, 120);
        go.GetComponent<RectTransform>().sizeDelta = new Vector3(250, 250);
        go.transform.eulerAngles += new Vector3(0f, 0f, Time.deltaTime * 50);
    }
}
