using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollviewLevelController : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] Button btnArrowLeft;
    [SerializeField] Button btnArrowRight;
    [SerializeField] Text txtPackNumber;
    [SerializeField] CanvasGroup canvasContent;
    [SerializeField] float decelerationRate = 10f;
    private RectTransform content;
    private ScrollRect scroll;
    private bool lerp;
    private Vector2 target;

    public void OnBeginDrag(PointerEventData eventData)
    {
        lerp = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        lerp = true;
        float vx = scroll.velocity.x;
        float content_x = scroll.content.anchoredPosition.x;
        if (vx > 50)
        {
            target = new Vector2(Mathf.Clamp(((int)(-600 - content_x) / 1100) * -1100 - 600, -38000, -600), 0f);
        }
        else if (vx < -50)
        {
            target = new Vector2(Mathf.Clamp(((int)(-600 - content_x) / 1100 + 1) * -1100 - 600, -38000, -600), 0f);
        }
        else
        {
            target = new Vector2(Mathf.Clamp(((int)(-50 - content_x) / 1100) * -1100 - 600, -38000, -600), 0f);
        }
        txtPackNumber.text = "PACK " + ((int)(-600 - target.x) / 1100 + 1).ToString() + "/35";
    }
   
    private void btnArrowLeftOnClick()
    {
        AudioManager.Instance.Play(AudioManager.SoundName.BUTTON);
        target = new Vector2(Mathf.Clamp(((int)(-50 - scroll.content.anchoredPosition.x) / 1100 - 1) * -1100 - 600, -38000, -600), 0f);
        txtPackNumber.text = "PACK " + ((int)(-600 - target.x) / 1100 + 1).ToString() + "/35";
        lerp = true;
    }

    private void btnArrowRightOnClick()
    {
        AudioManager.Instance.Play(AudioManager.SoundName.BUTTON);
        target = new Vector2(Mathf.Clamp(((int)(-50 - scroll.content.anchoredPosition.x) / 1100 + 1) * -1100 - 600, -38000, -600), 0f);
        txtPackNumber.text = "PACK " + ((int)(-600 - target.x) / 1100 + 1).ToString() + "/35";
        lerp = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        scroll = GetComponent<ScrollRect>();
        content = scroll.content;
        btnArrowRight.onClick.AddListener(() => { btnArrowRightOnClick(); });
        btnArrowLeft.onClick.AddListener(() => { btnArrowLeftOnClick(); });
        int startPage = (GameCache.Instance.levelSelected - 1) / 16;
        content.anchoredPosition = new Vector3(-startPage * 1100 - 600, 0, 0);
        txtPackNumber.text = "PACK " + (startPage + 1).ToString() + "/35";
    }

    // Update is called once per frame
    void Update()
    {
        if (lerp)
        {
            float decelerate = Mathf.Min(decelerationRate * Time.deltaTime, 1f);
            scroll.content.anchoredPosition = Vector2.Lerp(scroll.content.anchoredPosition, target, decelerate);
            if (Vector2.SqrMagnitude(scroll.content.anchoredPosition - target) < 0.25f)
            {
                scroll.velocity = Vector2.zero;
                scroll.content.anchoredPosition = target;
                lerp = false;
            }
        }
        //effect
        int x = (int)Mathf.Abs(content.anchoredPosition.x + 600) % 1100;
        if (x < 200)
        {
            scroll.transform.localScale = new Vector3(1 - 0.00075f * x, 1 - 0.00075f * x, 1);
            canvasContent.alpha = 1 - 0.0015f * x;
        }
        else if (x > 900)
        {
            scroll.transform.localScale = new Vector3(0.00075f * x + 0.175f, 0.00075f * x + 0.175f, 1);
            canvasContent.alpha = 0.0015f * x - 0.65f;
        }
        else
        {
            scroll.transform.localScale = new Vector3(0.85f, 0.85f, 1);
            canvasContent.alpha = 0.7f;
        }
        //
    }
}
