using UnityEngine;
using System.Collections;

public class LevelCellController : MonoBehaviour
{
    public Sprite LockSprite = null;
    private GameObject lockGameObject;
    private TextMesh textMeshCached;
    private Transform transformCached;
    private GameState pressedMode;
    private ButtonController buttonController;

    public int levelNumber { get; set; }

    public bool levelEnabled { get; set; }

    void Awake()
    {
        lockGameObject = null;
        transformCached = transform;
        textMeshCached = GetComponentInChildren<TextMesh>();
        buttonController = GetComponent<ButtonController>();
        pressedMode = buttonController.pressedMode;
    }

    void Start()
    {
    }
    
    void Update()
    {    
    }

    public void Refresh()
    {
        if (lockGameObject != null)
        {
            Destroy(lockGameObject);
        }

        if (levelEnabled)
        {
            textMeshCached.text = levelNumber.ToString();
            buttonController.pressedMode = pressedMode;
        } else
        {
            textMeshCached.text = string.Empty;
            lockGameObject = new GameObject();
            lockGameObject.AddComponent<SpriteRenderer>().sprite = LockSprite;
            Transform tfm = lockGameObject.transform;
            tfm.parent = transformCached;
            tfm.localPosition = new Vector3(0.1f, -0.4f, transformCached.position.z - 1f);
            buttonController.pressedMode = GameState.Intro;
        }
    }

    public void SetButtonOnClick(ButtonController.OnButtonClickEvent onClick)
    {
        buttonController.OnClick += onClick;
    }
}