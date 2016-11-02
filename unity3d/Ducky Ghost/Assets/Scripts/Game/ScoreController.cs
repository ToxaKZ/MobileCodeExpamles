using UnityEngine;
using System.Collections;

public class ScoreController : MonoBehaviour
{
    public int BonusScoreSeconds = 30;
    public int BonusScore = 50;
    public GameObject BonusScoreLabel = null;
    private TextMesh bonusScoreTextMesh;
    private int _score;
    private Transform transformCached;
    private TextMesh textMeshCached;
    private Renderer rendererCached;
    private static ScoreController helper = null;
    private int _bonusValue;

    public int bonusValue
    {
        get
        {
            return _bonusValue;
        }
        set
        {
            _bonusValue = value;
            DisplayBonusValue();
        }
    }

    public int score
    {
        get
        {
            return _score;
        }
    }

    public static ScoreController SharedInstance()
    {
        return helper;
    }

    void Awake()
    {
        _bonusValue = 0;
        _score = 0;
        bonusScoreTextMesh = BonusScoreLabel.GetComponent<TextMesh>();
        helper = this;
    }

    void Start()
    {
        transformCached = transform;
        rendererCached = renderer;
        textMeshCached = GetComponent<TextMesh>();
        InitBonusLabel();
        DisplayScore();
    }
    
    void Update()
    {
    }

    void InitBonusLabel()
    {
        Vector3 size = BonusScoreLabel.renderer.bounds.size;
        Transform tfm = BonusScoreLabel.transform;
        tfm.position = new Vector3(0, UtilitiesHelper.SharedInstance().worldScreenHeight / 2f - size.y / 2f, tfm.position.z);
    }

    void DisplayBonusValue()
    {
        bonusScoreTextMesh.text = string.Format("+{0}", bonusValue);
    }

    void DisplayScore()
    {
        textMeshCached.text = score.ToString();
        transformCached.position = new Vector3(UtilitiesHelper.SharedInstance().worldScreenWidth / 2f - rendererCached.bounds.size.x / 2f,
                                               UtilitiesHelper.SharedInstance().worldScreenHeight / 2f - rendererCached.bounds.size.y / 2f, transformCached.position.z);
    }

    public void IncreaseAndDisplayScore()
    {
        _score += bonusValue;
        DisplayScore();
    }
}