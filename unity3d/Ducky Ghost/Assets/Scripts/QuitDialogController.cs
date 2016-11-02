using UnityEngine;
using System.Collections;

public class QuitDialogController : MonoBehaviour
{
    public GameObject TitlePrefab = null;
    public GameObject Background = null;
    public GameObject[] Buttons = null;
    private GameState gameState;

    void Start()
    {
        gameState = UtilitiesHelper.SharedInstance().gameState;
        InitBackground();
        PrepareTitle();
        PrepareButtons();
        UtilitiesHelper.SharedInstance().gameState = GameState.Quit;
    }

    void Update()
    {
    }

    void InitBackground()
    {
        Bounds bounds = Background.renderer.bounds;
        Transform tfm = Background.transform;
        tfm.localScale = new Vector3(UtilitiesHelper.SharedInstance().worldScreenWidth / bounds.size.x,
                                     UtilitiesHelper.SharedInstance().worldScreenHeight / bounds.size.y,
                                     tfm.localScale.z);
    }

    void PrepareTitle()
    {
        GameObject title = Instantiate(TitlePrefab) as GameObject;
        Transform tfm = title.transform;
        tfm.parent = transform;
        title.GetComponent<TitleController>().SetText(LocalizationHelper.SharedInstance().GetValue(LocalizationHelper.RES_KEY_QUIT_FROM_TH_GAME));
        UtilitiesHelper.SharedInstance().ApplySortingLayer(title, Constants.SORTING_LAYER_NAME_QUIT);
    }

    void PrepareButtons()
    {
        foreach (GameObject child in Buttons)
        {
            ButtonController controller = child.GetComponent<ButtonController>();
            
            if (child.tag == Constants.TAG_NAME_YES)
            {
                controller.OnClick += YesCommandExecute;
            } else if (child.tag == Constants.TAG_NAME_NO)
            {
                controller.OnClick += NoCommandExecute;
            }
        }
    }

    void YesCommandExecute(GameObject sender)
    {
        Application.Quit();
    }

    void NoCommandExecute(GameObject sender)
    {
        if (gameState == GameState.Gaming)
        {
            GameController.SharedInstance().RestoreGame();
        }

        UtilitiesHelper.SharedInstance().gameState = gameState;
        Destroy(gameObject);
    }
}