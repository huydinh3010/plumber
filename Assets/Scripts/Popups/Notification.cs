using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class Notification : MonoBehaviour
{
    [SerializeField] Text textContent;
    [SerializeField] Image image;
    [SerializeField] float timeDuration;
    private Action btn_Callback;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show(string content, Sprite s_image, float duration, Action action = null)
    {
        textContent.text = content;
        if (s_image == null)
        {
            image.enabled = false;
        }
        else
        {
            image.enabled = true;
            image.sprite = s_image;
        }
        this.timeDuration = duration;
        btn_Callback = action;
        StartCoroutine(playAnimation());
    }

    public void OnHided()
    {
        GetComponent<RectTransform>().gameObject.SetActive(false);
    }

    IEnumerator playAnimation()
    {
        GetComponent<Animator>().Play("Show");
        yield return new WaitForSeconds(timeDuration);
        GetComponent<Animator>().Play("Hide");
    }

    public void btnOnClick()
    {
        btn_Callback?.Invoke();
        btn_Callback = null;
    }
}
