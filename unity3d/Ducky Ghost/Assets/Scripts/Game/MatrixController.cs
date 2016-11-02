using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class MatrixController : MonoBehaviour
{
    public int ColumnCount = 0;
    public int RowCount = 0;
    public GameObject EmptyPrefab = null;
    public GameObject[] ItemPrefabs = null;
    public float DefaultEffectDuration = 0.3f;
    public float DurationChildExchange = 1.2f;
    private GameObject[,] _items;
    private Vector3[,] _itemPositions;
    private int _totalItemCount;
    private static MatrixController helper = null;
    private float matrixSpace;

    public GameObject[,] items
    {
        get
        {
            return _items;
        }
    }
    
    public Vector3[,] itemPositions
    {
        get
        {
            return _itemPositions;
        }
    }

    public int totalItemCount
    {
        get
        {
            return _totalItemCount;
        }
    }

    public static MatrixController SharedInstance()
    {
        return helper;
    }

    void Awake()
    {
        if (StoreController.SharedInstance().removeAdsPurchased)
        {
            matrixSpace = 0f;
        } else
        {
            RowCount = RowCount - 1;
            matrixSpace = 0.3f;
        }

        _totalItemCount = 0;
        _items = new GameObject[ColumnCount, RowCount];
        _itemPositions = new Vector3[ColumnCount, RowCount];
        helper = this;
    }

    public void CreateMatrix()
    {
        Vector3 size = EmptyPrefab.renderer.bounds.size;
        float x = -((size.x * ColumnCount) / 2f);
        float y = (size.y * RowCount) / 2f + matrixSpace;
        float w = size.x / 2f;
        float h = size.y / 2f;
        float z = EmptyPrefab.transform.position.z;
        
        for (int col = 0; col < ColumnCount; col++)
        {
            float yy = y;
            
            for (int row = 0; row < RowCount; row++)
            {
                Vector3 position = new Vector3(x + w, yy - h, z);
                items[col, row] = Instantiate(EmptyPrefab, position, Quaternion.identity) as GameObject;
                itemPositions[col, row] = position;
                yy -= size.y;
            }
            
            x += size.x;
        }
    }

    public void CreateHeros()
    {
        int total = (ColumnCount * RowCount) / 2;
        int itemCount = ItemPrefabs.Length;

        for (int i = 0; i < total; i++)
        {
            GameObject prefab = ItemPrefabs[i < itemCount ? i : Random.Range(0, itemCount)];
            CreateHero(prefab);
            CreateHero(prefab);
            _totalItemCount += 2;
        }
    }

    void CreateHero(GameObject prefab)
    {
        for (int i = 0; i < Constants.RANDOM_COUNT; i++)
        {
            if (CreateItem(Random.Range(0, ColumnCount), Random.Range(0, RowCount), prefab))
            {
                return;
            }
        }

        for (int col = 0; col < ColumnCount; col++)
        {
            for (int row = 0; row < RowCount; row++)
            {
                if (CreateItem(col, row, prefab))
                {
                    return;
                }
            }
        }
    }

    bool CreateItem(int col, int row, GameObject prefab)
    {
        GameObject block = items[col, row];

        if (block != null && block.transform.childCount == 0)
        {
            CreateNewHero(prefab, block, false);
            return true;
        }

        return false;
    }

    GameObject CreateNewHero(GameObject prefab, GameObject block, bool fadeOut)
    {
        GameObject result = Instantiate(prefab) as GameObject;
        Transform tfm = result.transform;
        tfm.parent = block.transform;
        tfm.localPosition = new Vector3(0, 0, prefab.transform.position.z);

        if (fadeOut)
        {
            Color color = Color.white;
            color.a = 0f;
            result.GetComponent<SpriteRenderer>().color = color;
        }

        ApplyDefaultEffect(tfm);
        return result;
    }

    public void ApplyDefaultEffect(Transform tfm)
    {
        Sequence seq = DOTween.Sequence();
        seq.SetLoops(-1);
        seq.AppendInterval(Random.Range(0f, 3.0f));
        seq.Append(tfm.DOLocalMoveY(tfm.localPosition.y + 0.10f, DefaultEffectDuration));
        seq.Append(tfm.DOLocalMoveY(tfm.localPosition.y - 0.10f, DefaultEffectDuration));
        seq.Append(tfm.DOLocalMoveY(tfm.localPosition.y, DefaultEffectDuration));
        seq.Play();

        tfm.GetComponent<HeroController>().action = seq;
    }

    public void RemoveItem(int col, int row)
    {
        GameObject item = items[col, row];

        if (item != null)
        {
            HeroController controller = item.GetComponentInChildren<HeroController>();

            if (controller != null)
            {
                controller.StopAllActions();
            }

            Destroy(item);
            items[col, row] = null;
            _totalItemCount--;
        }
    }

    public void CheckRefreshMatrix()
    {
        if (!DetectorManager.SharedInstance().IsHaveValideBlocks())
        {
            RefreshMatrix();
        } else
        {
            AllProgressComplate();
        }
    }

    void RefreshMatrix()
    {
        List<string> tags = new List<string>();

        for (int col = 0; col < ColumnCount; col++)
        {
            for (int row = 0; row < RowCount; row++)
            {
                GameObject block = items[col, row];
                
                if (block == null)
                {
                    continue;
                }
                
                string tag = block.transform.GetChild(0).tag;
                
                if (!tags.Contains(tag))
                {
                    tags.Add(tag);
                }
            }
        }

        int tagsCount = tags.Count;

        while (true)
        {
            int i = 0;
            
            while (true)
            {
                if (i >= tagsCount)
                {
                    i = 0;
                }

                if (!RefreshChild(tags[i]) || !RefreshChild(tags[i]))
                {
                    break;
                }
                
                i++;
            }
            
            if (DetectorManager.SharedInstance().IsHaveValideBlocks())
            {
                ApplyRefreshEffect();
                break;
            } else
            {
                ClearRefreshedChilds();
            }
        }
    }

    bool RefreshChild(string tag)
    {
        HeroController controller = GetRandomHeroController();
        
        if (controller != null)
        {
            controller.tagEx = tag;
            return true;
        }

        return false;
    }

    HeroController GetRandomHeroController()
    {
        int rowIteration = 0;
        int row = Random.Range(0, RowCount);
        
        while (rowIteration < RowCount)
        {
            if (row >= RowCount)
            {
                row = 0;
            }
            
            int colIteration = 0;
            int col = Random.Range(0, ColumnCount);
            
            while (colIteration < ColumnCount)
            {
                if (col >= ColumnCount)
                {
                    col = 0;
                }
                
                GameObject block = items[col, row];

                if (block != null)
                {
                    HeroController result = block.GetComponentInChildren<HeroController>();

                    if (string.IsNullOrEmpty(result.tagEx))
                    {
                        return result;
                    }
                }
                
                col++;
                colIteration++;
            }
            
            row++;
            rowIteration++;
        }
        
        return null;
    }

    void ClearRefreshedChilds()
    {
        for (int col = 0; col < ColumnCount; col++)
        {
            for (int row = 0; row < RowCount; row++)
            {
                GameObject block = items[col, row];

                if (block != null)
                {
                    block.GetComponentInChildren<HeroController>().tagEx = null;
                }
            }
        }
    }

    void ApplyRefreshEffect()
    {
        for (int col = 0; col < ColumnCount; col++)
        {
            for (int row = 0; row < RowCount; row++)
            {
                GameObject block = items[col, row];

                if (block != null)
                {
                    GameController.SharedInstance().IncreaseProgress();
                    GameObject child = block.transform.GetChild(0).gameObject;
                    child.GetComponent<SpriteRenderer>().DOFade(0f, DurationChildExchange).OnComplete(() =>
                    {
                        OnRefreshFadeOutComplate(block, child);
                    });
                }
            }
        }
    }

    void OnRefreshFadeOutComplate(GameObject itemOne, GameObject itemTwo)
    {
        HeroController controller = itemTwo.GetComponent<HeroController>();
        string prefabTag = controller.GetTag();
        controller.StopAllActions();
        Destroy(itemTwo);
        GameObject child = CreateNewHero(GetItemPrefab(prefabTag), itemOne, true);
        child.GetComponent<SpriteRenderer>().DOFade(255f, DurationChildExchange).OnComplete(OnRefreshFadeInComplate);
    }

    void OnRefreshFadeInComplate()
    {
        GameController.SharedInstance().DecreaseProgress();

        if (!GameController.SharedInstance().isProgressing)
        {
            AllProgressComplate();
        }
    }

    GameObject GetItemPrefab(string prefabTag)
    {
        foreach (GameObject child in ItemPrefabs)
        {
            if (child.tag == prefabTag)
            {
                return child;
            }
        }

        return null;
    }

    void AllProgressComplate()
    {
        TouchManager.SharedInstance().TouchEnabled = true;
    }
}