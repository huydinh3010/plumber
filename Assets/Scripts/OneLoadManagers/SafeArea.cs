using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeArea : MonoBehaviour
{
    [SerializeField] RectTransform outSideSafeAreaTop;
    // Start is called before the first frame update
    void Start()
    {
        RectTransform safeArea = GetComponent<RectTransform>();
        Rect screenSafeArea = Screen.safeArea;
        Vector2 anchorMin = screenSafeArea.position;
        Vector2 anchorMax = screenSafeArea.position + screenSafeArea.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;
        safeArea.anchorMin = anchorMin;
        safeArea.anchorMax = anchorMax;
        if(outSideSafeAreaTop != null)
        {
            float height = Screen.height - Screen.height * anchorMax.y;
            outSideSafeAreaTop.sizeDelta = new Vector2(outSideSafeAreaTop.sizeDelta.x, height);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
