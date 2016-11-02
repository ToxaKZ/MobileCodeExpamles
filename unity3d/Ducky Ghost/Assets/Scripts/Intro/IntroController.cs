using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IntroController : MonoBehaviour
{
    public GameObject Background = null;
    public GameObject ShopButton = null;
    public GameObject AchievementButton = null;
    public GameObject LeaderboardButton = null;
    public GameObject PlayButton = null;
    public GameObject SocialInfo = null;

    public static void Scene()
    {
        Application.LoadLevel(Constants.SCENE_NAME_INTRO);
    }

    void Start()
    {
        InitBackground();
        PrepareButtons();
        UtilitiesHelper.SharedInstance().gameState = GameState.Intro;
    }

    void InitBackground()
    {
        Bounds bounds = Background.renderer.bounds;
        Transform tfm = Background.transform;
        tfm.localScale = new Vector3(UtilitiesHelper.SharedInstance().worldScreenWidth / bounds.size.x,
                                     UtilitiesHelper.SharedInstance().worldScreenHeight / bounds.size.y,
                                     tfm.localScale.z);
    }

    void PrepareButtons()
    {
        AchievementButton.GetComponent<ButtonController>().OnClick += AchievementCommandExecute;
        LeaderboardButton.GetComponent<ButtonController>().OnClick += LeaderboardCommandExecute;
        PlayButton.GetComponent<ButtonController>().OnClick += PlayCommandExecute;
        ShopButton.GetComponent<ButtonController>().OnClick += ShopCommandExecute;
        Transform tfm = ShopButton.transform;
        Vector3 size = ShopButton.renderer.bounds.size; 
        tfm.position = new Vector3(-(UtilitiesHelper.SharedInstance().worldScreenWidth / 2f - size.x / 2f),
                                   UtilitiesHelper.SharedInstance().worldScreenHeight / 2f - size.y / 2f, tfm.position.z);

        if (!StoreController.SharedInstance().isAvailable ||
            (StoreController.SharedInstance().removeAdsPurchased && StoreController.SharedInstance().turnOffTimeLimitPurchased))
        {
            Destroy(ShopButton);
        }

        size = LeaderboardButton.renderer.bounds.size;
        tfm = SocialInfo.transform;
        tfm.position = new Vector3(UtilitiesHelper.SharedInstance().worldScreenWidth / 2f -
            Mathf.Abs(LeaderboardButton.transform.localPosition.x) - size.x / 2f,
                                   UtilitiesHelper.SharedInstance().worldScreenHeight / 2f - size.y / 2f, tfm.position.z);

        if (!GCHelper.SharedInstance().isAvailable)
        {
            Destroy(SocialInfo);
        }
    }

    void ShopCommandExecute(GameObject sender)
    {
        ShopController.Scene();
    }

    void PlayCommandExecute(GameObject sender)
    {
        LevelController.Scene();
    }

    void LeaderboardCommandExecute(GameObject sender)
    {
        if (GCHelper.SharedInstance().isAvailable)
        {
            if (UtilitiesHelper.SharedInstance().internetReachable)
            {
                GCHelper.SharedInstance().ShowLeaderboard();
            } else
            {
                AlertMessage.DisplayAlert(LocalizationHelper.SharedInstance().GetValue(LocalizationHelper.RES_KEY_INTERNET_NO));
            }
        }
    }

    void AchievementCommandExecute(GameObject sender)
    {
        if (GCHelper.SharedInstance().isAvailable)
        {
            if (UtilitiesHelper.SharedInstance().internetReachable)
            {
                GCHelper.SharedInstance().ShowAchievement();
            } else
            {
                AlertMessage.DisplayAlert(LocalizationHelper.SharedInstance().GetValue(LocalizationHelper.RES_KEY_INTERNET_NO));
            }
        }
    }
}