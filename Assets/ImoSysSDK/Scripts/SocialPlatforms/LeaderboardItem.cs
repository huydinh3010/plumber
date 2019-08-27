using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ImoSysSDK.SocialPlatforms {

    [Serializable]
    public class LeaderboardItem {
        public string playerId;
        public string name;
        public string avatarUrl;
        public int score;
        public int rank;
        public string countryCode;
        public JObject metadata;
    }
}