package kz.yzkansarco.offlinedictionary.helpers;

import android.content.Context;
import android.content.DialogInterface;
import android.support.v7.app.AlertDialog;

import kz.yzkansarco.offlinedictionary.R;

public final class ExceptionHelper {

    private static ExceptionHelper mHelper = null;

    public static ExceptionHelper getInstance() {
        if (mHelper == null) {
            mHelper = new ExceptionHelper();
        }

        return mHelper;
    }

    private void showWindow(final Context ctx, final Exception e) {
        new AlertDialog.Builder(ctx)
                .setMessage(e.getMessage())
                .setCancelable(true)
                .setNegativeButton(R.string.close, new DialogInterface.OnClickListener() {
                    public void onClick(DialogInterface dialog, int id) {
                        dialog.cancel();
                    }
                }).create().show();
    }

    public void alertException(final Context ctx, final Exception e) {
        showWindow(ctx, e);
    }
}