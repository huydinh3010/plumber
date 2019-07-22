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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
