using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerButton : MonoBehaviour
{
    public Animation anim;
    public Animator animator;
    AnimationClip animationClip;
    private int[] data = { };
    public Sprite pipe;

    private void Awake()
    {

        Vector3 position = new Vector3(0, 0, 0);
        animator.bodyPosition = position;
        animator.Play("None", 0 , 0.25f);
        Debug.Log(position.ToString());
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
