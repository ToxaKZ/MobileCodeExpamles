package kz.yzkansarco.offlinedictionary.helpers;

import android.content.Context;
import android.support.annotation.NonNull;

import com.google.android.gms.ads.AdListener;
import com.google.android.gms.ads.AdRequest;
import com.google.android.gms.ads.AdView;
import com.google.android.gms.ads.InterstitialAd;

import kz.yzkansarco.offlinedictionary.R;

public final class AdsHelper {
    private static AdsHelper mHelper = null;
    private InterstitialAd mInterstitialAd;

    public static AdsHelper getInstance() {
        if (mHelper == null) {
            mHelper = new AdsHelper();
        }

        return mHelper;
    }

    protected AdsHelper() {
        mInterstitialAd = null;
    }

    @NonNull
    private InterstitialAd newInterstitialAd(@NonNull final Context ctx) {
        final InterstitialAd result = new InterstitialAd(ctx);
        result.setAdUnitId(ctx.getString(R.string.interstitial_ad_unit_id));
        result.setAdListener(new AdListener() {
            @Override
            public void onAdLoaded() {
            }

            @Override
            public void onAdFailedToLoad(int errorCode) {
            }

            @Override
            public void onAdClosed() {
                mInterstitialAd = null;
            }
        });

        return result;
    }

    public boolean load(final Context ctx) {
        if (mInterstitialAd == null) {
            mInterstitialAd = newInterstitialAd(ctx);

            final AdRequest adRequest = new AdRequest.Builder()
                    .setRequestAgent("android_studio:ad_template")
                    .build();
            mInterstitialAd.loadAd(adRequest);
        }

        return mInterstitialAd.isLoaded();
    }

    public void show() {
        mInterstitialAd.show();
    }

    public void setupAds(@NonNull final AdView adView) {
        final AdRequest adRequest = new AdRequest.Builder()
                .setRequestAgent("android_studio:ad_template")
                .build();
        adView.loadAd(adRequest);
    }
}