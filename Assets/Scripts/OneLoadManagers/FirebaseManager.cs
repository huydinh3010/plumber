using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;

public class FirebaseManager : MonoBehaviour
{

    public static FirebaseManager Instance;

    public FirebaseApp app;
    private bool ReadyToUse;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Initialize()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                //   app = Firebase.FirebaseApp.DefaultInstance;
                ReadyToUse = true;
                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                ReadyToUse = false;
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    public void LogEventLevelStart(int level, string type, int day)
    {
        if (ReadyToUse)
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent("level_start",
                new Firebase.Analytics.Parameter[] {
                    new Firebase.Analytics.Parameter("level", level),
                    new Firebase.Analytics.Parameter("type", type),
                    new Firebase.Analytics.Parameter("day", day)
                });
        }
    }

    public void LogEventLevelEnd(int level, string type, int day, float duration_secs, int turn_count, int remove_pipe_count, int construct_pipe_count)
    {
        if (ReadyToUse)
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent("level_end",
                new Firebase.Analytics.Parameter[] {
                    new Firebase.Analytics.Parameter("level", level),
                    new Firebase.Analytics.Parameter("type", type),
                    new Firebase.Analytics.Parameter("day", day),
                    new Firebase.Analytics.Parameter("duration_secs", duration_secs),
                    new Firebase.Analytics.Parameter("turn_count", turn_count),
                    new Firebase.Analytics.Parameter("remove_pipe_count", remove_pipe_count),
                    new Firebase.Analytics.Parameter("construct_pipe_count", construct_pipe_count)
                });
        }
    }

    public void LogEventRequestRewardedVideo(string purpose, bool has_video, int level)
    {
        if (ReadyToUse)
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent("start_rewarded_ads",
                new Firebase.Analytics.Parameter[] {
                    new Firebase.Analytics.Parameter("purpose", purpose),
                    new Firebase.Analytics.Parameter("has_video", has_video.ToString()),
                    new Firebase.Analytics.Parameter("level", level)
                });
        }
    }

    public void LogEventShareFacebook()
    {
        if (ReadyToUse)
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent("share_facebook");
        }
    }

    public void SetUserProperties(int level)
    {
        Firebase.Analytics.FirebaseAnalytics.SetUserProperty("level", level.ToString());
    }
}
