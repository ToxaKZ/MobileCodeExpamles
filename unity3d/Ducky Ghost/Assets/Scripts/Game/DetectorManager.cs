using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DetectorManager : MonoBehaviour
{
    public int MaxTurns = 3;
    private static DetectorManager helper = null;

    public static DetectorManager SharedInstance()
    {
        return helper;
    }

    void Awake()
    {
        helper = this;
    }

    void Start()
    {
    }
    
    void Update()
    {
    }

    public bool IsValideBlocks(int col, int row, GameObject source, GameObject dest, int limitIteration, List<LineVector> vectors)
    {
        bool result = false;

        if (vectors != null)
        {
            vectors.Clear();
        }

        if (source.GetComponentInChildren<HeroController>().GetTag() == dest.GetComponentInChildren<HeroController>().GetTag())
        {
            while (!result && limitIteration <= MaxTurns)
            {
                if (HorizontalPathCheck(col, row, source, dest, 1, limitIteration, Direction.right, false, vectors)
                    || HorizontalPathCheck(col, row, source, dest, 1, limitIteration, Direction.left, false, vectors))
                {
                    result = true;
                } else
                {
                    if (VerticalPathCheck(col, row, source, dest, 1, limitIteration, Direction.up, false, vectors)
                        || VerticalPathCheck(col, row, source, dest, 1, limitIteration, Direction.down, false, vectors))
                    {
                        result = true;
                    }
                }
                
                limitIteration++;
            }
        }
        
        return result;
    }

    bool HorizontalPathCheck(int col, int row, GameObject source, GameObject dest, int iteration, int limit, Direction direction, bool isMatrixOut, List<LineVector> vectors)
    {
        if (iteration > limit || row < -1 || row > MatrixController.SharedInstance().RowCount)
        {
            return false;
        }
        
        float w = source.renderer.bounds.size.x;
        float hw = w / 2f;
        bool result = false;
        LineVector vector = new LineVector();
        vector.direction = direction;
        vector.distance = 0;
        
        while (!result && col >= -1 && col <= MatrixController.SharedInstance().ColumnCount)
        {
            bool increase = true;
            bool isHorizontalMatrixOut = col == -1 || col == MatrixController.SharedInstance().ColumnCount;
            bool isVerticalMatrixOut = row == -1 || row == MatrixController.SharedInstance().RowCount;
            
            if (isHorizontalMatrixOut || isVerticalMatrixOut)
            {
                if (VerticalPathCheck(col, row - 1, source, dest, iteration + 1, limit, Direction.up, isVerticalMatrixOut, vectors)
                    || VerticalPathCheck(col, row + 1, source, dest, iteration + 1, limit, Direction.down, isVerticalMatrixOut, vectors))
                {
                    result = true;
                }
            } else
            {
                GameObject block = MatrixController.SharedInstance().items[col, row];
                
                if (block == dest)
                {
                    result = true;
                } else if (block == source || block == null)
                {
                    if (block == source)
                    {
                        increase = false;
                    }
                    
                    if (VerticalPathCheck(col, row - 1, source, dest, iteration + 1, limit, Direction.up, false, vectors)
                        || VerticalPathCheck(col, row + 1, source, dest, iteration + 1, limit, Direction.down, false, vectors))
                    {
                        result = true;
                    }
                } else
                {
                    break;
                }
            }
            
            if (increase)
            {
                vector.distance += isHorizontalMatrixOut || isMatrixOut ? hw + LineManager.SharedInstance().LineMargin : w;
                isMatrixOut = false;
            }
            
            col = direction == Direction.left ? col - 1 : col + 1;
        }
        
        if (result && vectors != null)
        {
            vectors.Add(vector);
        }
        
        return result;
    }
    
    bool VerticalPathCheck(int col, int row, GameObject source, GameObject dest, int iteration, int limit, Direction direction, bool isMatrixOut, List<LineVector> vectors)
    {
        if (iteration > limit || col < -1 || col > MatrixController.SharedInstance().ColumnCount)
        {
            return false;
        }
        
        float h = source.renderer.bounds.size.y;
        float hh = h / 2f;
        bool result = false;
        LineVector vector = new LineVector();
        vector.direction = direction;
        vector.distance = 0;
        
        while (!result && row >= -1 && row <= MatrixController.SharedInstance().RowCount)
        {
            bool increase = true;
            bool isHorizontalMatrixOut = col == -1 || col == MatrixController.SharedInstance().ColumnCount;
            bool isVerticalMatrixOut = row == -1 || row == MatrixController.SharedInstance().RowCount;
            
            if (isHorizontalMatrixOut || isVerticalMatrixOut)
            {
                if (HorizontalPathCheck(col + 1, row, source, dest, iteration + 1, limit, Direction.right, isHorizontalMatrixOut, vectors)
                    || HorizontalPathCheck(col - 1, row, source, dest, iteration + 1, limit, Direction.left, isHorizontalMatrixOut, vectors))
                {
                    result = true;
                }
            } else
            {
                GameObject block = MatrixController.SharedInstance().items[col, row];
                
                if (block == dest)
                {
                    result = true;
                } else if (block == source || block == null)
                {
                    if (block == source)
                    {
                        increase = false;
                    }
                    
                    if (HorizontalPathCheck(col + 1, row, source, dest, iteration + 1, limit, Direction.right, false, vectors)
                        || HorizontalPathCheck(col - 1, row, source, dest, iteration + 1, limit, Direction.left, false, vectors))
                    {
                        result = true;
                    }
                } else
                {
                    break;
                }
            }
            
            if (increase)
            {
                vector.distance += isVerticalMatrixOut || isMatrixOut ? hh + LineManager.SharedInstance().LineMargin : h;
                isMatrixOut = false;
            }
            
            row = direction == Direction.up ? row - 1 : row + 1;
        }
        
        if (result && vectors != null)
        {
            vectors.Add(vector);
        }
        
        return result;
    }

    public bool IsHaveValideBlocks()
    {
        return IsHaveValideBlocks(null, 0, 0);
    }

    bool IsHaveValideBlocks(GameObject source, int sourceCol, int sourceRow)
    {
        bool result;

        for (int col = 0; col < MatrixController.SharedInstance().ColumnCount; col++)
        {
            for (int row = 0; row < MatrixController.SharedInstance().RowCount; row++)
            {
                GameObject block = MatrixController.SharedInstance().items[col, row];

                if (block == null || source == block)
                {
                    continue;
                }

                if (source == null)
                {
                    result = IsHaveValideBlocks(block, col, row);
                } else
                {
                    result = IsValideBlocks(sourceCol, sourceRow, source, block, MaxTurns, null);
                }

                if (result)
                {
                    return true;
                }
            }
        }

        return false;
    }
}