using UnityEngine;
using System.Collections;

public class ButtonController : MonoBehaviour
{
    public float PressedPersent = 10f;
    private Transform transformCached;
    private bool isMouseMove;
    private Vector3 pressedScale;
    private Vector3 defaultScale;
    private Renderer rendererCached;
    public delegate void OnButtonClickEvent(GameObject sender);
    private Vector2 size;
    private Vector2 halfSize;

    public GameState pressedMode = GameState.Gaming;

    public event OnButtonClickEvent OnClick = null;

    void Start()
    {
        rendererCached = gameObject.renderer;
        transformCached = gameObject.transform;
        defaultScale = transformCached.localScale;
        pressedScale = new Vector3(defaultScale.x - (defaultScale.x / 100f * PressedPersent), defaultScale.y - (defaultScale.y / 100f * PressedPersent), defaultScale.z);
        isMouseMove = false;

        Bounds bound = rendererCached.bounds;
        size = new Vector2(bound.size.x, bound.size.y);
        halfSize = new Vector2(size.x / 2f, size.y / 2f);
    }
    
    void Update()
    {
        if (UtilitiesHelper.SharedInstance().gameState == pressedMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (OnButton())
                {
                    TouchBegan();
                }
            } else if (Input.GetMouseButton(0) && isMouseMove)
            {
                TouchMove();
            } else if (Input.GetMouseButtonUp(0))
            {
                TouchEnd();
            }
        }
    }

    void TouchBegan()
    {
        isMouseMove = true;
        PressedState();
    }

    void TouchMove()
    {
        if (OnButton())
        {
            PressedState();
        } else
        {
            NormalState();
        }
    }

    void TouchEnd()
    {
        if (OnButton() && OnClick != null)
        {
            OnClick(gameObject);
        }

        isMouseMove = false;
        NormalState();
    }

    bool OnButton()
    {
        Vector3 pos = transformCached.position;
        Rect boundingBox = new Rect(pos.x - halfSize.x, pos.y - halfSize.y, size.x, size.y);
        return boundingBox.Contains(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    void PressedState()
    {
        transformCached.localScale = pressedScale;
    }

    void NormalState()
    {
        transformCached.localScale = defaultScale;
    }
}