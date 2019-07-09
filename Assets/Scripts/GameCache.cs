using UnityEngine;
using System.Collections;

public class GameCache
{
    public static GameCache Instance = new GameCache();
    private GameCache()
    {
        //rate_c = 4;
        //ads_c = 0;
    }
    public int level_selected;
    public int mode;
    private int rate_c;
    private int ads_c;
    public bool canShowRatePanel()
    {
        if (!GameData.Instance.rate) return false;
        rate_c++;
        if(rate_c == 3)
        {
            rate_c = 0;
            return true;
        }
        return false;
    }
    public bool canShowAds()
    {
        if (!GameData.Instance.ads_on) return false;
        ads_c++;
        if(ads_c == 2)
        {
            ads_c = 0;
            return true;
        }
        return false;
    }
}
