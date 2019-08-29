using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ImoSysSDK.Network;
using System;
using ImoSysSDK.Others;
using Newtonsoft.Json;

namespace ImoSysSDK.SocialPlatforms {

    public class FetchLeaderboardTask {
        private const string PATH = "/v2/games/leaderboards/{0}/scopes/{1}";
        private const string PATH_FRIENDS = "/v2/games/leaderboards/{0}/scopes/{1}/friends";

        private int leaderboardId;

        public delegate void OnFetchLeaderboardSuccess(LeaderboardResponse response);

        public delegate void OnFetchLeaderboardFailed(string message);

        private event OnFetchLeaderboardFailed onFetchLeaderboardFailed;
        private event OnFetchLeaderboardSuccess onFetchLeaderboardSuccess;

        public FetchLeaderboardTask(int leaderboardId, OnFetchLeaderboardFailed onFetchLeaderboardFailed, OnFetchLeaderboardSuccess onFetchLeaderboardSuccess) {
            this.leaderboardId = leaderboardId;
            this.onFetchLeaderboardFailed = onFetchLeaderboardFailed;
            this.onFetchLeaderboardSuccess = onFetchLeaderboardSuccess;
        }

        public void Fetch(string scope, int? clazz, bool onlyFriends, string countryCode, int limit, int aboveCount) {
            string path = string.Format(onlyFriends ? PATH_FRIENDS : PATH, leaderboardId, scope);
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            if (countryCode != null) {
                queryParams.Add("country", countryCode);
            }
            queryParams.Add("limit", limit.ToString());
            queryParams.Add("aboveCount", aboveCount.ToString());
            queryParams.Add("playerId", GameServices.Instance.PlayerId);
            if (clazz != null) {
                queryParams.Add("class", clazz.ToString());
            }
            RestClient.SendGetRequest(path, queryParams, OnRequestFinished);
        }

        private void OnRequestFinished(long statusCode, string message, string data) {
            if (statusCode == 200) {
                LeaderboardResponse response = JsonConvert.DeserializeObject < LeaderboardResponse > (data);
                OnFetchLeaderboardSuccessCallback(response);
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

        private void OnFetchLeaderboardSuccessCallback(LeaderboardResponse response) {
            if (onFetchLeaderboardSuccess != null) {
                onFetchLeaderboardSuccess(response);
                onFetchLeaderboardSuccess = null;
            }
        }
    }
}
