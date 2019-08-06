using UnityEngine;
using System.Collections;
using ImoSysSDK.Network;
using System;

namespace ImoSysSDK.SocialPlatforms {

    public class LeaderboardUpdateScoreTask {

        private const string PATH = "/v1/games/leaderboards/{0}/update";

        private int leaderboardId;

        public delegate void OnUpdateScoreSuccess();

        public delegate void OnUpdateScoreFailed(string message);

        private event OnUpdateScoreFailed onUpdateScoreFailed;
        private event OnUpdateScoreSuccess onUpdateScoreSuccess;

        public LeaderboardUpdateScoreTask(int leaderboardId, OnUpdateScoreFailed onUpdateScoreFailed, OnUpdateScoreSuccess onUpdateScoreSuccess) {
            this.leaderboardId = leaderboardId;
            this.onUpdateScoreFailed = onUpdateScoreFailed;
            this.onUpdateScoreSuccess = onUpdateScoreSuccess;
        }

        public void AddScore(int score) {
            UpdateScoreBody body = new UpdateScoreBody(GameServices.Instance.PlayerId, score);
            RestClient.SendPostRequest(string.Format(PATH, this.leaderboardId), JsonUtility.ToJson(body), OnRequestFinished);
        }

        private void OnRequestFinished(long statusCode, string message, string data) {
            if (statusCode == 200) {
                OnUpdateScoreSuccessCallback();
            } else {
                OnUpdateScoreFailedCallback(message);
            }
        }

        private void OnUpdateScoreFailedCallback(string message) {
            if (onUpdateScoreFailed != null) {
                onUpdateScoreFailed(message);
                onUpdateScoreFailed = null;
            }
        }

        private void OnUpdateScoreSuccessCallback() {
            if (onUpdateScoreSuccess != null) {
                onUpdateScoreSuccess();
                onUpdateScoreSuccess = null;
            }
        }

        private class UpdateScoreBody {
            public string playerId;
            public int score;

            public UpdateScoreBody(string playerId, int score) {
                this.playerId = playerId;
                this.score = score;
            }
        }
    }
}