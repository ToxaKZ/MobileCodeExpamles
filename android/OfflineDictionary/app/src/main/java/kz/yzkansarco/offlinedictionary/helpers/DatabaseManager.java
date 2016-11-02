package kz.yzkansarco.offlinedictionary.helpers;

import android.content.Context;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;
import android.support.annotation.Nullable;

import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;

public class DatabaseManager extends SQLiteOpenHelper {
    private final static String DB_NAME = "store.db";
    private final static int DB_VERSION = 9;
    protected final Context mContext;

    public DatabaseManager(final Context context) {
        super(context, DB_NAME, null, DB_VERSION);
        mContext = context;
    }

    @Override
    public void onCreate(SQLiteDatabase db) {
    }

    @Override
    public void onUpgrade(SQLiteDatabase db, int oldVersion, int newVersion) {
    }

    @Nullable
    public String needCopyDatabase() {
        final SQLiteDatabase db = getReadableDatabase();
        try {
            Cursor cursor = db.rawQuery("select count(*) from sqlite_master m where m.type = ? and m.name = ?",
                    new String[]{"table", "settings"});
            try {
                if (cursor.moveToFirst()) {
                    if (cursor.getInt(0) > 0) {
                        final Cursor sc = db.rawQuery("select max(v.version) from settings v", null);
                        try {
                            if (sc.moveToFirst()) {
                                if (DB_VERSION == sc.getInt(0)) {
                                    return null;
                                }
                            }
                        } finally {
                            sc.close();
                        }
                    }
                }
            } finally {
                cursor.close();
            }

            return db.getPath();
        } finally {
            db.close();
        }
    }

    protected void copyDataBase(final String dbPath) throws IOException {
        final InputStream is = mContext.getAssets().open(DB_NAME);
        final FileOutputStream fos = new FileOutputStream(dbPath);
        final byte[] buffer = new byte[1024];
        int length;

        while ((length = is.read(buffer)) > 0) {
            fos.write(buffer, 0, length);
        }

        fos.flush();
        fos.close();
        is.close();
    }
}