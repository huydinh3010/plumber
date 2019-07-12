using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollLevelController : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public Button btnArrowLeft;
    public Button btnArrowRight;
    public Text txtPackNumber;
    public float speed_movement;
    public Material material;
    private RectTransform content;
    private ScrollRect scroll;
    private int currentPage;
    private bool isMoving;
    

    public void OnBeginDrag(PointerEventData eventData)
    {
        //currentPage = -((int)content.localPosition.x + 300) / 600;
    
    }

    public void OnDrag(PointerEventData eventData)
    {
      
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float oldx = -currentPage * 600 - 400;
        if (!isMoving && (currentPage != 0 || content.anchoredPosition.x - oldx <= 0) && (currentPage != 34 || content.anchoredPosition.x - oldx >= 0))
        {
            if (System.Math.Abs(content.anchoredPosition.x - oldx) < 150)
            {
                scroll.StopMovement();
                StartCoroutine(ChangePage(0)); // back to current page
            }
            else if (content.anchoredPosition.x < oldx)
            {
                scroll.StopMovement();
                StartCoroutine(ChangePage(1)); // next page
            }
            else if (content.anchoredPosition.x > oldx)
            {
                scroll.StopMovement();
                StartCoroutine(ChangePage(-1)); // prev page
            }
        }
       
    }



    private void btnArrowLeftOnClick()
    {
        if (!isMoving && currentPage > 0) StartCoroutine(ChangePage(-1));
    }

    private void btnArrowRightOnClick()
    {
        if (!isMoving && currentPage < 34) StartCoroutine(ChangePage(1));
    }

    private IEnumerator ChangePage(int d_page)
    {
        isMoving = true;
       
        float dx = -(currentPage + d_page) * 600 - 400  - content.anchoredPosition.x;
        if (dx > 0)
        {
            while(true)
            {
                dx -= speed_movement*Time.deltaTime;
                if(dx < 0)
                {
                    content.anchoredPosition = new Vector3(-(currentPage + d_page) * 600 - 400, 0);
                    break;
                }
                else content.anchoredPosition += new Vector2(speed_movement * Time.deltaTime, 0);
                yield return 0;
            }
        }
        else if (dx < 0)
        {
            while (true)
            {
                dx += speed_movement* Time.deltaTime;
                if(dx > 0)
                {
                    content.anchoredPosition = new Vector3(-(currentPage + d_page) * 600 - 400, 0);
                    break;
                }
                else content.anchoredPosition -= new Vector2(speed_movement * Time.deltaTime, 0);
                yield return 0;
            }
            
        }
        currentPage = -((int)content.anchoredPosition.x + 400) / 600;
        txtPackNumber.text = "PACK " + (currentPage + 1).ToString() + "/35";
        isMoving = false;
        
    }

    // Start is called before the first frame update
    void Start()
    {
        scroll = GetComponent<ScrollRect>();
        content = scroll.content;
        if (speed_movement <= 0) speed_movement = 5000f;
        btnArrowRight.onClick.AddListener(() => { btnArrowRightOnClick(); });
        btnArrowLeft.onClick.AddListener(() => { btnArrowLeftOnClick(); });
        currentPage = (GameCache.Instance.level_selected - 1) / 16;
        content.anchoredPosition = new Vector3(-currentPage * 600 - 400, 0, 0);
        txtPackNumber.text = "PACK " + (currentPage + 1).ToString() + "/35";
    }

    // Update is called once per frame
    void Update()
    {
        int x = (int)System.Math.Abs(content.anchoredPosition.x + 400) % 600;
        if (x < 100)
        {
            scroll.transform.localScale = new Vector3(1 - 0.0015f * x, 1 - 0.0015f * x, 1);
            material.color = new Color(1f, 1f, 1f, 1 - 0.003f * x);
        }
        else if (x > 500)
        {
            scroll.transform.localScale = new Vector3(0.0015f * x + 0.1f, 0.0015f * x + 0.1f, 1);
            material.color = new Color(1f, 1f, 1f, 0.003f * x - 0.8f);
        }
        else
        {
            scroll.transform.localScale = new Vector3(0.85f, 0.85f, 1);
            material.color = new Color(1f, 1f, 1f, 0.7f);
        }
    }
}
