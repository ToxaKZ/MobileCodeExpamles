using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class CongratulationController : MonoBehaviour
{
    public GameObject Background = null;
    public GameObject Title = null;
    public GameObject ScoreLabel = null;
    public List<GameObject> Buttons = null;
    public float ButtonSpace = 1f;
    public int RateLevelNumber = 5;
    private static int levelNumber = 0;
    private static int score = 0;

    public static void Scene(int levelNumber, int score)
    {
        CongratulationController.levelNumber = levelNumber;
        CongratulationController.score = score;
        Application.LoadLevel(Constants.SCENE_NAME_CONGRATULATION);
    }

    void Start()
    {
        InitBackground();
        Title.GetComponent<TitleController>().SetText(LocalizationHelper.SharedInstance().GetValue(LocalizationHelper.RES_KEY_CONGRATULATIONS));
        ScoreLabel.GetComponent<TitleController>().SetText(string.Format(
            LocalizationHelper.SharedInstance().GetValue(LocalizationHelper.RES_KEY_FORMAT_POINTS),
            CongratulationController.score));
        InitButtons();

        if (SettingsHelper.SharedInstance().sound)
        {
            audio.Play();
        }

        SaveLevelInfo();
        ShowAds();
        UtilitiesHelper.SharedInstance().gameState = GameState.Congratulation;
    }

    void InitBackground()
    {
        Bounds bounds = Background.renderer.bounds;
        Transform tfm = Background.transform;
        tfm.localScale = new Vector3(UtilitiesHelper.SharedInstance().worldScreenWidth / bounds.size.x,
                                     UtilitiesHelper.SharedInstance().worldScreenHeight / bounds.size.y,
                                     tfm.localScale.z);
    }

    void InitButtons()
    {
        GameObject rate = GameObject.FindGameObjectWithTag(Constants.TAG_NAME_RATE);

        if (rate != null)
        {
            rate.GetComponentInChildren<TextMesh>().text = LocalizationHelper.SharedInstance().GetValue(LocalizationHelper.RES_KEY_RATE);
            rate.GetComponent<ButtonController>().OnClick += RateCommandExecute;

            #if !UNITY_IPHONE && !UNITY_ANDROID
            Destroy(rate);
            #endif
        }

        if (CongratulationController.levelNumber >= Constants.LEVEL_COUNT)
        {
            GameObject button = GameObject.FindGameObjectWithTag(Constants.TAG_NAME_NEXT);

            if (button != null)
            {
                Buttons.Remove(button);
                Destroy(button);
            }
        }

        Vector3 size = Buttons[0].renderer.bounds.size;
        float x = -(((size.x * Buttons.Count) + (ButtonSpace * (Buttons.Count - 1))) / 2f);
        float hx = size.x / 2f;

        foreach (GameObject child in Buttons)
        {
            ButtonController controller = child.GetComponent<ButtonController>();
            
            if (child.tag == Constants.TAG_NAME_MAIN_MENU)
            {
                controller.OnClick += MainMenuCommandExecute;
            } else if (child.tag == Constants.TAG_NAME_RESTART)
            {
                controller.OnClick += RestartCommandExecute;
            } else if (child.tag == Constants.TAG_NAME_NEXT)
            {
                controller.OnClick += NextCommandExecute;
            } else if (child.tag == Constants.TAG_NAME_TWEET)
            {
                controller.OnClick += TweetCommandExecute;
            }

            Transform tfm = child.transform;
            tfm.position = new Vector3(x + hx, tfm.position.y, tfm.position.z);
            x += size.x + ButtonSpace;
        }
    }

    void MainMenuCommandExecute(GameObject sender)
    {
        IntroController.Scene();
    }
    
    void RestartCommandExecute(GameObject sender)
    {
        GameController.Restart();
    }

    void NextCommandExecute(GameObject sender)
    {
        GameController.Scene(CongratulationController.levelNumber >= Constants.LEVEL_COUNT ? 1 : CongratulationController.levelNumber + 1);
    }

    void SaveLevelInfo()
    {
        SettingsHelper.SharedInstance().socialReported = false;
        SettingsHelper.SharedInstance().SetLevelScore(CongratulationController.levelNumber, CongratulationController.score);
        SettingsHelper.SharedInstance().Save();
    }

    void RateCommandExecute(GameObject sender)
    {
        string url = null;

        #if UNITY_ANDROID
        url = "market://details?id=com.shopanov.yelnar.duckyghost";
        #elif UNITY_IPHONE
        url = string.Format(UtilitiesHelper.SharedInstance().iosVersion >= 7f ? "itms-apps://itunes.apple.com/app/id{0}" :
                            "itms-apps://ax.itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?type=Purple+Software&id={0}", "931396283");
        #endif

        if (!string.IsNullOrEmpty(url))
        {
            Application.OpenURL(url);
        }
    }

    void TweetCommandExecute(GameObject sender)
    {
        if (UtilitiesHelper.SharedInstance().internetReachable)
        {
            Application.OpenURL(string.Concat("https://twitter.com/intent/tweet?text=",
                                              WWW.EscapeURL(string.Format(LocalizationHelper.SharedInstance().GetValue(
                LocalizationHelper.RES_KEY_FORMAT_TWEET), CongratulationController.score, CongratulationController.levelNumber))));
        } else
        {
            AlertMessage.DisplayAlert(LocalizationHelper.SharedInstance().GetValue(LocalizationHelper.RES_KEY_INTERNET_NO));
        }
    }

    void ShowAds()
    {
        if (AdsHelper.SharedInstance().isAvailable)
        {
            AdsHelper.SharedInstance().ShowInterstitial();
        }
    }
}