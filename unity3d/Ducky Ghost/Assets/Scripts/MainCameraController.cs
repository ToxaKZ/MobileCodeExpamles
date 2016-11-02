using UnityEngine;
using System.Collections;

public class MainCameraController : MonoBehaviour
{
    public float DesignScreenHeight = 0f;
    public float DesignScreenWidth = 0f;
    public int DesignPixelToUnits = 100;
    public GameObject AlertPrefab = null;
    public GameObject QuitDialogPrefab = null;

    void Awake()
    {
        UtilitiesHelper.SharedInstance().ResizeMainCamera(DesignScreenWidth, DesignScreenHeight, DesignPixelToUnits);
    }

    void Start()
    {
        GCHelper.SharedInstance().Authenticate();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && UtilitiesHelper.SharedInstance().gameState != GameState.Quit)
        {
            if (UtilitiesHelper.SharedInstance().gameState == GameState.Gaming)
            {
                GameController.SharedInstance().PauseGame(false);
            }

            Instantiate(QuitDialogPrefab);
        }
    }
}