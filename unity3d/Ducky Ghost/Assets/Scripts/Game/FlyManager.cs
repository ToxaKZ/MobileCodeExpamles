using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class FlyManager : MonoBehaviour
{
    public int Min = 0;
    public int Max = 0;
    public float Delay = 15f;
    public float Duration = 0.5f;
    private static FlyManager helper = null;

    public static FlyManager SharedInstance()
    {
        return helper;
    }

    void Awake()
    {
        helper = this;
    }

    public void Initialize()
    {
        InvokeRepeating("Fly", Delay, Delay);
    }

    void Fly()
    {
        if (UtilitiesHelper.SharedInstance().gameState != GameState.Gaming || GameController.SharedInstance().isProgressing
            || !TouchManager.SharedInstance().TouchEnabled)
        {
            return;
        }

        int count = Random.Range(Min, Max + 1) * 2;

        if (count > MatrixController.SharedInstance().totalItemCount)
        {
            return;
        }

        TouchManager.SharedInstance().TouchEnabled = false;
        List<Transform> buffer = new List<Transform>();

        for (int i = 0; i < count; i++)
        {
            bool isAppend = false;

            for (int j = 0; j < Constants.RANDOM_COUNT; j++)
            {
                if (Append(Random.Range(0, MatrixController.SharedInstance().ColumnCount), Random.Range(0, MatrixController.SharedInstance().RowCount), buffer))
                {
                    isAppend = true;
                    break;
                }
            }

            if (!isAppend)
            {
                int col = 0;

                while (!isAppend && col < MatrixController.SharedInstance().ColumnCount)
                {
                    for (int row = 0; row < MatrixController.SharedInstance().RowCount; row++)
                    {
                        if (Append(col, row, buffer))
                        {
                            isAppend = true;
                            break;
                        }
                    }

                    col++;
                }
            }
        }

        count = buffer.Count;

        if (count > 0 && count % 2 == 0)
        {
            for (int i = 0; i < count; i+= 2)
            {
                ApplyFlyEffect(buffer[i], buffer[i + 1]);
            }
        } else
        {
            TouchManager.SharedInstance().TouchEnabled = true;
        }
    }

    bool Append(int col, int row, List<Transform> buffer)
    {
        GameObject block = MatrixController.SharedInstance().items[col, row];

        if (block != null && block != TouchManager.SharedInstance().selectedBlock)
        {
            Transform tfm = block.transform;

            if (tfm.childCount > 0)
            {
                Transform child = tfm.GetChild(0);

                if (!buffer.Contains(child))
                {
                    buffer.Add(child);
                    return true;
                }
            }
        }

        return false;
    }

    void ApplyFlyEffect(Transform source, Transform dest)
    {
        source.GetComponent<HeroController>().StopAllActions();
        dest.GetComponent<HeroController>().StopAllActions();

        Transform sourceParent = source.parent;
        Transform destParent = dest.parent;

        source.parent = null;
        dest.parent = null;

        source.position = new Vector3(sourceParent.position.x, sourceParent.position.y, source.position.z);
        dest.position = new Vector3(destParent.position.x, destParent.position.y, dest.position.z);

        GameController.SharedInstance().IncreaseProgress();
        source.DOMove(dest.position, Duration).OnComplete(() =>
        {
            OnFlyComplete(source, destParent);
        });
        GameController.SharedInstance().IncreaseProgress();
        dest.DOMove(source.position, Duration).OnComplete(() => 
        {
            OnFlyComplete(dest, sourceParent);
        });
    }

    void OnFlyComplete(Transform child, Transform parent)
    {
        child.parent = parent;
        child.localPosition = new Vector3(0f, 0f, child.localPosition.z);
        MatrixController.SharedInstance().ApplyDefaultEffect(child);
        GameController.SharedInstance().DecreaseProgress();

        if (!GameController.SharedInstance().isProgressing)
        {
            MatrixController.SharedInstance().CheckRefreshMatrix();
        }
    }
}