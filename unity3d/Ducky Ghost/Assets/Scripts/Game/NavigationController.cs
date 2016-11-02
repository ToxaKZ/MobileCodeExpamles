using UnityEngine;
using System.Collections;
using DG.Tweening;

public class NavigationController : MonoBehaviour
{
    public float HighlightInterval = 0.5f;
    public int HighlightCount = 2;
    public float Opacity = 0.5f;
    public float FadeOutDuration = 0.5f;
    private TitleController[] children;
    private Color defaultColor;
    private Color selectedColor;
    private int childIndex;
    private int childCount;
    private Direction direction;

    void Awake()
    {
        children = GetComponentsInChildren<TitleController>();
        childCount = children.Length;
    }

    public void Initialization(Direction dir, Color color)
    {
        direction = dir;
        defaultColor = new Color(color.r, color.g, color.b, Opacity);
        selectedColor = color;

        string ch;

        if (direction == Direction.right || direction == Direction.down)
        {
            ch = ">";
            childIndex = 0;
        } else
        {
            ch = "<";
            childIndex = childCount - 1;
        }

        foreach (TitleController child in children)
        {
            child.SetText(ch);
            child.SetColor(defaultColor);
        }

        if (direction == Direction.up || direction == Direction.down)
        {
            transform.Rotate(0f, 0f, -90f);
        }

        Sequence seq = DOTween.Sequence();
        seq.SetLoops(childCount * HighlightCount);
        seq.OnComplete(NavigationComplete);
        seq.AppendCallback(Highlight);
        seq.AppendInterval(HighlightInterval);
        seq.Play();
    }

    void Highlight()
    {
        bool increase;

        if (direction == Direction.right || direction == Direction.down)
        {
            if (childIndex >= childCount)
            {
                childIndex = 0;
            }

            increase = true;
        } else
        {
            if (childIndex < 0)
            {
                childIndex = childCount - 1;
            }

            increase = false;
        }

        children[childIndex].SetColor(selectedColor);
        TextMesh mesh = children[childIndex].GetTextMesh();

        DOTween.To(() => 
        {
            return mesh.color;
        }, (Color color) => 
        {
            mesh.color = color;
        }, defaultColor, FadeOutDuration);

        if (increase)
        {
            childIndex++;
        } else
        {
            childIndex--;
        }
    }

    void NavigationComplete()
    {
        Destroy(gameObject);
        GameController.SharedInstance().DecreaseProgress();

        if (!GameController.SharedInstance().isProgressing)
        {
            MoveManager.SharedInstance().ShowGoMessage();
        }
    }
}