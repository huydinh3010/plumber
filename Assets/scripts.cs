﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scripts : MonoBehaviour
{

    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.Play("cross_front");
        anim.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
