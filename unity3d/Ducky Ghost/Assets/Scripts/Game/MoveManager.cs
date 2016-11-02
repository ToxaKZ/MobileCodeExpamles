using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class MoveManager : MonoBehaviour
{
    public GameObject NavigationPrefab = null;
    public GameObject GoMessagePrefab = null;
    public Vector3 GoMessagePosition = Vector3.zero;
    public Color GoMessageColor = Color.red;
    public int GoMessageFontSize = 40;
    public float GoMessageDuration = 0.5f;
    public float GoMessageDelay = 0.5f;
    public float MoveSpeed = 5f;
    public Color[] HighlightColors;
    private static MoveManager helper = null;
    private List<HighlightModel[]> highlightModels;

    public static MoveManager SharedInstance()
    {
        return helper;
    }

    void Awake()
    {
        helper = this;
    }

    public void Initialize()
    {
        int halfColumnCount = MatrixController.SharedInstance().ColumnCount / 2;
        int halfRowCount = MatrixController.SharedInstance().RowCount / 2;

        highlightModels = new List<HighlightModel[]>();
        highlightModels.Add(new HighlightModel[]{});
        highlightModels.Add(new HighlightModel[] {
            new HighlightModel()
            {
                col = 0,
                row = 0,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[0],
                direction = Direction.right
            }
        });
        highlightModels.Add(new HighlightModel[] {
            new HighlightModel()
            {
                col = 0,
                row = 0,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[0],
                direction = Direction.left
            }
        });
        highlightModels.Add(new HighlightModel[] {
            new HighlightModel()
            {
                col = 0,
                row = 0,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[0],
                direction = Direction.up
            }
        });
        highlightModels.Add(new HighlightModel[] {
            new HighlightModel()
            {
                col = 0,
                row = 0,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[0],
                direction = Direction.down
            }
        });
        highlightModels.Add(new HighlightModel[] {
            new HighlightModel()
            {
                col = 0,
                row = 0,
                columnCount = halfColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[0],
                direction = Direction.left
            },
            new HighlightModel()
            {
                col = halfColumnCount,
                row = 0,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[1],
                direction = Direction.right
            }
        });
        highlightModels.Add(new HighlightModel[] {
            new HighlightModel()
            {
                col = 0,
                row = 0,
                columnCount = halfColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[0],
                direction = Direction.right
            },
            new HighlightModel()
            {
                col = halfColumnCount,
                row = 0,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[1],
                direction = Direction.left
            }
        });
        highlightModels.Add(new HighlightModel[] {
            new HighlightModel()
            {
                col = 0,
                row = 0,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = halfRowCount,
                color = HighlightColors[0],
                direction = Direction.up
            },
            new HighlightModel()
            {
                col = 0,
                row = halfRowCount,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[1],
                direction = Direction.down
            }
        });
        highlightModels.Add(new HighlightModel[] {
            new HighlightModel()
            {
                col = 0,
                row = 0,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = halfRowCount,
                color = HighlightColors[0],
                direction = Direction.down
            },
            new HighlightModel()
            {
                col = 0,
                row = halfRowCount,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[1],
                direction = Direction.up
            }
        });
        highlightModels.Add(new HighlightModel[] {
            new HighlightModel()
            {
                col = 0,
                row = 0,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = halfRowCount,
                color = HighlightColors[0],
                direction = Direction.right
            },
            new HighlightModel()
            {
                col = 0,
                row = halfRowCount,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[1],
                direction = Direction.left
            }
        });
        highlightModels.Add(new HighlightModel[] {
            new HighlightModel()
            {
                col = 0,
                row = 0,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = halfRowCount,
                color = HighlightColors[0],
                direction = Direction.left
            },
            new HighlightModel()
            {
                col = 0,
                row = halfRowCount,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[1],
                direction = Direction.right
            }
        });
        highlightModels.Add(new HighlightModel[] {
            new HighlightModel()
            {
                col = 0,
                row = 0,
                columnCount = halfColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[0],
                direction = Direction.down
            },
            new HighlightModel()
            {
                col = halfColumnCount,
                row = 0,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[1],
                direction = Direction.up
            }
        });
        highlightModels.Add(new HighlightModel[] {
            new HighlightModel()
            {
                col = 0,
                row = 0,
                columnCount = halfColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[0],
                direction = Direction.up
            },
            new HighlightModel()
            {
                col = halfColumnCount,
                row = 0,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[1],
                direction = Direction.down
            }
        });
        highlightModels.Add(new HighlightModel[] {
            new HighlightModel()
            {
                col = 0,
                row = 0,
                columnCount = halfColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[0],
                direction = Direction.up
            },
            new HighlightModel()
            {
                col = halfColumnCount,
                row = 0,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[1],
                direction = Direction.right
            }
        });
        highlightModels.Add(new HighlightModel[] {
            new HighlightModel()
            {
                col = 0,
                row = 0,
                columnCount = halfColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[0],
                direction = Direction.right
            },
            new HighlightModel()
            {
                col = halfColumnCount,
                row = 0,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[1],
                direction = Direction.up
            }
        });
        highlightModels.Add(new HighlightModel[] {
            new HighlightModel()
            {
                col = 0,
                row = 0,
                columnCount = halfColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[0],
                direction = Direction.down
            },
            new HighlightModel()
            {
                col = halfColumnCount,
                row = 0,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[1],
                direction = Direction.left
            }
        });
        highlightModels.Add(new HighlightModel[] {
            new HighlightModel()
            {
                col = 0,
                row = 0,
                columnCount = halfColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[0],
                direction = Direction.left
            },
            new HighlightModel()
            {
                col = halfColumnCount,
                row = 0,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[1],
                direction = Direction.down
            }
        });
        highlightModels.Add(new HighlightModel[] {
            new HighlightModel()
            {
                col = 0,
                row = 0,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = halfRowCount,
                color = HighlightColors[0],
                direction = Direction.up
            },
            new HighlightModel()
            {
                col = 0,
                row = halfRowCount,
                columnCount = halfColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[1],
                direction = Direction.left
            },
            new HighlightModel()
            {
                col = halfColumnCount,
                row = halfRowCount,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[2],
                direction = Direction.right
            }
        });
        highlightModels.Add(new HighlightModel[] {
            new HighlightModel()
            {
                col = 0,
                row = 0,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = halfRowCount,
                color = HighlightColors[0],
                direction = Direction.up
            },
            new HighlightModel()
            {
                col = 0,
                row = halfRowCount,
                columnCount = halfColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[1],
                direction = Direction.right
            },
            new HighlightModel()
            {
                col = halfColumnCount,
                row = halfRowCount,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[2],
                direction = Direction.left
            }
        });
        highlightModels.Add(new HighlightModel[] {
            new HighlightModel()
            {
                col = 0,
                row = 0,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = halfRowCount,
                color = HighlightColors[0],
                direction = Direction.down
            },
            new HighlightModel()
            {
                col = 0,
                row = halfRowCount,
                columnCount = halfColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[1],
                direction = Direction.right
            },
            new HighlightModel()
            {
                col = halfColumnCount,
                row = halfRowCount,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[2],
                direction = Direction.left
            }
        });
        highlightModels.Add(new HighlightModel[] {
            new HighlightModel()
            {
                col = 0,
                row = 0,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = halfRowCount,
                color = HighlightColors[0],
                direction = Direction.down
            },
            new HighlightModel()
            {
                col = 0,
                row = halfRowCount,
                columnCount = halfColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[1],
                direction = Direction.left
            },
            new HighlightModel()
            {
                col = halfColumnCount,
                row = halfRowCount,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[2],
                direction = Direction.right
            }
        });
        highlightModels.Add(new HighlightModel[] {
            new HighlightModel()
            {
                col = 0,
                row = 0,
                columnCount = halfColumnCount,
                rowCount = halfRowCount,
                color = HighlightColors[0],
                direction = Direction.up
            },
            new HighlightModel()
            {
                col = halfColumnCount,
                row = 0,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = halfRowCount,
                color = HighlightColors[1],
                direction = Direction.down
            },
            new HighlightModel()
            {
                col = 0,
                row = halfRowCount,
                columnCount = halfColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[2],
                direction = Direction.left
            },
            new HighlightModel()
            {
                col = halfColumnCount,
                row = halfRowCount,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[3],
                direction = Direction.right
            }
        });
        highlightModels.Add(new HighlightModel[] {
            new HighlightModel()
            {
                col = 0,
                row = 0,
                columnCount = halfColumnCount,
                rowCount = halfRowCount,
                color = HighlightColors[0],
                direction = Direction.up
            },
            new HighlightModel()
            {
                col = halfColumnCount,
                row = 0,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = halfRowCount,
                color = HighlightColors[1],
                direction = Direction.down
            },
            new HighlightModel()
            {
                col = 0,
                row = halfRowCount,
                columnCount = halfColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[2],
                direction = Direction.right
            },
            new HighlightModel()
            {
                col = halfColumnCount,
                row = halfRowCount,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[3],
                direction = Direction.left
            }
        });
        highlightModels.Add(new HighlightModel[] {
            new HighlightModel()
            {
                col = 0,
                row = 0,
                columnCount = halfColumnCount,
                rowCount = halfRowCount,
                color = HighlightColors[0],
                direction = Direction.down
            },
            new HighlightModel()
            {
                col = halfColumnCount,
                row = 0,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = halfRowCount,
                color = HighlightColors[1],
                direction = Direction.up
            },
            new HighlightModel()
            {
                col = 0,
                row = halfRowCount,
                columnCount = halfColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[2],
                direction = Direction.left
            },
            new HighlightModel()
            {
                col = halfColumnCount,
                row = halfRowCount,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[3],
                direction = Direction.right
            }
        });
        highlightModels.Add(new HighlightModel[] {
            new HighlightModel()
            {
                col = 0,
                row = 0,
                columnCount = halfColumnCount,
                rowCount = halfRowCount,
                color = HighlightColors[0],
                direction = Direction.down
            },
            new HighlightModel()
            {
                col = halfColumnCount,
                row = 0,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = halfRowCount,
                color = HighlightColors[1],
                direction = Direction.up
            },
            new HighlightModel()
            {
                col = 0,
                row = halfRowCount,
                columnCount = halfColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[2],
                direction = Direction.right
            },
            new HighlightModel()
            {
                col = halfColumnCount,
                row = halfRowCount,
                columnCount = MatrixController.SharedInstance().ColumnCount,
                rowCount = MatrixController.SharedInstance().RowCount,
                color = HighlightColors[3],
                direction = Direction.left
            }
        });
    }

    public void HighlightMovePath(int levelNumber)
    {
        HighlightModel[] collection = highlightModels[levelNumber - 1];

        if (collection.Length > 0)
        {
            foreach (HighlightModel child in collection)
            {
                ColorBlocks(child.col, child.row, child.columnCount, child.rowCount, child.color);
                Bounds bound = MatrixController.SharedInstance().items[child.col, child.row].renderer.bounds;
                float x = bound.min.x + ((bound.size.x * (child.columnCount - child.col)) / 2f);
                float y = bound.max.y - ((bound.size.y * (child.rowCount - child.row)) / 2f);
                GameController.SharedInstance().IncreaseProgress();
                GameObject navigation = Instantiate(NavigationPrefab, new Vector3(x, y, NavigationPrefab.transform.position.z), Quaternion.identity) as GameObject;
                NavigationController controller = navigation.GetComponent<NavigationController>();
                controller.Initialization(child.direction, child.color);
            }
        } else
        {
            ColorBlocks(0, 0, MatrixController.SharedInstance().ColumnCount, MatrixController.SharedInstance().RowCount, HighlightColors[0]);
            ShowGoMessage();
        }
    }

    void ColorBlocks(int col, int row, int columnCount, int rowCount, Color color)
    {
        for (int i = col; i < columnCount; i++)
        {
            for (int j = row; j < rowCount; j++)
            {
                GameObject block = MatrixController.SharedInstance().items[i, j];
                SpriteRenderer sr = block.GetComponent<SpriteRenderer>();
                sr.color = new Color(color.r, color.g, color.b, sr.color.a);
                block.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color;
            }
        }
    }

    public void ShowGoMessage()
    {
        Vector3 scale = GoMessagePrefab.transform.localScale;
        GameObject msg = Instantiate(GoMessagePrefab, GoMessagePosition, Quaternion.identity) as GameObject;
        TitleController controller = msg.GetComponent<TitleController>();
        controller.SetText(LocalizationHelper.SharedInstance().GetValue(LocalizationHelper.RES_KEY_GO));
        controller.SetColor(GoMessageColor);
        controller.SetShadowColor(Color.black, true);
        controller.SetFontSize(GoMessageFontSize);
        Transform tfm = msg.transform;
        tfm.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();
        seq.OnComplete(() =>
        {
            GoComplate(msg);
        });
        seq.Append(tfm.DOScale(scale, GoMessageDuration));
        seq.AppendInterval(GoMessageDelay);
        seq.Play();
    }

    void GoComplate(GameObject item)
    {
        Destroy(item);
        ColorBlocks(0, 0, MatrixController.SharedInstance().ColumnCount, MatrixController.SharedInstance().RowCount, Color.white);
        GameController.SharedInstance().StartLevel();
    }

    public bool MoveBlocks()
    {
        bool result = false;

        if (GameController.SharedInstance().levelNumber > 1)
        {
            HighlightModel[] collection = highlightModels[GameController.SharedInstance().levelNumber - 1];

            if (collection.Length > 0)
            {
                foreach (HighlightModel child in collection)
                {
                    bool moved = false;

                    switch (child.direction)
                    {
                        case Direction.left:
                            moved = MoveToLeft(child);
                            break;
                        case Direction.right:
                            moved = MoveToRight(child);
                            break;
                        case Direction.up:
                            moved = MoveToUp(child);
                            break;
                        case Direction.down:
                            moved = MoveToDown(child);
                            break;
                    }

                    if (moved)
                    {
                        result = true;
                    }
                }
            }
        }

        return result;
    }

    bool MoveToLeft(HighlightModel model)
    {
        bool result = false;

        for (int row = model.row; row < model.rowCount; row++)
        {
            int? position = null;
            int iteration = 0;

            for (int col = model.col; col < model.columnCount; col++)
            {
                GameObject block = MatrixController.SharedInstance().items[col, row];

                if (block == null)
                {
                    iteration++;

                    if (position == null)
                    {
                        position = col;
                    }
                } else if (iteration > 0)
                {
                    ReplaceAndApplyMovingEffect(col, row, position.Value, row);
                    position++;
                    result = true;
                }
            }
        }

        return result;
    }

    bool MoveToRight(HighlightModel model)
    {
        bool result = false;
        
        for (int row = model.row; row < model.rowCount; row++)
        {
            int? position = null;
            int iteration = 0;
            
            for (int col = model.columnCount - 1; col >= model.col; col--)
            {
                GameObject block = MatrixController.SharedInstance().items[col, row];
                
                if (block == null)
                {
                    iteration++;
                    
                    if (position == null)
                    {
                        position = col;
                    }
                } else if (iteration > 0)
                {
                    ReplaceAndApplyMovingEffect(col, row, position.Value, row);
                    position--;
                    result = true;
                }
            }
        }
        
        return result;
    }

    bool MoveToUp(HighlightModel model)
    {
        bool result = false;
        
        for (int col = model.col; col < model.columnCount; col++)
        {
            int? position = null;
            int iteration = 0;
            
            for (int row = model.row; row < model.rowCount; row++)
            {
                GameObject block = MatrixController.SharedInstance().items[col, row];
                
                if (block == null)
                {
                    iteration++;
                    
                    if (position == null)
                    {
                        position = row;
                    }
                } else if (iteration > 0)
                {
                    ReplaceAndApplyMovingEffect(col, row, col, position.Value);
                    position++;
                    result = true;
                }
            }
        }
        
        return result;
    }

    bool MoveToDown(HighlightModel model)
    {
        bool result = false;
        
        for (int col = model.col; col < model.columnCount; col++)
        {
            int? position = null;
            int iteration = 0;
            
            for (int row = model.rowCount - 1; row >= model.row; row--)
            {
                GameObject block = MatrixController.SharedInstance().items[col, row];
                
                if (block == null)
                {
                    iteration++;
                    
                    if (position == null)
                    {
                        position = row;
                    }
                } else if (iteration > 0)
                {
                    ReplaceAndApplyMovingEffect(col, row, col, position.Value);
                    position--;
                    result = true;
                }
            }
        }
        
        return result;
    }

    void ReplaceAndApplyMovingEffect(int sourceCol, int sourceRow, int destCol, int destRow)
    {
        GameObject source = MatrixController.SharedInstance().items[sourceCol, sourceRow];
        MatrixController.SharedInstance().items[sourceCol, sourceRow] = MatrixController.SharedInstance().items[destCol, destRow];
        MatrixController.SharedInstance().items[destCol, destRow] = source;
        GameController.SharedInstance().IncreaseProgress();
        Transform tfm = source.transform;
        Vector3 pos = MatrixController.SharedInstance().itemPositions[destCol, destRow];
        tfm.DOMove(pos, Vector3.Distance(tfm.position, pos) / MoveSpeed).OnComplete(OnMoveComplate);
    }

    void OnMoveComplate()
    {
        GameController.SharedInstance().DecreaseProgress();

        if (!GameController.SharedInstance().isProgressing)
        {
            MatrixController.SharedInstance().CheckRefreshMatrix();
        }
    }

    private class HighlightModel
    {
        public int col { get; set; }

        public int row { get; set; }

        public int columnCount { get; set; }

        public int rowCount { get; set; }

        public Color color { get; set; }

        public Direction direction { get; set; }
    }
}