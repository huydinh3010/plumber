﻿using UnityEngine;
using System.Collections;
using ImoSysSDK.SocialPlatforms;
public class GameCache
{
    public static GameCache Instance = new GameCache();
    private GameCache()
    {

    }
    public int simpleLevelSelected;
    public int challengeLevelSelected;
    public int mode;
    public bool firstGameLoad;
    public bool lastLevel;
    public bool showAchievement;
    public int unlockAchievementProgress;
    public bool avatarLoaded;
    public int showAddCoinCount;
    public bool connectedToServer;
}
