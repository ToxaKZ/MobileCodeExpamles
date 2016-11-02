using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class UtilitiesHelper
{
    private static UtilitiesHelper helper = null;
    private float _worldScreenHeight;
    private float _worldScreenWidth;

    public GameState gameState { get; set; }

    public float iosVersion
    {
        get
        {
            float osVersion = -1f;
            string versionString = SystemInfo.operatingSystem.Replace("iPhone OS ", "");
            float.TryParse(versionString.Substring(0, 1), out osVersion);
            return osVersion;
        }
    }

    public float worldScreenHeight
    {
        get
        {
            return _worldScreenHeight;
        }
    }

    public float worldScreenWidth
    {
        get
        {
            return _worldScreenWidth;
        }
    }

    public bool internetReachable
    {
        get
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }
    }

    public static UtilitiesHelper SharedInstance()
    {
        if (helper == null)
        {
            helper = new UtilitiesHelper();
        }

        return helper;
    }

    public void ResizeMainCamera(float width, float height, int units)
    {
        float desiredRatio = width / height;
        float currentRatio = (float)Screen.width / (float)Screen.height;
        
        if (currentRatio >= desiredRatio)
        {
            Camera.main.orthographicSize = height / 2f / units;
        } else
        {
            float differenceInSize = desiredRatio / currentRatio;
            Camera.main.orthographicSize = height / 2f / units * differenceInSize;
        }

        _worldScreenHeight = Camera.main.orthographicSize * 2f;
        _worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
    }

    public void ApplySortingLayer(GameObject item, string sortingLayerName)
    {
        item.renderer.sortingLayerName = sortingLayerName;
        Transform transformCached = item.transform;
        int count = transformCached.childCount;
        
        for (int i = 0; i < count; i++)
        {
            ApplySortingLayer(transformCached.GetChild(i).gameObject, sortingLayerName);
        }
    }

    public void ReportSocial()
    {
        if (GCHelper.SharedInstance().isAvailable && UtilitiesHelper.SharedInstance().internetReachable)
        {
            ReportScore();
            ReportAchievement();
            SettingsHelper.SharedInstance().socialReported = true;
            SettingsHelper.SharedInstance().Save();
        }
    }

    void ReportScore()
    {
        int totalScore = SettingsHelper.SharedInstance().GetTotalScore();
            
        if (totalScore > 0)
        {
            GCHelper.SharedInstance().ReportScore(totalScore);
        }
    }
    
    void ReportAchievement()
    {
        Dictionary<string, int> achievements = new Dictionary<string, int>();
        achievements.Add("CgkIkcDf4MweEAIQAg", 1);
        achievements.Add("CgkIkcDf4MweEAIQAw", 5);
        achievements.Add("CgkIkcDf4MweEAIQBA", 10);
        achievements.Add("CgkIkcDf4MweEAIQBQ", 15);
        achievements.Add("CgkIkcDf4MweEAIQBg", 20);
            
        int openedMaxLevelNumber = SettingsHelper.SharedInstance().GetOpenedMaxLevelNumber();
            
        foreach (KeyValuePair<string, int> child in achievements)
        {
            if (openedMaxLevelNumber >= child.Value)
            {
                GCHelper.SharedInstance().ReportAchievement(child.Key, 100f);
            }
        }
    }
}