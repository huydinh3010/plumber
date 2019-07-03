using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class demo : MonoBehaviour
{
    public SpriteRenderer sprite;
    private SpriteRenderer[] clone_sp;
    private SpriteRenderer[] sp_anim;
    public GameObject sp_obj;
    float speed = 1f;
    int index = 0;
    bool running = false;

    //private void Awake()
    //{
    //    sp_anim = sp_obj.GetComponentsInChildren<SpriteRenderer>();
    //    Debug.Log(sp_anim.Length);
        

    //}

    public static void next()
    {

    }

    void Start()
    {
        StartCoroutine(Example());
        //StartCoroutine(Example());
        ////clone_sp = new SpriteRenderer[4];
        ////for(int i = 0; i < 4; i++)
        ////{
        ////    clone_sp[i] = Instantiate(sprite, new Vector3(0f, i - 2, 0f), new Quaternion(0f, 0f, 0f, 0f));
        ////}

    }

    private void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
        //    if (hit.collider != null)
        //    {
        //        Debug.Log(hit.collider.tag);
        //    }
        //}
        //float cz = sp_obj.transform.eulerAngles.z;
        //sp_obj.transform.eulerAngles = new Vector3(0, 0, cz + speed);

        for(int i = 0; i < 4; i++)
        {
            Debug.Log(name + ": " + i);
        }
            
           

    }

    IEnumerator Example()
    {
        while (true)
        {
            for (int i = 0; i < 4; i++)
            {
                Debug.Log("Example: " + i);

            }
            yield return 0;
        }
        
    }

    //IEnumerator Example()
    //{
    //    while (true)
    //    {
    //        if (index < sp_anim.Length - 1)
    //        {
    //            sp_anim[index].sortingOrder = 0;
    //            sp_anim[++index].sortingOrder = 1;
    //        }
    //        else
    //        {
    //            sp_anim[index].sortingOrder = 0;
    //            index = 0;
    //            sp_anim[0].sortingOrder = 1;
    //        }
    //        yield return new WaitForSeconds(1);

    //    }

    //}


    //void OnGUI()
    //{
    //    ////Debug.Log("Current event detected: " + Event.current);
    //    ////Use the Sliders to manipulate the RGB component of Color
    //    ////Use the Label to identify the Slider
    //    //GUI.Label(new Rect(0, 30, 50, 30), "Red: ");
    //    ////Use the Slider to change amount of red in the Color
    //    //m_Red = GUI.HorizontalSlider(new Rect(35, 25, 200, 30), m_Red, 0, 1);

    //    ////The Slider manipulates the amount of green in the GameObject
    //    //GUI.Label(new Rect(0, 70, 50, 30), "Green: ");
    //    //m_Green = GUI.HorizontalSlider(new Rect(35, 60, 200, 30), m_Green, 0, 1);

    //    ////This Slider decides the amount of blue in the GameObject
    //    //GUI.Label(new Rect(0, 105, 50, 30), "Blue: ");
    //    //m_Blue = GUI.HorizontalSlider(new Rect(35, 95, 200, 30), m_Blue, 0, 1);

    //    ////Set the Color to the values gained from the Sliders
    //    //m_NewColor = new Color(m_Red, m_Green, m_Blue);

    //    ////Set the SpriteRenderer to the Color defined by the Sliders
    //    //m_SpriteRenderer.color = m_NewColor;

    //    //float cz = m_SpriteRenderer.transform.eulerAngles.z;
    //    //m_SpriteRenderer.transform.eulerAngles = new Vector3(0, 0, cz + speed) ;
        
    //    //if (sp.sortingOrder-- < -60) sp.sortingOrder = 60;
        
        

    //}
}
