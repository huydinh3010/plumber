using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemLeaderBroadSetup : MonoBehaviour
{
    [SerializeField] Text textName;
    [SerializeField] Text textScore;
    [SerializeField] Image imageFlag;
    [SerializeField] Text textRank;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setup(int rank, string name, int score, string country_code) 
    {
        textRank.text = rank.ToString();
        textName.text = name;
        textScore.text = score.ToString();
        country_code = (country_code != null && country_code.Length > 0) ? country_code : "zz";
        imageFlag.sprite = Resources.Load<Sprite>("Flags/" + country_code);
    }
}
