using UnityEngine;
using System.Collections;

public class LevelController : MonoBehaviour
{
    public GameObject Background = null;
    public GameObject IntroButton = null;

    public static void Scene()
    {
        Application.LoadLevel(Constants.SCENE_NAME_LEVEL);
    }

    void Start()
    {
        InitBackground();
        PrepareButton();
        UtilitiesHelper.SharedInstance().gameState = GameState.Level;
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

    void PrepareButton()
    {
        Transform tfm = IntroButton.transform;
        Vector3 size = IntroButton.renderer.bounds.size;
        tfm.position = new Vector3(-(UtilitiesHelper.SharedInstance().worldScreenWidth / 2f - size.x / 2f),
                                   -(UtilitiesHelper.SharedInstance().worldScreenHeight / 2f - size.y / 2f),
                                   tfm.position.z);
        IntroButton.GetComponent<ButtonController>().OnClick += (GameObject sender) => 
        {
            IntroController.Scene();
        };
    }
}