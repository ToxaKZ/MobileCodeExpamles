using UnityEngine;
using System.Collections;
using System;

public class GCHelper
{
    private bool _isAvailable;
    private const string LEADERBOARD_ID = "CgkIkcDf4MweEAIQAQ";
    private static GCHelper helper;

    public bool isAvailable
    {
        get
        {
            return _isAvailable;
        }
    }

    public static GCHelper SharedInstance()
    {
        if (helper == null)
        {
            helper = new GCHelper();
        }

        return helper;
    }

    private GCHelper()
    {
        #if UNITY_ANDROID
        GooglePlayGames.PlayGamesPlatform.Activate();
        _isAvailable = true;
        #elif UNITY_IPHONE
        _isAvailable = true;
        #else
        _isAvailable = false;
        #endif
    }

    public void Authenticate()
    {
        if (!Social.localUser.authenticated)
        {
            if (UtilitiesHelper.SharedInstance().internetReachable)
            {
                Social.localUser.Authenticate((bool success) => 
                {
                    if (success)
                    {
                        if (!SettingsHelper.SharedInstance().socialReported)
                        {
                            UtilitiesHelper.SharedInstance().ReportSocial();
                        }
                    }
                });
            }
        }
    }

    public void ShowLeaderboard()
    {
        if (!Social.localUser.authenticated)
        {
            Authenticate();
        } else
        {
            Social.ShowLeaderboardUI();
        }
    }

    public void ShowAchievement()
    {
        if (!Social.localUser.authenticated)
        {
            Authenticate();
        } else
        {
            Social.ShowAchievementsUI();
        }
    }

    public void ReportScore(int score)
    {
        if (!Social.localUser.authenticated)
        {
            Authenticate();
        } else
        {
            Social.ReportScore(score, LEADERBOARD_ID, null);
        }
    }

    public void ReportAchievement(string id, double progress)
    {
        if (!Social.localUser.authenticated)
        {
            Authenticate();
        } else
        {
            Social.ReportProgress(id, progress, null);
        }
    }
}