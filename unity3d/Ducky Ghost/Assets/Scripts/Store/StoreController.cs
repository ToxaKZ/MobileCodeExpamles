using UnityEngine;
using System.Collections;
#if UNITY_IPHONE && UNITY_ANDROID
using Soomla.Store;
#endif
using System;
using DG.Tweening;

public class StoreController : MonoBehaviour
{
    private bool _isAvailable;
    private static StoreController helper = null;

    public bool isAvailable
    {
        get
        {
            return _isAvailable;
        }
    }

    public bool removeAdsPurchased
    {
        get
        {
            try
            {
                #if UNITY_IPHONE && UNITY_ANDROID
                return isAvailable ? StoreInventory.NonConsumableItemExists(Store.RemoveAdsItem.ItemId) : false;
                #else
                return false;
                #endif
            } catch
            {
                return false;
            }
        }
    }
    
    public bool turnOffTimeLimitPurchased
    {
        get
        {
            try
            {
                #if UNITY_IPHONE && UNITY_ANDROID
                return isAvailable ? StoreInventory.NonConsumableItemExists(Store.TurnOffTimeLimitItem.ItemId) : false;
                #else
                return false;
                #endif
            } catch
            {
                return false;
            }
        }
    }

    public static StoreController SharedInstance()
    {
        return helper;
    }

    void Awake()
    {
        if (helper == null)
        {
            helper = this;
            GameObject.DontDestroyOnLoad(this.gameObject);
        } else
        {
            GameObject.Destroy(this);
        }
    }

    void Start()
    {
        DOTween.Init(false, true, LogBehaviour.Default).SetCapacity(600, 150);

        #if UNITY_ANDROID
        _isAvailable = true;
        #elif UNITY_IPHONE
        _isAvailable = true;
        #else
        _isAvailable = false;
        #endif

        if (isAvailable)
        {
            #if UNITY_IPHONE && UNITY_ANDROID
            StoreEvents.OnUnexpectedErrorInStore += OnUnexpectedErrorInStore;
            StoreEvents.OnMarketPurchaseCancelled += OnMarketPurchaseCancelled;
            StoreEvents.OnMarketPurchase += OnMarketPurchase;
            StoreEvents.OnRestoreTransactionsFinished += OnRestoreTransactionsFinished;
            SoomlaStore.Initialize(new Store());
            #endif
        }

        IntroController.Scene();
    }

    public void PurchaseRemoveAds()
    {
        #if UNITY_IPHONE && UNITY_ANDROID
        try
        {
            StoreInventory.BuyItem(Store.RemoveAdsItem.ItemId);
        } catch (Exception ex)
        {
            StoreEventHandler(false, ex.Message);
        }
        #endif
    }
    
    public void PurchaseTurnOffTimeLimit()
    {
        #if UNITY_IPHONE && UNITY_ANDROID
        try
        {
            StoreInventory.BuyItem(Store.TurnOffTimeLimitItem.ItemId);
        } catch (Exception ex)
        {
            StoreEventHandler(false, ex.Message);
        }
        #endif
    }
    
    public void RestorePurchases()
    {
        #if UNITY_IPHONE && UNITY_ANDROID
        try
        {
            SoomlaStore.RestoreTransactions();
        } catch (Exception ex)
        {
            StoreEventHandler(false, ex.Message);
        }
        #endif
    }

    #if UNITY_IPHONE && UNITY_ANDROID
    private void OnUnexpectedErrorInStore(string message)
    {
        StoreEventHandler(false, message);
    }
    
    private void OnMarketPurchaseCancelled(PurchasableVirtualItem item)
    {
        StoreEventHandler(true, null);
    }
    
    private void OnMarketPurchase(PurchasableVirtualItem pvi, string purchaseToken, string payload)
    {
        StoreEventHandler(false, null);
    }
    
    private void OnRestoreTransactionsFinished(bool success)
    {
        StoreEventHandler(!success, null);
    }

    void StoreEventHandler(bool cancelled, string error)
    {
        ShopController controller = Camera.main.GetComponent<ShopController>();

        if (controller != null)
        {
            controller.StoreEventHandler(cancelled, error);
        }
    }

    private sealed class Store : IStoreAssets
    {
        private const string PRODUCT_ID_REMOVE_ADS = "com.shopanov.yelnar.duckyghost.removeadsex";
        private const string PRODUCT_ID_TURN_OFF_TIME_LIMIT = "com.shopanov.yelnar.duckyghost.unlockthetimelimitex";
        
        public int GetVersion()
        {
            return 1;
        }
        
        public VirtualCurrency[] GetCurrencies()
        {
            return new VirtualCurrency[]{};
        }
        
        public VirtualGood[] GetGoods()
        {
            return new VirtualGood[]{};
        }
        
        public VirtualCurrencyPack[] GetCurrencyPacks()
        {
            return new VirtualCurrencyPack[]{};
        }
        
        public VirtualCategory[] GetCategories()
        {
            return new VirtualCategory[]{};
        }
        
        public NonConsumableItem[] GetNonConsumableItems()
        {
            return new NonConsumableItem[]
            {
                RemoveAdsItem,
                TurnOffTimeLimitItem
            };
        }
        
        public static NonConsumableItem RemoveAdsItem = 
            new NonConsumableItem(string.Empty, string.Empty, PRODUCT_ID_REMOVE_ADS, new PurchaseWithMarket(
                new MarketItem(PRODUCT_ID_REMOVE_ADS, MarketItem.Consumable.NONCONSUMABLE, 0.99)));
        public static NonConsumableItem TurnOffTimeLimitItem = 
            new NonConsumableItem(string.Empty, string.Empty, PRODUCT_ID_TURN_OFF_TIME_LIMIT, new PurchaseWithMarket(
                new MarketItem(PRODUCT_ID_TURN_OFF_TIME_LIMIT, MarketItem.Consumable.NONCONSUMABLE, 0.99)));
    }
    #endif
}