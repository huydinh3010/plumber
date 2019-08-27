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
            public const string Periodically = "periodically";
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
            internal set {
                playerId = value;
            }
        }

        public void UpdateScore(int leaderboardId, int score, Action<bool> callback) {
            UpdateScore(leaderboardId, score, null, null, callback);
        }

        public void UpdateScore(int leaderboardId, int score, string jsonMetadata, Action<bool> callback) {
            UpdateScore(leaderboardId, score, null, jsonMetadata, callback);
        }

        public void UpdateScore(int leaderboardId, int score, int? clazz, string jsonMetadata, Action<bool> callback) {
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
            updateScore.UpdateScore(score, clazz, jsonMetadata);
        }

        public void AddScore(int leaderboardId, int score, Action<bool> callback) {
            AddScore(leaderboardId, score, null, null, callback);
        }

        public void AddScore(int leaderboardId, int score, string jsonMetadata, Action<bool> callback) {
            AddScore(leaderboardId, score, null, jsonMetadata, callback);
        }

        public void AddScore(int leaderboardId, int score, int? clazz, string jsonMetadata, Action<bool> callback) {
            LeaderboardAddScoreTask task = new LeaderboardAddScoreTask(leaderboardId, (message) => {
                callback(false);
            },
            () => {
                callback(true);
            });
            task.AddScore(clazz, score, jsonMetadata);
        }

        public void FetchLeaderboard(int leaderboardId, string scope, int limit, int aboveCount, Action<bool, LeaderboardResponse> callback) {
            FetchLeaderboard(leaderboardId, scope, null, false, null, limit, aboveCount, callback);
        }

        public void FetchLeaderboard(int leaderboardId, string scope, int limit, int aboveCount, string countryCode, Action<bool, LeaderboardResponse> callback) {
            FetchLeaderboard(leaderboardId, scope, null, false, countryCode, limit, aboveCount, callback);
        }

        public void FetchFriendLeaderboard(int leaderboardId, string scope, int limit, int aboveCount, Action<bool, LeaderboardResponse> callback) {
            FetchLeaderboard(leaderboardId, scope, null, true, null, limit, aboveCount, callback);
        }

        public void FetchFriendLeaderboard(int leaderboardId, string scope, int limit, int aboveCount, string countryCode, Action<bool, LeaderboardResponse> callback) {
            FetchLeaderboard(leaderboardId, scope, null, true, countryCode, limit, aboveCount, callback);
        }

        public void FetchLeaderboard(int leaderboardId, string scope, int clazz, int limit, int aboveCount, Action<bool, LeaderboardResponse> callback) {
            FetchLeaderboard(leaderboardId, scope, clazz, false, null, limit, aboveCount, callback);
        }

        public void FetchLeaderboard(int leaderboardId, string scope, int clazz, int limit, int aboveCount, string countryCode, Action<bool, LeaderboardResponse> callback) {
            FetchLeaderboard(leaderboardId, scope, clazz, false, countryCode, limit, aboveCount, callback);
        }

        public void RenamePlayer(string newName, Action<bool> callback) {
            RenamePlayerTask task = new RenamePlayerTask((message) => {
                callback(false);
            },
            () => {
                callback(true);
            });
            task.Rename(newName);
        }

        public void GetPeriodicallyLeaderboadInfo(int leaderboardId, Action<bool, PeriodicallyLeaderboardInfo> callback) {
            LeaderboardFetchPeriodicallyInfoTask task = new LeaderboardFetchPeriodicallyInfoTask(leaderboardId, (message) => {
                callback(false, null);
            }, (leaderboardInfo) => {
                callback(true, leaderboardInfo);
            });
            task.FetchInfo();
        }

        private void FetchLeaderboard(int leaderboardId, string scope, int? clazz, bool onlyFriends, string countryCode, int limit, int aboveCount, Action<bool, LeaderboardResponse> callback) {
            FetchLeaderboardTask fetchLeaderboardTask = new FetchLeaderboardTask(leaderboardId, (message) => {
                callback(false, null);
            },
            (items) => {
                callback(true, items);
            });
            fetchLeaderboardTask.Fetch(scope, clazz, onlyFriends, countryCode, limit, aboveCount);
        }

    }
}
