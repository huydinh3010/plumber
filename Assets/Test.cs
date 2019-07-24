using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class Test : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public ScrollRect scroll;
    public int page;
    public float page_size;
    public float decelerationRate = 10f;
    private float target;
    private bool lerp;
    private float content_size;
    
    // Start is called before the first frame update
    void Start()
    {
        content_size = scroll.content.rect.width;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (lerp)
        {
            float decelerate = Mathf.Min(decelerationRate * Time.deltaTime, 1f);
            scroll.content.anchoredPosition = Vector2.Lerp(scroll.content.anchoredPosition, new Vector2(target, 0), decelerate);
            if (Vector2.SqrMagnitude(scroll.content.anchoredPosition - new Vector2(target, 0)) < 0.25f)
            {
                // snap to target and stop lerping
                scroll.velocity = Vector2.zero;
                scroll.content.anchoredPosition = new Vector2(target, 0);
                lerp = false;
                // clear also any scrollrect move that may interfere with our lerping
                
            }
        }
        
    }

    public void btnStopOnClick()
    {
        scroll.velocity = Vector2.zero;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        lerp = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        lerp = true;
        float vx = scroll.velocity.x;
        float content_x = scroll.content.anchoredPosition.x;
        Debug.Log("Velocity x = " + vx);
        if(vx > 10)
        {
            // keo sang phai
            target = Mathf.Clamp(((int)(content_x - 200) / 400 + (content_x > 0 ? 1 : 0)) * 400 + 200, -600, 600);
        }
        else if(vx < -10)
        {
            // keo sang trai
            target = Mathf.Clamp(((int)(content_x + 200) / 400 - (content_x < 0 ? 1 : 0)) * 400 - 200, -600, 600);
        }
        else
        {
            //int offset = Mathf.Abs((int)content_x % 400);
            //if(offset <= 200)
            //{
            
            target = Mathf.Clamp(((int)content_x / 400 - (content_x < 0 ? 1 : 0)) * 400 + 200, -600, 600);
            
            //}
            //else
            //{
            //    target = ((int)content_x / 400) * 400 + 200;
            //}
        }
    }

    

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log(scroll.velocity.x);
    }
}
