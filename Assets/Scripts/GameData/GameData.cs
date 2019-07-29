using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

[System.Serializable]
public class GameData
{
    public static GameData Instance = new GameData();
    private GameData()
    {
        
    }
    public int unlock_level;
    public int points;
    public int coins;
    public List<int> level_stars;
    public int day;
    public int[] completed;
    public int reward_coins;
    public bool ads_on;
    public bool sound_on;
    public long lastDayAccess;
    public int continueDay;
    public bool clampDailyReward;
    public bool clampChallengeReward;
    public bool rate;
    public int achievement_progress;
    public LevelState unlocklv_state;
    public void increaseCoin(int value)
    {
        coins += value;
        EventDispatcher.Instance.PostEvent(EventID.OnCoinChange, null, value);
        AudioManager.Instance.Play("coins_reward");
    }

    public bool decreaseCoin(int value)
    {
        if (coins >= value)
        {
            coins -= value;
            EventDispatcher.Instance.PostEvent(EventID.OnCoinChange, null, -value);
            AudioManager.Instance.Play("coins_decrease");
            return true;
        }
        return false;
    }

    public void increasePoint(int value)
    {
        points += value;
        EventDispatcher.Instance.PostEvent(EventID.OnPointChange, null, value);
        
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

    private void updateDay()
    {
        int diff = (System.DateTime.Now.Date - System.DateTime.FromFileTime(lastDayAccess).Date).Days;
        if(diff > 0)
        {
            if (diff == 1 && continueDay < 5) continueDay++;
            else continueDay = 1;
            clampDailyReward = false;
            clampChallengeReward = false;
            completed = new int[8];
            day = day + diff;
            if (day > 366) day = day % 367 + 1;
            lastDayAccess = System.DateTime.Now.Date.ToFileTime();
        }
        
    }

    public void LoadDataFromFile()
    {
        string path = Application.persistentDataPath + "/user.data";
        if (File.Exists(path))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(path, FileMode.Open);
            string data = binaryFormatter.Deserialize(fileStream) as string;
            fileStream.Close();
            Instance = JsonUtility.FromJson<GameData>(data);
            Instance.updateDay();
        }
        else
        {
            level_stars = new List<int>();
            level_stars.Add(0);
            unlock_level = 1;
            points = 0;
            coins = 200;
            day = 1;
            continueDay = 1;
            completed = new int[8];
            reward_coins = 100;
            ads_on = true;
            rate = true;
            sound_on = true;
            lastDayAccess = System.DateTime.Now.Date.ToFileTime();
            clampDailyReward = false;
            clampChallengeReward = false;
            achievement_progress = 0;
            unlocklv_state = new LevelState();
            GameCache.Instance.firstGameLoad = true;
        }
        GameCache.Instance.level_selected = unlock_level;
    }

    public void SaveDataToFile()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/user.data";
        FileStream fileStream = new FileStream(path, FileMode.Create);
        string data = JsonUtility.ToJson(this);
        Debug.Log("Save data: " + data);
        binaryFormatter.Serialize(fileStream, data);
        fileStream.Close();
    }
}


[System.Serializable]
public class LevelState
{
    public float durations;
    public int turn_count;
    public int remove_pipe;
    public int construct_pipe;
    public int[] pipes_type;
    public int[] pipes_rotation;

    public void newLevel()
    {
        durations = 0f;
        construct_pipe = 0;
        remove_pipe = 0;
        turn_count = 0;
        pipes_type = Array.Empty<int>();
        pipes_rotation = Array.Empty<int>();
    }

}

