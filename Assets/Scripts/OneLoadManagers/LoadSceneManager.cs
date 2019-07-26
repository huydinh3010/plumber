using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
public class LoadSceneManager : MonoBehaviour
{
    public static LoadSceneManager Instance;

    public float speed = 3f;
    public Image transitionImage;
    private float alpha;
    private bool closing;
    private bool opening;
    private string nextSceneName;

    private void Awake()
    {
        Instance = Instance == null ? this : Instance;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadScene(string name)
    {
        if (!closing)
        {
            alpha = 0f;
            closing = true;
            nextSceneName = name;
            transitionImage.gameObject.SetActive(true);
            transitionImage.color = new Color(1, 1, 1, 0);
            StartCoroutine(playCloseEffect());
        }
    }

    public void OpenScene()
    {
        if (!opening)
        {
            alpha = 1f;
            opening = true;
            transitionImage.gameObject.SetActive(true);
            transitionImage.color = new Color(1, 1, 1, 1);
            StartCoroutine(playOpenEffect());
        }
    }

    IEnumerator playCloseEffect()
    {
        while (alpha < 1)
        {
            alpha += speed * Time.deltaTime;
            transitionImage.color = new Color(1, 1, 1, alpha < 1 ? alpha : 1);
            yield return 1;
        }
        SceneManager.LoadScene(nextSceneName);
        closing = false;
    }

    IEnumerator playOpenEffect()
    {
        while (alpha > 0.1f)
        {
            alpha -= speed * Time.deltaTime;
            transitionImage.color = new Color(1, 1, 1, alpha > 0.1f ? alpha : 0);
            yield return 1;
        }
        opening = false;
        transitionImage.gameObject.SetActive(false);
    }
}
