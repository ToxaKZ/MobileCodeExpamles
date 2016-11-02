using GoogleMobileAds.Api;
using System;

public sealed class AdsHelper
{
    #if UNITY_WP8
    private bool created = false;
    #else
    private BannerView AdView = null;
    private InterstitialAd interstitial = null;
    #endif
    
    public bool isAvailable
    {
        get
        {
            #if UNITY_ANDROID
            return true;
            #elif UNITY_IPHONE
            return true;
            #elif UNITY_WP8
            return true;
            #else
            return false;
            #endif
        }
    }
    
    private string AdUnitID
    {
        get
        {
            #if UNITY_ANDROID
            return "ca-app-pub-3915512366261784/7554790559";
            #elif UNITY_IPHONE
            return "ca-app-pub-3915512366261784/2984990154";
            #elif UNITY_WP8
            return "ca-app-pub-3915512366261784/9861256557";
            #else
            return null;
            #endif
        }
    }
    
    private string interstitialAdUnitID
    {
        get
        {
            #if UNITY_ANDROID
            return "ca-app-pub-3915512366261784/9726498957";
            #elif UNITY_IPHONE
            return "ca-app-pub-3915512366261784/3679965356";
            #else   
            return null;
            #endif
        }
    }
    
    private static AdsHelper Helper = null;
    
    public static AdsHelper SharedInstance()
    {
        if (Helper == null)
        {
            Helper = new AdsHelper();
        }
        
        return Helper;
    }
    
    private AdsHelper()
    {
        if (isAvailable)
        {
            #if !UNITY_WP8
            if (!StoreController.SharedInstance().removeAdsPurchased)
            {
                InitAdView();
                InitInterstitialAd();
            }
            #endif
        }
    }
    
    #if !UNITY_WP8
    void InitAdView()
    {
        AdView = new BannerView(AdUnitID, AdSize.SmartBanner, AdPosition.Bottom);
        AdView.LoadAd(CreateAdRequest());
        AdView.Hide();
    }
    
    void InitInterstitialAd()
    {
        interstitial = new InterstitialAd(interstitialAdUnitID);
        interstitial.LoadAd(CreateAdRequest());
        interstitial.AdClosed += HandleAdClosed;
    }
    
    void HandleAdClosed(object sender, EventArgs args)
    {
        interstitial.Destroy();
        interstitial = null;
        InitInterstitialAd();
    }
    
    AdRequest CreateAdRequest()
    {
        return new AdRequest.Builder()
        //            .AddTestDevice(AdRequest.TestDeviceSimulator)
        //            .AddTestDevice("0319a67e61135b6d06cdaf0b686ae077")
        //            .AddTestDevice("1de6a7dc166b5b73d7d70957591a41e2")
        //            .AddTestDevice("7F1AD720A51CF9DAE3D124F9D7C3D0ED")
        //            .AddTestDevice("6D86B06A193A33F23B6CC733C4224461")
        //            .AddTestDevice("749814CB923C58E6316CDB96C0695B85")
            .AddKeyword("game")
            .Build();
    }
    #endif
    
    public void Show()
    {
        if (!StoreController.SharedInstance().removeAdsPurchased)
        {
            #if UNITY_WP8
            Windows_Ad_Plugin.Helper.Instance.CreateAd(AdUnitID,
                                                       Windows_Ad_Plugin.Helper.AD_FORMATS.SMART_BANNER,
                                                       Windows_Ad_Plugin.Helper.HORIZONTAL_ALIGNMENT.CENTER,
                                                       Windows_Ad_Plugin.Helper.VERTICAL_ALIGNMENT.BOTTOM, 0f, 0f, false);
            created = true;
            #else
            if (AdView != null)
            {
                AdView.Show();
            }
            #endif
        }
    }
    
    public void Hide()
    {
        #if UNITY_WP8
        if (created)
        {
            created = false;
            Windows_Ad_Plugin.Helper.Instance.HandleDestruction();
        }            
        #else
        if (AdView != null)
        {
            AdView.Hide();
        }
        #endif
    }
    
    public void ShowInterstitial()
    {
        if (!StoreController.SharedInstance().removeAdsPurchased)
        {
            #if !UNITY_WP8
            if (interstitial != null && interstitial.IsLoaded())
            {
                interstitial.Show();
            }
            #endif
        }
    }
}