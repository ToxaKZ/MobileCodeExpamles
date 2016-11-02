using UnityEngine;
using System.Collections;
using DG.Tweening;

public class HeroController : MonoBehaviour
{
    public Sequence action { get; set; }

    public string tagEx { get; set; }

    void Awake()
    {
        action = null;
        tagEx = null;
    }

    void Start()
    {
    }
    
    void Update()
    {    
    }

    public void StopAllActions()
    {
        if (action != null)
        {
            action.Kill();
            action = null;
        }
    }

    public string GetTag()
    {
        return !string.IsNullOrEmpty(tagEx) ? tagEx : tag;
    }
}