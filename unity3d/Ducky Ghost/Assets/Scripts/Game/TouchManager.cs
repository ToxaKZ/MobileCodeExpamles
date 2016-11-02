using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

[RequireComponent(typeof(AudioSource))]
public class TouchManager : MonoBehaviour
{
    public AudioClip AudioTrue = null;
    public AudioClip AudioFalse = null;
    public GameObject PoohPrefab = null;
    public Color SelectedColor = Color.red;
    public float PoohScaleDuration = 0.4f;
    public float PoohFadeOutDuration = 0.2f;
    private GameObject _selectedBlock;
    private int selectedCol;
    private int selectedRow;

    public bool TouchEnabled { get; set; }

    public GameObject selectedBlock
    {
        get
        {
            return _selectedBlock;
        }
    }

    private static TouchManager helper = null;

    public static TouchManager SharedInstance()
    {
        return helper;
    }

    void Awake()
    {
        helper = this;
    }

    void Start()
    {
        _selectedBlock = null;
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TouchBegan();
        }
    }

    void TouchBegan()
    {
        if (TouchEnabled && UtilitiesHelper.SharedInstance().gameState == GameState.Gaming && !GameController.SharedInstance().isProgressing)
        {
            List<LineVector> vectors = new List<LineVector>();

            for (int col = 0; col < MatrixController.SharedInstance().ColumnCount; col++)
            {
                for (int row = 0; row < MatrixController.SharedInstance().RowCount; row++)
                {
                    GameObject block = MatrixController.SharedInstance().items[col, row];

                    if (block == null || block.transform.childCount == 0)
                    {
                        continue;
                    }

                    Bounds bounds = block.renderer.bounds;
                    Rect boundingBox = new Rect(bounds.min.x, bounds.min.y, bounds.size.x, bounds.size.y);

                    if (!boundingBox.Contains(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
                    {
                        continue;
                    }

                    SetColor(block, SelectedColor);

                    if (selectedBlock == null)
                    {
                        _selectedBlock = block;
                        selectedCol = col;
                        selectedRow = row;
                    } else if (selectedBlock != block)
                    {
                        if (DetectorManager.SharedInstance().IsValideBlocks(selectedCol, selectedRow, selectedBlock, block, 1, vectors))
                        {
                            PlaySound(true);
                            TouchEnabled = false;
                            ScoreController.SharedInstance().IncreaseAndDisplayScore();
                            Pooh(selectedBlock.transform.position);
                            Pooh(block.transform.position);
                            MatrixController.SharedInstance().RemoveItem(selectedCol, selectedRow);
                            MatrixController.SharedInstance().RemoveItem(col, row);
                            LineManager.SharedInstance().DrawLine(selectedBlock.transform.position, vectors);
                        } else
                        {
                            PlaySound(false);
                            SetColor(block, Color.white);
                            SetColor(selectedBlock, Color.white);
                        }
                        
                        _selectedBlock = null;
                    }

                    return;
                }
            }
        }
    }

    void SetColor(GameObject item, Color color)
    {
        SpriteRenderer sr = item.GetComponent<SpriteRenderer>();
        sr.color = new Color(color.r, color.g, color.b, sr.color.a);        
    }

    void Pooh(Vector3 position)
    {
        GameObject pooh = Instantiate(PoohPrefab, new Vector3(position.x, position.y, PoohPrefab.transform.position.z), Quaternion.identity) as GameObject;
        Transform tfm = pooh.transform;
        tfm.localScale = Vector3.zero;
        SpriteRenderer sr = pooh.GetComponent<SpriteRenderer>();
        tfm.DOScale(new Vector3(1f, 1f, 1f), PoohScaleDuration);
        sr.DOFade(0f, PoohScaleDuration + 0.1f).OnComplete(() => 
        {
            Destroy(pooh);
        });
    }

    void PlaySound(bool isTrue)
    {
        if (SettingsHelper.SharedInstance().fx)
        {
            if (audio.isPlaying)
            {
                audio.Stop();
            }

            if (isTrue)
            {
                audio.PlayOneShot(AudioTrue);
            } else
            {
                audio.PlayOneShot(AudioFalse);
            }
        }
    }
}