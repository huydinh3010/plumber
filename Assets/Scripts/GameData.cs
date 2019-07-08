using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    
    public static GameData Instance = new GameData();
    private GameData()
    {
        LoadDataFromFile();
    }
    public int unlock_level;
    public int points;
    public int coins;
    public int[] level_stars;
    public int day;
    public int[] completed;
    public int reward_coins;
    public bool ads_on;
    public bool sound_on;

    public bool firstMenuLoad;
    public int level_selected;
    public int pack_selected;
    public int mode;

    public void increaseCoin(int value)
    {
        coins += value;
        EventDispatcher.Instance.PostEvent(EventID.OnCoinChange, null);
    }

    public bool decreaseCoin(int value)
    {
        if (coins > value)
        {
            coins -= value;
            EventDispatcher.Instance.PostEvent(EventID.OnCoinChange, null);
            return true;
        }
        return false;
    }

    public void increasePoint(int value)
    {
        points += value;
        EventDispatcher.Instance.PostEvent(EventID.OnPointChange, null);
    }

    public bool decreasePoint(int value)
    {
        if (points > value)
        {
            points -= value;
            EventDispatcher.Instance.PostEvent(EventID.OnPointChange, null);
            return true;
        }
        return false;
    }

    

    public void LoadDataFromFile()
    {
        // test game
        unlock_level = 558;
        points = 2500;
        coins = 1000;
        level_stars = new int[560];
        System.Random rd = new System.Random();
        for (int i = 0; i < level_stars.Length; i++)
        {
            if (i >= unlock_level) level_stars[i] = -1;
            else if (i == unlock_level - 1) level_stars[i] = 0;
            else
            {
                level_stars[i] = rd.Next(1, 3);
            }
        }
        day = 2;
        completed = new int[8] {0,1,0,0,0,0,0,0};
        reward_coins = 100;
        ads_on = true;
        firstMenuLoad = true;
        //
    }

    public void SaveDataToFile()
    {
        
    }
}
