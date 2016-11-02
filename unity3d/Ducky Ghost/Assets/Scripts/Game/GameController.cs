using UnityEngine;
using System.Collections;
using DG.Tweening;

[RequireComponent(typeof(AudioSource))]
public class GameController : MonoBehaviour
{
    public GameObject PausePrefab = null;
    public GameObject Background = null;
    private int proggressCount;
    private static GameController helper = null;
    private static int _levelNumber = 1;
    private float timeScale;

    public bool isProgressing
    {
        get
        {
            return proggressCount != 0;
        }
    }

    public int levelNumber
    {
        get
        {
            return _levelNumber;
        }
    }

    public static GameController SharedInstance()
    {
        return helper;
    }

    public static void Scene(int levelNumber)
    {
        GameController._levelNumber = levelNumber;
        Application.LoadLevel(Constants.SCENE_NAME_GAME);
    }

    public static void Restart()
    {
        GameController.Scene(GameController._levelNumber);
    }

    void Awake()
    {
        timeScale = Time.timeScale;
        UtilitiesHelper.SharedInstance().gameState = GameState.Gaming;
        helper = this;
    }

    void Start()
    {
        proggressCount = 0;
        InitBackground();
        TouchManager.SharedInstance().TouchEnabled = false;
        MatrixController.SharedInstance().CreateMatrix();
        MatrixController.SharedInstance().CreateHeros();
        MoveManager.SharedInstance().Initialize();
        MoveManager.SharedInstance().HighlightMovePath(levelNumber);
        CheckBackgroundMusic();
        AdsShow();
    }

    void InitBackground()
    {
        Bounds bounds = Background.renderer.bounds;
        Transform tfm = Background.transform;
        tfm.localScale = new Vector3(UtilitiesHelper.SharedInstance().worldScreenWidth / bounds.size.x,
                                     UtilitiesHelper.SharedInstance().worldScreenHeight / bounds.size.y,
                                     tfm.localScale.z);
    }

    void CheckBackgroundMusic()
    {
        if (SettingsHelper.SharedInstance().sound)
        {
            if (!audio.isPlaying)
            {
                audio.Play();
            }
        } else
        {
            audio.Stop();
        }
    }

    public void IncreaseProgress()
    {
        proggressCount++;
    }

    public void DecreaseProgress()
    {
        if (proggressCount > 0)
        {
            proggressCount--;
        }
    }

    public void StartLevel()
    {
        TimeController.SharedInstance().StartTime();
        FlyManager.SharedInstance().Initialize();
        TouchManager.SharedInstance().TouchEnabled = true;
    }

    public void GameOver()
    {
        GameOverController.Scene();
    }

    public bool CheckFinish()
    {
        if (MatrixController.SharedInstance().totalItemCount <= 0)
        {
            CongratulationController.Scene(levelNumber, ScoreController.SharedInstance().score);
            return true;
        }
        
        return false;
    }

    public void PauseGame(bool dialog)
    {
        if (UtilitiesHelper.SharedInstance().gameState == GameState.Gaming)
        {
            if (audio.isPlaying)
            {
                audio.Pause();
            }

            AdsHide();

            if (dialog)
            {
                Instantiate(PausePrefab);
            }

            Time.timeScale = 0;
        }
    }

    public void RestoreGame()
    {
        Restore();
        CheckBackgroundMusic();
        AdsShow();
    }

    public void Restore()
    {
        Time.timeScale = timeScale;
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            PauseGame(true);
        } else
        {
            if (UtilitiesHelper.SharedInstance().gameState == GameState.Paused)
            {
                if (audio.isPlaying)
                {
                    audio.Pause();
                }
            }
        }
    }

    void AdsShow()
    {
        if (AdsHelper.SharedInstance().isAvailable)
        {
            AdsHelper.SharedInstance().Show();
        }
    }

    void AdsHide()
    {
        if (AdsHelper.SharedInstance().isAvailable)
        {
            AdsHelper.SharedInstance().Hide();
        }
    }

    void OnDestroy()
    {
        DOTween.KillAll();
        AdsHide();
    }
}