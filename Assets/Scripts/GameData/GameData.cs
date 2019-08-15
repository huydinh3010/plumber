﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using ImoSysSDK.SocialPlatforms;

[System.Serializable]
public class GameData
{

    public static GameData Instance = new GameData();
    private const string DATA_FILE_NAME = "player.dat";
    private GameData()
    {

    }
    public int unlockLevel;
    public int points;
    public int coins;
    public List<int> listLevelStars;
    public int dayOfDailyChallenge;
    public int[] dailyChallengeProgess;
    public int dailyChallengeRewardCoin;
    public bool isAdsOn;
    public bool isSoundOn;
    public long lastDayAccess;
    public long lastFbShare;
    public long lastWatchVideo;
    public int continueDay;
    public bool dailyRewardStatus;
    public bool challengeRewardStatus;
    public bool isRateOn;
    public int achievementProgress;
    public int watchVideoRemain;
    public LevelState unlockLvState;
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
        try
        {
            GameServices.Instance.UpdateScore(GameConfig.LEADERBROAD_ID, points, (success) =>
            {
                //Debug.Log("IMO update leaderboard: " + success);
            });
        }
        catch
        {

        }
    }

    public bool decreasePoint(int value)
    {
        if (points > value)
        {
            points -= value;
            EventDispatcher.Instance.PostEvent(EventID.OnPointChange, null);
            try
            {
                GameServices.Instance.UpdateScore(GameConfig.LEADERBROAD_ID, points, (success) =>
                {
                    //Debug.Log("IMO update leaderboard: " + success);
                });
            }
            catch
            {

            }
            return true;
        }
        return false;
    }

    private void updateDay()
    {
        int diff = (System.DateTime.Now.Date - System.DateTime.FromFileTime(lastDayAccess).Date).Days;
        if (diff > 0)
        {
            if (diff == 1 && continueDay < 5) continueDay++;
            else continueDay = 1;
            dailyRewardStatus = false;
            challengeRewardStatus = false;
            dailyChallengeProgess = new int[8];
            watchVideoRemain = GameConfig.WATCH_VIDEO_LIMIT;
            lastWatchVideo = System.DateTime.Now.AddDays(-2).ToFileTime();
            lastFbShare = System.DateTime.Now.AddDays(-2).ToFileTime();
            dayOfDailyChallenge = dayOfDailyChallenge + diff;
            if (dayOfDailyChallenge > 366) dayOfDailyChallenge = dayOfDailyChallenge % 367 + 1;
            lastDayAccess = System.DateTime.Now.Date.ToFileTime();
        }
    }

    public void LoadData()
    {
        if (PlayerPrefs.HasKey("GAME"))
        {
            string path = Application.persistentDataPath + "/" + DATA_FILE_NAME;
            if (File.Exists(path))
            {
                try
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    FileStream fileStream = new FileStream(path, FileMode.Open);
                    string data = binaryFormatter.Deserialize(fileStream) as string;
                    fileStream.Close();
                    Instance = JsonUtility.FromJson<GameData>(data);
                    Instance.updateDay();
                    for (int i = GameConfig.ACHIEVEMENT_CONDITION_POINT.Length - 1; i >= 0; i--)
                    {
                        if (Instance.points >= GameConfig.ACHIEVEMENT_CONDITION_POINT[i])
                        {
                            GameCache.Instance.unlockAchievementProgress = i + 1;
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    newData();
                }
            }
            else
            {
                newData();
            }
            GameCache.Instance.levelSelected = Instance.unlockLevel;
        }
        else
        {
            PlayerPrefs.SetInt("GAME", 1);
            newData();
            SaveDataToFile();
        }
    }

    private void newData()
    {
        listLevelStars = new List<int>();
        listLevelStars.Add(0);
        unlockLevel = 1;
        points = 0;
        coins = 200;
        dayOfDailyChallenge = 1;
        continueDay = 1;
        dailyChallengeProgess = new int[8];
        dailyChallengeRewardCoin = 100;
        isAdsOn = true;
        isRateOn = true;
        isSoundOn = true;
        lastDayAccess = System.DateTime.Now.Date.ToFileTime();
        lastWatchVideo = System.DateTime.Now.AddDays(-2).ToFileTime();
        lastFbShare = System.DateTime.Now.AddDays(-2).ToFileTime();
        dailyRewardStatus = false;
        challengeRewardStatus = false;
        achievementProgress = 0;
        watchVideoRemain = GameConfig.WATCH_VIDEO_LIMIT;
        unlockLvState = new LevelState();
        GameCache.Instance.firstGameLoad = true;
    }

    public void SaveDataToFile()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + DATA_FILE_NAME;
        FileStream fileStream = new FileStream(path, FileMode.Create);
        string data = JsonUtility.ToJson(this);
        //Debug.Log("Save data: " + data);
        binaryFormatter.Serialize(fileStream, data);
        fileStream.Close();
    }
}

[System.Serializable]
public class LevelState
{
    public float durationSecs;
    public int turnCount;
    public int removePipeCount;
    public int constructPipeCount;
    public int[] listPipeTypes;
    public int[] listPipeRotations;

    public void NewLevelState()
    {
        durationSecs = 0f;
        constructPipeCount = 0;
        removePipeCount = 0;
        turnCount = 0;
        listPipeTypes = Array.Empty<int>();
        listPipeRotations = Array.Empty<int>();
    }

}

