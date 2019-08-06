using UnityEngine;
using System.Collections;
using System;

namespace ImoSysSDK.SocialPlatforms {

    public class GameServices {

        private static readonly object instanceLock = new object();

        private static GameServices instance;

        private string playerId;

        public static class LeaderboardTypes {
            public const string LifeTime = "lifetime";
            public const string Weekly = "weekly";
            public const string Monthly = "monthly";
        }

        public static GameServices Instance {
            get {
                if (instance == null) {
                    lock (instanceLock) {
                        if (instance == null) {
                            instance = new GameServices();
                        }
                    }
                }
                return instance;
            }
        }

        public GameServices() {
        }

        public string PlayerId {
            get {
                if (playerId == null) {
                    playerId = FacebookHelper.Instance.IsLoggedIn ? "fb:" + FacebookHelper.Instance.FacebookUserId : Core.ImoSysSDK.Instance.DeviceId;
                }
                return playerId;
            }
        }

        public void UpdateScore(int leaderboardId, int score, Action<bool> callback) {
            LeaderboardUpdateScoreTask updateScore = new LeaderboardUpdateScoreTask(leaderboardId, (message) => {
                if (callback != null) {
                    callback(false);
                }
            },
            () => {
                if (callback != null) {
                    callback(true);
                }
            });
            updateScore.AddScore(score);
        }

        public void FetchLeaderboard(int leaderboardId, string scope, int limit, Action<bool, LeaderboardItem[]> callback) {
            FetchLeaderboard(leaderboardId, scope, false, null, limit, callback);
        }

        public void FetchLeaderboard(int leaderboardId, string scope, int limit, string countryCode, Action<bool, LeaderboardItem[]> callback) {
            FetchLeaderboard(leaderboardId, scope, false, countryCode, limit, callback);
        }

        public void FetchFriendLeaderboard(int leaderboardId, string scope, int limit, Action<bool, LeaderboardItem[]> callback) {
            FetchLeaderboard(leaderboardId, scope, true, null, limit, callback);
        }

        public void FetchFriendLeaderboard(int leaderboardId, string scope, int limit, string countryCode, Action<bool, LeaderboardItem[]> callback) {
            FetchLeaderboard(leaderboardId, scope, true, countryCode, limit, callback);
        }

        private void FetchLeaderboard(int leaderboardId, string scope, bool onlyFriends, string countryCode, int limit, Action<bool, LeaderboardItem[]> callback) {
            FetchLeaderboardTask fetchLeaderboardTask = new FetchLeaderboardTask(leaderboardId, (message) => {
                callback(false, null);
            },
            (items) => {
                callback(true, items);
            });
            fetchLeaderboardTask.Fetch(scope, onlyFriends, countryCode, limit);
        }
    }
}
