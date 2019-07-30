using UnityEngine;
using System.Collections;

public class GameCache
{
    public static GameCache Instance = new GameCache();
    private GameCache()
    {

    }
    public int levelSelected;
    public int mode;
    public bool firstGameLoad;
    public bool lastLevel;

    private int rate_c;
    private int ads_c;

    public bool canShowRatePanel()
    {
        if (!GameData.Instance.isRateOn) return false;
        rate_c++;
        if(rate_c == 8)
        {
            rate_c = 0;
            return true;
        }
        return false;
    }
    public bool canShowAds()
    {
        if (!GameData.Instance.isAdsOn) return false;
        ads_c++;
        if(ads_c == 6)
        {
            ads_c = 0;
            return true;
        }
        return false;
    }
}
