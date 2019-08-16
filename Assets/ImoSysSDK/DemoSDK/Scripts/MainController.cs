using ImoSysSDK.SocialPlatforms;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateLeaderboardClick() {
        GameServices.Instance.UpdateScore(2342368, 1000, (success) => {
            Debug.Log("IMO update leaderboard: " + success);
        });
    }

    public void GetLeaderboardClick() {
        GameServices.Instance.FetchLeaderboard(2342368, GameServices.LeaderboardTypes.LifeTime, 10, 1, (success, items) => {
            Debug.Log("IMO get leaderboard: " + success);
            if(items  != null) {
                for (int i = 0; i < items.Length; i++) {
                    Debug.Log("leaderboard i(" + i + "): " + items[i].name + " - " + items[i].score);
                }
            }
        });
    }


    public void LoginFacebookClick() {
        FacebookHelper.Instance.Login(onLoginFBFailed, onLoginFBSuccess);
    }

    private void onLoginFBFailed(string message) {
        Debug.Log("login failed: " + message);
    }

    private void onLoginFBSuccess() {
        Debug.Log("Login success");
    }
}
