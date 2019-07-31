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
    private int ads_c1;
    private int ads_c2;

    public bool canShowRatePanel()
    {
        if (!GameData.Instance.isRateOn) return false;
        rate_c++;
        if(rate_c == 4)
        {
            rate_c = 0;
            return true;
        }
        return false;
    }
    public bool canShowAds1()
    {
        if (!GameData.Instance.isAdsOn) return false;
        ads_c1++;
        if(ads_c1 == 1)
        {
            ads_c1 = 0;
            return true;
        }
        return false;
    }

    public bool canShowAds2()
    {
        if (!GameData.Instance.isAdsOn) return false;
        ads_c2++;
        if (ads_c2 == 1)
        {
            ads_c2 = 0;
            return true;
        }
        return false;
    }
}
