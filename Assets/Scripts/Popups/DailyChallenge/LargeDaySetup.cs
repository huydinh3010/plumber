using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LargeDaySetup : MonoBehaviour , IDayUIDailyRewardSetup
{
    [SerializeField] GameObject chestClose;
    [SerializeField] GameObject chestOpen;
    [SerializeField] GameObject textCoin;
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
        chestClose.SetActive(true);
        chestOpen.SetActive(false);
        textCoin.SetActive(false);
        imageTick.SetActive(false);
        GetComponent<Button>().enabled = false;
    }

    public void SetActiveState()
    {
        chestClose.SetActive(false);
        chestOpen.SetActive(true);
        textCoin.SetActive(true);
        imageTick.SetActive(false);
        GetComponent<Button>().enabled = true;
    }

    public void SetPassedState()
    {
        chestClose.SetActive(false);
        chestOpen.SetActive(true);
        textCoin.SetActive(true);
        imageTick.SetActive(true);
        GetComponent<Button>().enabled = false;
    }
}
