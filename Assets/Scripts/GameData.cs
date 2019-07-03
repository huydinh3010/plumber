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
    public int completed;
    public int reward_coins;
    public bool ads_on;

    public bool firstMenuLoad;
    private void LoadDataFromFile()
    {
        // test game
        unlock_level = 10;
        points = 250;
        coins = 235;
        level_stars = new int[560];
        System.Random rd = new System.Random();
        for (int i = 0; i < level_stars.Length; i++)
        {
            if (i > unlock_level) level_stars[i] = -1;
            else if (i == unlock_level) level_stars[i] = 0;
            else
            {
                level_stars[i] = rd.Next(1, 3);
            }
        }
        day = 2;
        completed = 4;
        reward_coins = 100;
        ads_on = true;
        firstMenuLoad = true;
        //
    }

    public void SaveDataToFile()
    {
        
    }
}
