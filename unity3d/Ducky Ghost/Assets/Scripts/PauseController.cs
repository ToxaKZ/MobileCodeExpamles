using UnityEngine;
using System.Collections;

public class PauseController : MonoBehaviour
{
    public GameObject TitlePrefab = null;
    public GameObject Background = null;
    public GameObject[] Buttons = null;
    public GameObject SoundButton = null;
    public GameObject SoundOffButton = null;
    public GameObject FXButton = null;
    public GameObject FXOffButton = null;
    private GameState gameState;

    void Awake()
    {
        gameState = UtilitiesHelper.SharedInstance().gameState;
        UtilitiesHelper.SharedInstance().gameState = GameState.Paused;
    }

    void Start()
    {
        InitBackground();
        PrepareTitle();
        PrepareButtons();
        RefreshAudioButton();
    }
    
    void Update()
    {
    }

    void InitBackground()
    {
        Bounds bounds = Background.renderer.bounds;
        Transform tfm = Background.transform;
        tfm.localScale = new Vector3(UtilitiesHelper.SharedInstance().worldScreenWidth / bounds.size.x,
                                     UtilitiesHelper.SharedInstance().worldScreenHeight / bounds.size.y,
                                     tfm.localScale.z);
    }

    void PrepareTitle()
    {
        GameObject title = Instantiate(TitlePrefab) as GameObject;
        Transform tfm = title.transform;
        tfm.parent = transform;
        title.GetComponent<TitleController>().SetText(LocalizationHelper.SharedInstance().GetValue(LocalizationHelper.RES_KEY_PAUSE));
        UtilitiesHelper.SharedInstance().ApplySortingLayer(title, Constants.SORTING_LAYER_NAME_PAUSE);
    }

    void PrepareButtons()
    {
        foreach (GameObject child in Buttons)
        {
            ButtonController controller = child.GetComponent<ButtonController>();

            if (child.tag == Constants.TAG_NAME_PLAY)
            {
                controller.OnClick += PlayCommandExecute;
            } else if (child.tag == Constants.TAG_NAME_MAIN_MENU)
            {
                controller.OnClick += MainMenuCommandExecute;
            } else if (child.tag == Constants.TAG_NAME_RESTART)
            {
                controller.OnClick += RestartCommandExecute;
            }
        }

        SoundButton.GetComponent<ButtonController>().OnClick += SoundOffCommandExecute;
        SoundOffButton.GetComponent<ButtonController>().OnClick += SoundOnCommandExecute;
        FXButton.GetComponent<ButtonController>().OnClick += FXOffCommandExecute;
        FXOffButton.GetComponent<ButtonController>().OnClick += FXOnCommandExecute;
    }

    void PlayCommandExecute(GameObject sender)
    {
        Restore(true);
        Destroy(gameObject);
    }

    void MainMenuCommandExecute(GameObject sender)
    {
        Restore(false);
        IntroController.Scene();   
    }

    void RestartCommandExecute(GameObject sender)
    {
        Restore(false);
        GameController.Restart();
    }

    void Restore(bool isGame)
    {
        if (isGame)
        {
            GameController.SharedInstance().RestoreGame();
        } else
        {
            GameController.SharedInstance().Restore();
        }

        UtilitiesHelper.SharedInstance().gameState = gameState;
    }

    void RefreshAudioButton()
    {
        if (SettingsHelper.SharedInstance().sound)
        {
            SoundButton.SetActive(true);
            SoundOffButton.SetActive(false);
        } else
        {
            SoundButton.SetActive(false);
            SoundOffButton.SetActive(true);
        }

        if (SettingsHelper.SharedInstance().fx)
        {
            FXButton.SetActive(true);
            FXOffButton.SetActive(false);
        } else
        {
            FXButton.SetActive(false);
            FXOffButton.SetActive(true);
        }
    }

    void SoundOnCommandExecute(GameObject sender)
    {
        SoundExecute(true);
    }

    void SoundOffCommandExecute(GameObject sender)
    {
        SoundExecute(false);
    }

    void SoundExecute(bool value)
    {
        SettingsHelper.SharedInstance().sound = value;
        SettingsHelper.SharedInstance().Save();
        RefreshAudioButton();
    }

    void FXOnCommandExecute(GameObject sender)
    {
        FXExecute(true);
    }
    
    void FXOffCommandExecute(GameObject sender)
    {
        FXExecute(false);
    }
    
    void FXExecute(bool value)
    {
        SettingsHelper.SharedInstance().fx = value;
        SettingsHelper.SharedInstance().Save();
        RefreshAudioButton();
    }
}