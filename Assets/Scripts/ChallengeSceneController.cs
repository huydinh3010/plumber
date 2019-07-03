using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeSceneController : MonoBehaviour
{

    private GameObject selected;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void btnBackOnClick()
    {

    }

    public void btnAddCoinOnClick()
    {

    }

    public void onClickGameObject()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 postion = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D raycast = Physics2D.Raycast(postion, Vector2.zero);
            if (raycast.collider != null) selected = raycast.collider.gameObject;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Vector2 postion = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D raycast = Physics2D.Raycast(postion, Vector2.zero);
            if (raycast.collider != null && raycast.collider.gameObject == selected)
            {
                Debug.Log(selected.name);
            }
            selected = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        onClickGameObject();
    }
}
