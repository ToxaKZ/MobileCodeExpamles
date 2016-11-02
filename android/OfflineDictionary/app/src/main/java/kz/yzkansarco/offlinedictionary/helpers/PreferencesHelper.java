package kz.yzkansarco.offlinedictionary.helpers;

import android.content.Context;
import android.content.SharedPreferences;
import android.preference.PreferenceManager;
import android.support.annotation.Nullable;

public final class PreferencesHelper {
    private static final String KEY_NAME_HISTORY = "history";
    private static final String KEY_NAME_FAVORITES = "favorites";
    private static final String KEY_NAME_NO_ADS = "noAds";
    private static final String KEY_NAME_DIRECTION_ID = "directionId";

    private static PreferencesHelper mHelper = null;

    public static PreferencesHelper getInstance() {
        if (mHelper == null) {
            mHelper = new PreferencesHelper();
        }

        return mHelper;
    }

    public void setHistory(final Context ctx, final String value) {
        final SharedPreferences sharedPreferences =
                PreferenceManager.getDefaultSharedPreferences(ctx);
        sharedPreferences.edit().putString(KEY_NAME_HISTORY, value).apply();
    }

    @Nullable
    public String getHistory(final Context ctx) {
        final SharedPreferences sharedPreferences =
                PreferenceManager.getDefaultSharedPreferences(ctx);
        return sharedPreferences.getString(KEY_NAME_HISTORY, null);
    }

    public void setFavorites(final Context ctx, final String value) {
        final SharedPreferences sharedPreferences =
                PreferenceManager.getDefaultSharedPreferences(ctx);
        sharedPreferences.edit().putString(KEY_NAME_FAVORITES, value).apply();
    }

    @Nullable
    public String getFavorites(final Context ctx) {
        final SharedPreferences sharedPreferences =
                PreferenceManager.getDefaultSharedPreferences(ctx);
        return sharedPreferences.getString(KEY_NAME_FAVORITES, null);
    }

    public void setNoAds(final Context ctx, final boolean value) {
        final SharedPreferences sharedPreferences =
                PreferenceManager.getDefaultSharedPreferences(ctx);
        sharedPreferences.edit().putBoolean(KEY_NAME_NO_ADS, value).apply();
    }

    public boolean getNoAds(final Context ctx) {
        final SharedPreferences sharedPreferences =
                PreferenceManager.getDefaultSharedPreferences(ctx);
        return sharedPreferences.getBoolean(KEY_NAME_NO_ADS, false);
    }

    public void setDirectionId(final Context ctx, final int value) {
        final SharedPreferences sharedPreferences =
                PreferenceManager.getDefaultSharedPreferences(ctx);
        sharedPreferences.edit().putInt(KEY_NAME_DIRECTION_ID, value).apply();
    }

    public int getDirectionId(final Context ctx) {
        final SharedPreferences sharedPreferences =
                PreferenceManager.getDefaultSharedPreferences(ctx);
        return sharedPreferences.getInt(KEY_NAME_DIRECTION_ID, 0);
    }
}