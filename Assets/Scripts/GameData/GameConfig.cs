using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig
{
    public static readonly int[] ACHIEVEMENT_CONDITION_POINT = { 50, 100, 500, 1000, 2000, 5000, 10000, 20000, 50000, 100000 };
    public static readonly int[] ACHIEVEMENT_COIN_REWARD = { 10, 20, 50, 100, 200, 500, 1000, 2000, 5000, 10000 };
    public static readonly int[] PASS_LEVEL_COIN_REWARD = { 1, 2, 3 };
    public static readonly int[] PASS_LEVEL_POINT_REWARD = { 10, 20, 30 };
    public static readonly int[] DAILY_REWARD_COIN = { 10, 25, 50, 75, 100 };
    public static readonly float[] SHOP_PRICE = {4.99f, 0.99f, 1.99f, 3.99f, 4.99f, 7.99f, 9.99f, 14.99f};
    public static readonly int[] SHOP_COIN = { 0, 100, 200, 500, 1000, 2000, 5000, 10000 };
    public const int DAILY_CHALLENGE_COIN_REWARD = 100;
    public const int NUMBER_OF_SIMPLE_LEVEL = 560;
    public const int REWARDED_VIDEO_COIN = 10;
    public const int REMOVE_PIPE_COST = 50;
    public const int CONSTRUCT_PIPE_COST = 25;
}
