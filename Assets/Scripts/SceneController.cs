using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public float speed;
    public Texture2D transitionTexture;
    private float alpha;
    private bool closing;
    private bool opening;
    private string nextSceneName;

    public void loadScene(string name)
    {
        alpha = 0f;
        closing = true;
        nextSceneName = name;
    }

    public void openScene()
    {
        alpha = 1f;
        opening = true;
    }

    private void playCloseEffect()
    {
        alpha += speed * Time.deltaTime;
        if (alpha > 1f)
        {
            SceneManager.LoadScene(nextSceneName);
            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 1);
            //closing = false;
        }
        else
        {
            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
        }
        GUI.depth = -10;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), transitionTexture);
    }

    private void playOpenEffect()
    {
        alpha -= speed * Time.deltaTime;
        if (alpha < 0)
        {
            opening = false;
        }
        else
        {
            GUI.depth = -10;
            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), transitionTexture);
        }
    }

    private void OnGUI()
    {
        if (closing)
        {
            playCloseEffect();
        }
        else if (opening)
        {
            playOpenEffect();
        }
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
