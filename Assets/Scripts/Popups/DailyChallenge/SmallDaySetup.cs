using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SmallDaySetup : MonoBehaviour, IDayUIDailyRewardSetup
{
    [SerializeField] GameObject textCoinTop;
    [SerializeField] GameObject imageCoin;
    [SerializeField] GameObject imageEffect;
    [SerializeField] GameObject textCoinMiddle;
    [SerializeField] GameObject imageTick;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetNormalState()
    {
        textCoinTop.SetActive(true);
        imageCoin.SetActive(true);
        imageEffect.SetActive(false);
        textCoinMiddle.SetActive(false);
        imageTick.SetActive(false);
        GetComponent<Button>().enabled = false;
        GetComponent<Animator>().Play("Idle");
    }

    public void SetActiveState()
    {
        textCoinTop.SetActive(false);
        imageCoin.SetActive(false);
        imageEffect.SetActive(true);
        textCoinMiddle.SetActive(true);
        imageTick.SetActive(false);
        GetComponent<Button>().enabled = true;
        GetComponent<Animator>().Play("Scale");
    }

    public void SetPassedState()
    {
        textCoinTop.SetActive(false);
        imageCoin.SetActive(false);
        imageEffect.SetActive(true);
        textCoinMiddle.SetActive(true);
        imageTick.SetActive(true);
        GetComponent<Button>().enabled = false;
        GetComponent<Animator>().Play("Idle");
    }
}
