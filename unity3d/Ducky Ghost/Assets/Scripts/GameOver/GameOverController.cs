using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class GameOverController : MonoBehaviour
{
    public GameObject Background = null;
    public GameObject Title = null;
    public GameObject[] Buttons = null;

    public static void Scene()
    {
        Application.LoadLevel(Constants.SCENE_NAME_GAME_OVER);
    }

    void Start()
    {
        InitBackground();
        Title.GetComponent<TitleController>().SetText(LocalizationHelper.SharedInstance().GetValue(LocalizationHelper.RES_KEY_LEVEL_FAILED));
        PrepareButtons();

        if (SettingsHelper.SharedInstance().sound)
        {
            audio.Play();
        }

        ShowAds();
        UtilitiesHelper.SharedInstance().gameState = GameState.GameOver;
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
        foreach (GameObject child in Buttons)
        {
            ButtonController controller = child.GetComponent<ButtonController>();

            if (child.tag == Constants.TAG_NAME_MAIN_MENU)
            {
                controller.OnClick += MainMenuCommandExecute;
            } else if (child.tag == Constants.TAG_NAME_RESTART)
            {
                controller.OnClick += RestartCommandExecute;
            }
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

    void ShowAds()
    {
        if (AdsHelper.SharedInstance().isAvailable)
        {
            AdsHelper.SharedInstance().ShowInterstitial();
        }
    }
}