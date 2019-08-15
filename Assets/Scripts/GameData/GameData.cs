using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using ImoSysSDK.SocialPlatforms;

[System.Serializable]
public class GameData
{
    private class PlayerPrefKey
    {
        public const string GAME = "G0";
        public const string UNLOCK_LEVEL = "UL1";
        public const string POINTS = "P2";
        public const string COINS = "C3";
        public const string DAY_OF_DAILY_CHALLENGE = "DODC4";
        public const string ADS_ON = "AO5";
        public const string SOUND_ON = "SO6";
        public const string CONTINUE_DAY = "CD7";
        public const string DAILY_REWARD_STATUS = "DRS8";
        public const string CHALLENGE_REWARD_STATUS = "CRS8";
        public const string RATE_ON = "RO9";
        public const string ACHIEVEMENT_PROGRESS = "AP10";
        public const string WATCH_VIDEO_REMAIN = "WVR11";
        public const string DAILY_CHALLENGE_PROGESS = "DCP12";
        public const string DAILY_CHALLENGE_LENGTH = "DCL12";
        public const string LIST_LEVEL_STARS = "LLS13";
        public const string LEVEL_STARS_LENGTH = "LSL13";
        public const string LAST_DAY_ACCESS = "LDA14";
        public const string LAST_FB_SHARE = "LFS15";
        public const string LAST_WATCH_VIDEO = "LWV16";
    }

    public class LevelState
    {
        public static LevelState Instance = new LevelState();
        private class PlayerPrefKey
        {
            public const string DURATION_SECS = "DS17";
            public const string TURN_COUNT = "TC18";
            public const string REMOVE_PIPE_COUNT = "RPC19";
            public const string CONSTRUCT_PIPE_COUNT = "CPC20";
            public const string LIST_PIPE_TYPES = "LPT21";
            public const string LIST_PIPE_ROTATIONS = "LPR22";
            public const string LIST_LENGTH = "LL23";
        }

        private LevelState()
        {
            listPipeTypes = new ListInt(PlayerPrefKey.LIST_PIPE_TYPES, PlayerPrefKey.LIST_LENGTH);
            listPipeRotations = new ListInt(PlayerPrefKey.LIST_PIPE_ROTATIONS, PlayerPrefKey.LIST_LENGTH);
        }

        public float durationSecs
        {
            get
            {
                return PlayerPrefs.GetFloat(PlayerPrefKey.DURATION_SECS);
            }
            set
            {
                PlayerPrefs.SetFloat(PlayerPrefKey.DURATION_SECS, value);
            }
        }

        public int turnCount
        {
            get
            {
                return PlayerPrefs.GetInt(PlayerPrefKey.TURN_COUNT);
            }
            set
            {
                PlayerPrefs.SetInt(PlayerPrefKey.TURN_COUNT, value);
            }
        }

        public int removePipeCount
        {
            get
            {
                return PlayerPrefs.GetInt(PlayerPrefKey.REMOVE_PIPE_COUNT);
            }
            set
            {
                PlayerPrefs.SetInt(PlayerPrefKey.REMOVE_PIPE_COUNT, value);
            }
        }

        public int constructPipeCount
        {
            get
            {
                return PlayerPrefs.GetInt(PlayerPrefKey.CONSTRUCT_PIPE_COUNT);
            }
            set
            {
                PlayerPrefs.SetInt(PlayerPrefKey.CONSTRUCT_PIPE_COUNT, value);
            }
        }

        public ListInt listPipeTypes;

        public ListInt listPipeRotations;

        public void NewLevelState()
        {
            durationSecs = 0f;
            constructPipeCount = 0;
            removePipeCount = 0;
            turnCount = 0;
            listPipeTypes.Clear();
            listPipeRotations.Clear();
        }
    }

    public class ListInt
    {
        private string key;
        private string key_length;

        public ListInt(string key, string key_length)
        {
            this.key = key;
            this.key_length = key_length;
        }
        public int Count
        {
            get
            {
                return PlayerPrefs.GetInt(key_length);
            }
        }

        public void Add(int value)
        {
            int length = PlayerPrefs.GetInt(key_length);
            PlayerPrefs.SetInt(key + "_" + length, value);
            PlayerPrefs.SetInt(key_length, length + 1);
        }

        public int this[int i]
        {
            get
            {
                return PlayerPrefs.GetInt(key + "_" + i);
            }
            set
            {
                PlayerPrefs.SetInt(key + "_" + i, value);
            }
        }

        public void Clear()
        {
            for(int i = 0; i < PlayerPrefs.GetInt(key_length); i++)
            {
                PlayerPrefs.DeleteKey(key + "_" + i);
            }
            PlayerPrefs.SetInt(key_length, 0);
        }

        public void NewValue(int length, int value = 0)
        {
            Clear();
            PlayerPrefs.SetInt(key_length, length);
            for(int i = 0; i < length; i++)
            {
                PlayerPrefs.SetInt(key + "_" + i, value);
            }
        }
    }

    public static GameData Instance = new GameData();

    private GameData()
    {

    }

    public int unlockLevel
    {
        get
        {
            return PlayerPrefs.GetInt(PlayerPrefKey.UNLOCK_LEVEL);
        }
        set
        {
            PlayerPrefs.SetInt(PlayerPrefKey.UNLOCK_LEVEL, value);
        }
    }

    public int points
    {
        get
        {
            return PlayerPrefs.GetInt(PlayerPrefKey.POINTS);
        }
        set
        {
            PlayerPrefs.SetInt(PlayerPrefKey.POINTS, value);
        }
    }

    public int coins
    {
        get
        {
            return PlayerPrefs.GetInt(PlayerPrefKey.COINS);
        }
        set
        {
            PlayerPrefs.SetInt(PlayerPrefKey.COINS, value);
        }
    }
    
    public int dayOfDailyChallenge
    {
        get
        {
            return PlayerPrefs.GetInt(PlayerPrefKey.DAY_OF_DAILY_CHALLENGE);
        }
        set
        {
            PlayerPrefs.SetInt(PlayerPrefKey.DAY_OF_DAILY_CHALLENGE, value);
        }
    }

    public bool isAdsOn
    {
        get
        {
            return PlayerPrefs.GetInt(PlayerPrefKey.ADS_ON) == 1;
        }
        set
        {
            PlayerPrefs.SetInt(PlayerPrefKey.ADS_ON, value ? 1 : 0);
        }
    }

    public bool isSoundOn
    {
        get
        {
            return PlayerPrefs.GetInt(PlayerPrefKey.SOUND_ON) == 1;
        }
        set
        {
            PlayerPrefs.SetInt(PlayerPrefKey.SOUND_ON, value ? 1 : 0);
        }
    }

    public int continueDay
    {
        get
        {
            return PlayerPrefs.GetInt(PlayerPrefKey.CONTINUE_DAY);
        }
        set
        {
            PlayerPrefs.SetInt(PlayerPrefKey.CONTINUE_DAY, value);
        }
    }

    public bool dailyRewardStatus
    {
        get
        {
            return PlayerPrefs.GetInt(PlayerPrefKey.DAILY_REWARD_STATUS) == 1;
        }
        set
        {
            PlayerPrefs.SetInt(PlayerPrefKey.DAILY_REWARD_STATUS, value ? 1 : 0);
        }
    }

    public bool challengeRewardStatus
    {
        get
        {
            return PlayerPrefs.GetInt(PlayerPrefKey.CHALLENGE_REWARD_STATUS) == 1;
        }
        set
        {
            PlayerPrefs.SetInt(PlayerPrefKey.CHALLENGE_REWARD_STATUS, value ? 1 : 0);
        }
    }

    public bool isRateOn
    {
        get
        {
            return PlayerPrefs.GetInt(PlayerPrefKey.RATE_ON) == 1;
        }
        set
        {
            PlayerPrefs.SetInt(PlayerPrefKey.RATE_ON, value ? 1 : 0);
        }
    }

    public int achievementProgress
    {
        get
        {
            return PlayerPrefs.GetInt(PlayerPrefKey.ACHIEVEMENT_PROGRESS);
        }
        set
        {
            PlayerPrefs.SetInt(PlayerPrefKey.ACHIEVEMENT_PROGRESS, value);
        }
    }

    public int watchVideoRemain
    {
        get
        {
            return PlayerPrefs.GetInt(PlayerPrefKey.WATCH_VIDEO_REMAIN);
        }
        set
        {
            PlayerPrefs.SetInt(PlayerPrefKey.WATCH_VIDEO_REMAIN, value);
        }
    }

    public ListInt listLevelStars = new ListInt(PlayerPrefKey.LIST_LEVEL_STARS, PlayerPrefKey.LEVEL_STARS_LENGTH);

    public ListInt dailyChallengeProgess = new ListInt(PlayerPrefKey.DAILY_CHALLENGE_PROGESS, PlayerPrefKey.DAILY_CHALLENGE_LENGTH);

    public DateTime lastDayAccess
    {
        get
        {
            long temp = Convert.ToInt64(PlayerPrefs.GetString(PlayerPrefKey.LAST_DAY_ACCESS));
            return DateTime.FromBinary(temp);
        }
        set
        {
            PlayerPrefs.SetString(PlayerPrefKey.LAST_DAY_ACCESS, value.ToBinary().ToString());
        }
    }

    public DateTime lastFbShare
    {
        get
        {
            long temp = Convert.ToInt64(PlayerPrefs.GetString(PlayerPrefKey.LAST_FB_SHARE));
            return DateTime.FromBinary(temp);
        }
        set
        {
            PlayerPrefs.SetString(PlayerPrefKey.LAST_FB_SHARE, value.ToBinary().ToString());
        }
    }

    public DateTime lastWatchVideo
    {
        get
        {
            long temp = Convert.ToInt64(PlayerPrefs.GetString(PlayerPrefKey.LAST_WATCH_VIDEO));
            return DateTime.FromBinary(temp);
        }
        set
        {
            PlayerPrefs.SetString(PlayerPrefKey.LAST_WATCH_VIDEO, value.ToBinary().ToString());
        }
    }

    public LevelState unlockLvState = LevelState.Instance;   

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
            GameServices.Instance.UpdateScore(GameConfig.LEADERBROAD_ID, points, null);
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
                GameServices.Instance.UpdateScore(GameConfig.LEADERBROAD_ID, points, null);
            }
            catch
            {

            }
            return true;
        }
        return false;
    }

    private void updateData()
    {
        int delta = (System.DateTime.Now.Date - lastDayAccess.Date).Days;
        if (delta > 0)
        {
            if (delta == 1 && continueDay < 5) continueDay++;
            else continueDay = 1;
            dailyRewardStatus = false;
            challengeRewardStatus = false;
            dailyChallengeProgess.NewValue(8);
            watchVideoRemain = GameConfig.WATCH_VIDEO_LIMIT;
            lastWatchVideo = System.DateTime.Now.AddDays(-2);
            lastFbShare = System.DateTime.Now.AddDays(-2);
            dayOfDailyChallenge = dayOfDailyChallenge + delta;
            if (dayOfDailyChallenge > 366) dayOfDailyChallenge = dayOfDailyChallenge % 367 + 1;
            lastDayAccess = System.DateTime.Now.Date;
        }
        for (int i = GameConfig.ACHIEVEMENT_CONDITION_POINT.Length - 1; i >= 0; i--)
        {
            if (points >= GameConfig.ACHIEVEMENT_CONDITION_POINT[i])
            {
                GameCache.Instance.unlockAchievementProgress = i + 1;
                break;
            }
        }
        GameCache.Instance.levelSelected = unlockLevel;
    }

    public void LoadData()
    {
        if (PlayerPrefs.HasKey(PlayerPrefKey.GAME))
        {
            updateData();
        }
        else
        {
            PlayerPrefs.SetInt(PlayerPrefKey.GAME, 1);
            newData();
        }
    }

    private void newData()
    {
        listLevelStars.NewValue(1);
        unlockLevel = 1;
        points = 0;
        coins = 200;
        dayOfDailyChallenge = 1;
        continueDay = 1;
        dailyChallengeProgess.NewValue(8);
        isAdsOn = true;
        isRateOn = true;
        isSoundOn = true;
        lastDayAccess = System.DateTime.Now.Date;
        lastWatchVideo = System.DateTime.Now.AddDays(-2);
        lastFbShare = System.DateTime.Now.AddDays(-2);
        dailyRewardStatus = false;
        challengeRewardStatus = false;
        achievementProgress = 0;
        watchVideoRemain = GameConfig.WATCH_VIDEO_LIMIT;
        unlockLvState.NewLevelState();
        GameCache.Instance.firstGameLoad = true;
    }
}


