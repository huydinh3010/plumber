using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onclick : MonoBehaviour
{

    float speed = 2f;
    bool rotating = false;


    private void OnMouseUp()
    {
        Debug.Log("Mouse clicked");
        rotating = true;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (rotating)
        {
            Transform transform = GetComponent<Transform>();
            float current_axisz = transform.eulerAngles.z;
            transform.eulerAngles = new Vector3(0f, 0f, current_axisz + speed);
            
        }
    }
}
