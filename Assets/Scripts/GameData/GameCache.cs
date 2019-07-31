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
}
