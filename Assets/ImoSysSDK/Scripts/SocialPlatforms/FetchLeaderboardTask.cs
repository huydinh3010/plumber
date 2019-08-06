using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ImoSysSDK.Network;
using System;
using ImoSysSDK.Others;

namespace ImoSysSDK.SocialPlatforms {

    public class FetchLeaderboardTask {
        private const string PATH = "/v1/games/leaderboards/{0}/scopes/{1}";
        private const string PATH_FRIENDS = "/v1/games/leaderboards/{0}/scopes/{1}/friends";

        private int leaderboardId;

        public delegate void OnFetchLeaderboardSuccess(LeaderboardItem[] items);

        public delegate void OnFetchLeaderboardFailed(string message);

        private event OnFetchLeaderboardFailed onFetchLeaderboardFailed;
        private event OnFetchLeaderboardSuccess onFetchLeaderboardSuccess;

        public FetchLeaderboardTask(int leaderboardId, OnFetchLeaderboardFailed onFetchLeaderboardFailed, OnFetchLeaderboardSuccess onFetchLeaderboardSuccess) {
            this.leaderboardId = leaderboardId;
            this.onFetchLeaderboardFailed = onFetchLeaderboardFailed;
            this.onFetchLeaderboardSuccess = onFetchLeaderboardSuccess;
        }

        public void Fetch(string scope, int limit) {
            Fetch(scope, false, null, limit);
        }

        public void Fetch(string scope, int limit, string countryCode) {
            Fetch(scope, false, countryCode, limit);
        }

        public void FetchFriends(string scope, int limit) {
            Fetch(scope, true, null, limit);
        }

        public void FetchFriends(string scope, int limit, string countryCode) {
            Fetch(scope, true, countryCode, limit);
        }

        public void Fetch(string scope, bool onlyFriends, string countryCode, int limit) {
            string path = string.Format(onlyFriends ? PATH_FRIENDS : PATH, leaderboardId, scope);
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            if (countryCode != null) {
                queryParams.Add("country", countryCode);
            }
            queryParams.Add("limit", limit.ToString());
            queryParams.Add("playerId", GameServices.Instance.PlayerId);
            RestClient.SendGetRequest(path, queryParams, OnRequestFinished);
        }

        private void OnRequestFinished(long statusCode, string message, string data) {
            if (statusCode == 200) {
                LeaderboardItem[] items = JsonHelper.FromJson<LeaderboardItem>(data);
                OnFetchLeaderboardSuccessCallback(items);
            } else {
                OnFetchLeaderboardFailedCallback(message);
            }
        }

        private void OnFetchLeaderboardFailedCallback(string message) {
            if (onFetchLeaderboardFailed != null) {
                onFetchLeaderboardFailed(message);
                onFetchLeaderboardFailed = null;
            }
        }

        private void OnFetchLeaderboardSuccessCallback(LeaderboardItem[] items) {
            if (onFetchLeaderboardSuccess != null) {
                onFetchLeaderboardSuccess(items);
                onFetchLeaderboardSuccess = null;
            }
        }
    }
}
