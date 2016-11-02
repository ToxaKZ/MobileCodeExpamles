using UnityEngine;
using System.Collections;

public class TimeController : MonoBehaviour
{
    public GameObject Pause = null;
    public GameObject Label = null;
    public int TotalGameMinutes = 3;
    private TextMesh textMesh;
    private Transform labelTransformCached;
    private Renderer labelRendererCached;
    private static TimeController helper = null;
    private int totalGameSeconds;

    public static TimeController SharedInstance()
    {
        return helper;
    }

    void Awake()
    {
        textMesh = Label.GetComponent<TextMesh>();
        labelTransformCached = Label.transform;
        labelRendererCached = Label.renderer;
        helper = this;
    }

    void Start()
    {
        totalGameSeconds = 60 * TotalGameMinutes;
        ScoreController.SharedInstance().bonusValue = (totalGameSeconds / ScoreController.SharedInstance().BonusScoreSeconds)
            * ScoreController.SharedInstance().BonusScore;
        Pause.GetComponent<ButtonController>().OnClick += PauseCommandExecute;

        if (StoreController.SharedInstance().turnOffTimeLimitPurchased)
        {
            Color color = textMesh.color;
            color.a = 0;
            textMesh.color = color;
        }

        DisplayTime();
        PreparePositions();
    }

    void Update()
    {
        if (UtilitiesHelper.SharedInstance().gameState == GameState.Gaming)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (OnLabel())
                {
                    PauseCommandExecute(Label);
                }
            }
        }    
    }

    void PreparePositions()
    {
        Bounds buttonBound = Pause.renderer.bounds;
        Transform tfm = transform;
        tfm.position = new Vector3(-(UtilitiesHelper.SharedInstance().worldScreenWidth / 2f -
            Mathf.Abs(Pause.transform.localPosition.x) - Pause.renderer.bounds.size.x / 2f),
                                   UtilitiesHelper.SharedInstance().worldScreenHeight / 2f - buttonBound.size.y / 2f, tfm.position.z);
    }

    void DecreaseTime()
    {
        totalGameSeconds--;
        DisplayTime();

        if (totalGameSeconds % ScoreController.SharedInstance().BonusScoreSeconds == 0
            && ScoreController.SharedInstance().bonusValue > ScoreController.SharedInstance().BonusScore)
        {
            ScoreController.SharedInstance().bonusValue -= ScoreController.SharedInstance().BonusScore;
        }

        if (totalGameSeconds <= 0)
        {
            if (!StoreController.SharedInstance().turnOffTimeLimitPurchased)
            {
                GameController.SharedInstance().GameOver();
            } else
            {
                CancelInvoke();
            }
        }
    }

    void DisplayTime()
    {
        long minutes = totalGameSeconds / 60;
        float seconds_divisor = totalGameSeconds % 60;
        long seconds = Mathf.CeilToInt(seconds_divisor);
        textMesh.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);
    }

    public void StartTime()
    {
        InvokeRepeating("DecreaseTime", 1.0f, 1.0f);
    }

    void PauseCommandExecute(GameObject sender)
    {
        GameController.SharedInstance().PauseGame(true);
    }

    bool OnLabel()
    {
        Vector3 pos = labelTransformCached.position;
        Vector3 size = labelRendererCached.bounds.size;
        Rect boundingBox = new Rect(pos.x - size.x / 2f, pos.y - size.y / 2f, size.x, size.y);
        return boundingBox.Contains(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
}