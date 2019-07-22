using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    public float speed;
    public Image transitionImage;
    public Canvas canvas;
    private Image clone;
    private float alpha;
    private bool closing;
    private bool opening;
    private string nextSceneName;

    public void loadScene(string name)
    {
        if (!closing)
        {
            alpha = 0f;
            closing = true;
            nextSceneName = name;
            clone = Instantiate(transitionImage, Vector3.zero, Quaternion.identity, canvas.transform);
            clone.color = new Color(1, 1, 1, 0);
            
            StartCoroutine(playCloseEffect());
        }
    }

    public void openScene()
    {
        if (!opening)
        {
            alpha = 1f;
            opening = true;
            clone = Instantiate(transitionImage, Vector3.zero, Quaternion.identity, canvas.transform);
            clone.color = new Color(1, 1, 1, 1);
            
            StartCoroutine(playOpenEffect());
        }
    }

    IEnumerator playCloseEffect()
    {
        while (alpha < 1 )
        {
            alpha += speed * Time.deltaTime;
            if(alpha < 1)
            {
                clone.color = new Color(1, 1, 1, alpha);
            }
            else
            {
                clone.color = new Color(1, 1, 1, 1);
                SceneManager.LoadScene(nextSceneName);
            }
            yield return 1;
        }
        
    }

    IEnumerator playOpenEffect()
    {
        while (alpha > 0.1f)
        {
            alpha -= speed * Time.deltaTime ;
            if (alpha > 0.1f)
            {
                clone.color = new Color(1, 1, 1, alpha);
            }
            else
            {
                clone.color = new Color(1, 1, 1, 0);
            }
            yield return 1;
        }
        Destroy(clone);
    }

    //private void playCloseEffect()
    //{
    //    alpha += speed * Time.deltaTime;
    //    if (alpha > 1f)
    //    {
    //        if (!loading)
    //        {
    //            loading = true;
    //            SceneManager.LoadScene(nextSceneName);
    //        }
    //        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 1);
           
    //    }
    //    else
    //    {
    //        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
    //    }
    //    GUI.depth = -10;
    //    GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), transitionTexture);
       
    //}

    //private void playOpenEffect()
    //{
    //    alpha -= speed * Time.deltaTime;
    //    if (alpha < 0)
    //    {
    //        opening = false;
    //    }
    //    else
    //    {
    //        GUI.depth = -10;
    //        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
    //        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), transitionTexture);
    //    }
    //}

    //private void OnGUI()
    //{
    //    if (closing)
    //    {
    //        playCloseEffect();
    //    }
    //    else if (opening)
    //    {
    //        playOpenEffect();
    //    }
    //}

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
