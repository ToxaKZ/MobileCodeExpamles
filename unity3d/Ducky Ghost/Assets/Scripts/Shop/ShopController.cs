using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShopController : MonoBehaviour
{
    public GameObject Background = null;
    public List<GameObject> MarketButtons = null;
    public List<GameObject> MarketTitles = null;
    public float ButtonSpace = 1f;
    public float LabelSpace = 1f;
    private GameState gameState;

    public static void Scene()
    {
        Application.LoadLevel(Constants.SCENE_NAME_SHOP);
    }

    void Start()
    {
        InitBackground();
        PrepareTitles();
        PrepareButtons();
        UtilitiesHelper.SharedInstance().gameState = GameState.Shop;
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

    void PrepareTitles()
    {
        foreach (GameObject child in MarketTitles)
        {
            child.GetComponent<TitleController>().SetText(LocalizationHelper.SharedInstance().GetValue(child.tag));
        }
    }

    void PrepareButtons()
    {
        foreach (GameObject child in MarketButtons)
        {
            ButtonController controller = child.GetComponent<ButtonController>();
            
            if (child.tag == Constants.TAG_NAME_REMOVE_ADS)
            {
                controller.OnClick += RemoveAdsCommandExecute;
            } else if (child.tag == Constants.TAG_NAME_TURN_OFF_TIME_LIMIT)
            {
                controller.OnClick += TurnOffTimeLimitCommandExecute;
            } else if (child.tag == Constants.TAG_NAME_RESTORE)
            {
                controller.OnClick += RestoreCommandExecute;
            }
        }

        GameObject button = GameObject.FindGameObjectWithTag(Constants.TAG_NAME_MAIN_MENU);

        if (button != null)
        {
            button.GetComponent<ButtonController>().OnClick += IntroCommandExecute;
            Transform tfm = button.transform;
            Vector3 size = button.renderer.bounds.size;
            tfm.position = new Vector3(-(UtilitiesHelper.SharedInstance().worldScreenWidth / 2f - size.x / 2f),
                                       -(UtilitiesHelper.SharedInstance().worldScreenHeight / 2f - size.y / 2f),
                                       tfm.position.z);
        }

        RefreshMarketButtons();
    }

    void RemoveAdsCommandExecute(GameObject sender)
    {
        if (StoreController.SharedInstance().isAvailable && !StoreController.SharedInstance().removeAdsPurchased)
        {
            if (UtilitiesHelper.SharedInstance().internetReachable)
            {
                gameState = UtilitiesHelper.SharedInstance().gameState;
                UtilitiesHelper.SharedInstance().gameState = GameState.Purchasing;
                StoreController.SharedInstance().PurchaseRemoveAds();
            } else
            {
                AlertMessage.DisplayAlert(LocalizationHelper.SharedInstance().GetValue(LocalizationHelper.RES_KEY_INTERNET_NO));
            }
        }
    }

    void TurnOffTimeLimitCommandExecute(GameObject sender)
    {
        if (StoreController.SharedInstance().isAvailable && !StoreController.SharedInstance().turnOffTimeLimitPurchased)
        {
            if (UtilitiesHelper.SharedInstance().internetReachable)
            {
                gameState = UtilitiesHelper.SharedInstance().gameState;
                UtilitiesHelper.SharedInstance().gameState = GameState.Purchasing;
                StoreController.SharedInstance().PurchaseTurnOffTimeLimit();
            } else
            {
                AlertMessage.DisplayAlert(LocalizationHelper.SharedInstance().GetValue(LocalizationHelper.RES_KEY_INTERNET_NO));
            }
        }        
    }

    void RestoreCommandExecute(GameObject sender)
    {
        if (StoreController.SharedInstance().isAvailable && 
            (!StoreController.SharedInstance().removeAdsPurchased || !StoreController.SharedInstance().turnOffTimeLimitPurchased))
        {
            if (UtilitiesHelper.SharedInstance().internetReachable)
            {
                gameState = UtilitiesHelper.SharedInstance().gameState;
                UtilitiesHelper.SharedInstance().gameState = GameState.Purchasing;
                StoreController.SharedInstance().RestorePurchases();
            } else
            {
                AlertMessage.DisplayAlert(LocalizationHelper.SharedInstance().GetValue(LocalizationHelper.RES_KEY_INTERNET_NO));
            }
        }        
    }

    public void StoreEventHandler(bool cancelled, string error)
    {
        if (!string.IsNullOrEmpty(error))
        {
            AlertMessage.DisplayAlert(error);
        } else if (!cancelled)
        {
            RefreshMarketButtons();
        }
        
        UtilitiesHelper.SharedInstance().gameState = gameState;
    }

    void IntroCommandExecute(GameObject sender)
    {
        IntroController.Scene();
    }

    void RefreshMarketButtons()
    {
        if (StoreController.SharedInstance().removeAdsPurchased)
        {
            RemoveButtonWithTitleByTag(Constants.TAG_NAME_REMOVE_ADS);
        }

        if (StoreController.SharedInstance().turnOffTimeLimitPurchased)
        {
            RemoveButtonWithTitleByTag(Constants.TAG_NAME_TURN_OFF_TIME_LIMIT);
        }

        if (StoreController.SharedInstance().removeAdsPurchased && StoreController.SharedInstance().turnOffTimeLimitPurchased)
        {
            RemoveButtonWithTitleByTag(Constants.TAG_NAME_RESTORE);
        }

        int count = MarketButtons.Count;
        float hh = (ButtonSpace * (count - 1)) + (LabelSpace * MarketTitles.Count);

        for (int i = 0; i < count; i++)
        {
            hh += MarketButtons[i].renderer.bounds.size.y + MarketTitles[i].renderer.bounds.size.y;
        }

        float y = hh / 2f;
        
        for (int i = 0; i < count; i++)
        {
            Transform tfm = MarketButtons[i].transform;
            Vector3 size = MarketButtons[i].renderer.bounds.size;
            float hs = size.y / 2f;
            tfm.position = new Vector3(tfm.position.x, y - hs, tfm.position.z);
            Transform labelTfm = MarketTitles[i].transform;
            Vector3 labelSize = MarketTitles[i].renderer.bounds.size;
            labelTfm.position = new Vector3(labelTfm.position.x, tfm.position.y - hs - LabelSpace - labelSize.y / 2f, labelTfm.position.z);
            y -= size.y + ButtonSpace + LabelSpace + labelSize.y;
        }
    }

    GameObject GetObjectByTag(List<GameObject> collection, string tag)
    {
        if (collection.Count > 0)
        {
            foreach (GameObject child in collection)
            {
                if (tag == child.tag)
                {
                    return child;
                }
            }
        }

        return null;
    }

    void RemoveButtonWithTitleByTag(string tag)
    {
        RemoveGameObjectByTag(MarketButtons, tag);
        RemoveGameObjectByTag(MarketTitles, tag);
    }

    void RemoveGameObjectByTag(List<GameObject> collection, string tag)
    {
        GameObject obj = GetObjectByTag(collection, tag);
        
        if (obj != null)
        {
            collection.Remove(obj);
            Destroy(obj);
        }
    }
}