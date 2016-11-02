using UnityEngine;
using System.Collections;

public class TitleController : MonoBehaviour
{
    private TextMesh[] textMeshCached;

    void Awake()
    {
        textMeshCached = GetComponentsInChildren<TextMesh>();
    }

    void Start()
    {   
    }
    
    void Update()
    {    
    }

    public void SetText(string text)
    {
        foreach (TextMesh child in textMeshCached)
        {
            child.text = text;
        }
    }

    public void SetColor(Color color)
    {
        textMeshCached[1].color = color;
    }

    public void SetShadowColor(Color color, bool ignorAlpha)
    {
        if (ignorAlpha)
        {
            color.a = textMeshCached[0].color.a;
        }

        textMeshCached[0].color = color;
    }

    public void SetFontSize(int fontSize)
    {
        foreach (TextMesh child in textMeshCached)
        {
            child.fontSize = fontSize;
        }
    }

    public TextMesh GetTextMesh()
    {
        return textMeshCached[1];
    }
}