package kz.yzkansarco.offlinedictionary.helpers;

import android.content.Context;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.text.Editable;
import android.text.TextUtils;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.BufferedReader;
import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.nio.charset.Charset;
import java.util.ArrayList;
import java.util.List;
import java.util.zip.GZIPInputStream;

import kz.yzkansarco.offlinedictionary.models.ArticleModel;
import kz.yzkansarco.offlinedictionary.models.DirectionModel;
import kz.yzkansarco.offlinedictionary.models.FavoriteModel;
import kz.yzkansarco.offlinedictionary.models.HistoryModel;
import kz.yzkansarco.offlinedictionary.models.WordModel;

public final class DatabaseHelper extends DatabaseManager {
    private final static String AVAILABLE_LETTERS = "абвгдёежзийклмнопрстуфхцчшщъыьэюяәіңғүұқөһqwertyuiopasdfghjklzxcvbnmäößü";
    private final static int MAX_RESULT_SET = 100;
    private final static String KEY_NAME = "name";
    private final static String KEY_WORD_ID = "wordId";
    private final static String KEY_FROM_CULTURE = "fromCulture";
    private final static String KEY_TO_CULTURE = "toCulture";
    private final static String KEY_DIRECTION_ID = "directionId";

    private static DatabaseHelper mHelper = null;
    private final ArrayList<String> mAvailableTableNames;

    public static DatabaseHelper getInstance(final Context context) {
        if (mHelper == null) {
            mHelper = new DatabaseHelper(context);

            final String dbPath = mHelper.needCopyDatabase();

            if (!TextUtils.isEmpty(dbPath)) {
                try {
                    mHelper.copyDataBase(dbPath);
                } catch (IOException e) {
                    e.printStackTrace();
                }
            }

            mHelper.setAvailableTableNames();
        }

        return mHelper;
    }

    public DatabaseHelper(final Context context) {
        super(context);
        mAvailableTableNames = new ArrayList<>();
    }

    private String getTableName(final int directionId, final String text) {
        String result = "words_" + Integer.toString(directionId);

        if (text.length() > 0) {
            final int index = AVAILABLE_LETTERS.indexOf(text.charAt(0));

            if (index != -1) {
                result = TextUtils.concat(result, "_", Integer.toString(index)).toString();
            }
        }

        return result;
    }

    private void setAvailableTableNames() {
        final SQLiteDatabase db = getReadableDatabase();
        try {
            final Cursor cursor = db.rawQuery("select m.name from sqlite_master m where m.type = ?",
                    new String[]{"table"});
            try {
                if (cursor.moveToFirst()) {
                    do {
                        mAvailableTableNames.add(cursor.getString(0));
                    } while (cursor.moveToNext());
                }
            } finally {
                cursor.close();
            }
        } finally {
            db.close();
        }
    }

    @Nullable
    public WordModel[] getWords(final int directionId, final String text) {
        final String tableName = getTableName(directionId, text);

        if (mAvailableTableNames.contains(tableName)) {
            final SQLiteDatabase db = getReadableDatabase();
            try {
                final Cursor cursor = db.rawQuery(String.format("select w.word_id, w.name from %s w " +
                                "where w.name like ? order by w.name limit %s",
                        tableName, MAX_RESULT_SET),
                        new String[]{text.concat("%")});
                try {
                    if (cursor.moveToFirst()) {
                        final ArrayList<WordModel> rs = new ArrayList<>();

                        do {
                            final WordModel model = new WordModel();
                            model.setId(cursor.getInt(0));
                            model.setName(cursor.getString(1));
                            rs.add(model);
                        } while (cursor.moveToNext());

                        return rs.toArray(new WordModel[rs.size()]);
                    }
                } finally {
                    cursor.close();
                }
            } finally {
                db.close();
            }
        }

        return null;
    }

    @Nullable
    public ArrayList<ArticleModel> getArticles(final int directionId, final int wordId) throws IOException {
        final SQLiteDatabase db = getReadableDatabase();
        try {
            final Cursor cursor = db.rawQuery("select a.id, a.name from words_links wl " +
                            "join articles a on a.id = wl.article_id " +
                            "join dictionaries d on d.id = wl.dic_id " +
                            "where wl.word_id = ? and d.direction_id = ?",
                    new String[]{Integer.toString(wordId), Integer.toString(directionId)});
            try {
                if (cursor.moveToFirst()) {
                    final ArrayList<ArticleModel> rs = new ArrayList<>();

                    do {
                        final ArticleModel model = new ArticleModel();
                        model.setId(cursor.getInt(0));
                        model.setName(decompress(cursor.getBlob(1)));
                        rs.add(model);
                    } while (cursor.moveToNext());

                    return rs;
                }
            } finally {
                cursor.close();
            }
        } finally {
            db.close();
        }

        return null;
    }

    @NonNull
    public ArrayList<HistoryModel> getHistory() throws JSONException {
        final ArrayList<HistoryModel> result = new ArrayList<>();
        final String jsonString = PreferencesHelper.getInstance().getHistory(mContext);

        if (!TextUtils.isEmpty(jsonString)) {
            final JSONArray jsonArray = new JSONArray(jsonString);
            final int count = Math.min(jsonArray.length(), MAX_RESULT_SET);

            for (int i = 0; i < count; i++) {
                final JSONObject jsonObject = jsonArray.getJSONObject(i);
                final HistoryModel model = new HistoryModel();
                model.setWordId(jsonObject.getInt(KEY_WORD_ID));
                model.setName(jsonObject.getString(KEY_NAME));
                model.setDirectionId(jsonObject.getInt(KEY_DIRECTION_ID));
                model.setFromCulture(jsonObject.getString(KEY_FROM_CULTURE));
                model.setToCulture(jsonObject.getString(KEY_TO_CULTURE));
                result.add(model);
            }
        }

        return result;
    }

    public void setHistory(@NonNull final ArrayList<HistoryModel> history) throws JSONException {
        final JSONArray jsonArray = new JSONArray();

        for (final HistoryModel child : history) {
            final JSONObject jsonObject = new JSONObject();
            jsonObject.put(KEY_WORD_ID, child.getWordId());
            jsonObject.put(KEY_NAME, child.getName());
            jsonObject.put(KEY_DIRECTION_ID, child.getDirectionId());
            jsonObject.put(KEY_FROM_CULTURE, child.getFromCulture());
            jsonObject.put(KEY_TO_CULTURE, child.getToCulture());
            jsonArray.put(jsonObject);
        }

        PreferencesHelper.getInstance().setHistory(mContext, jsonArray.toString());
    }

    public void insertHistory(@NonNull final HistoryModel history) throws JSONException {
        final ArrayList<HistoryModel> dataSet = getHistory();

        for (int i = dataSet.size() - 1; i >= 0; i--) {
            if (dataSet.get(i).getWordId() == history.getWordId()) {
                dataSet.remove(i);
            }
        }

        dataSet.add(0, history);
        setHistory(dataSet);
    }

    @NonNull
    public ArrayList<FavoriteModel> getFavorites() throws JSONException {
        final ArrayList<FavoriteModel> result = new ArrayList<>();
        final String jsonString = PreferencesHelper.getInstance().getFavorites(mContext);

        if (!TextUtils.isEmpty(jsonString)) {
            final JSONArray jsonArray = new JSONArray(jsonString);
            final int count = Math.min(jsonArray.length(), MAX_RESULT_SET);

            for (int i = 0; i < count; i++) {
                final JSONObject jsonObject = jsonArray.getJSONObject(i);
                final FavoriteModel model = new FavoriteModel();
                model.setWordId(jsonObject.getInt(KEY_WORD_ID));
                model.setName(jsonObject.getString(KEY_NAME));
                model.setDirectionId(jsonObject.getInt(KEY_DIRECTION_ID));
                model.setFromCulture(jsonObject.getString(KEY_FROM_CULTURE));
                model.setToCulture(jsonObject.getString(KEY_TO_CULTURE));
                result.add(model);
            }
        }

        return result;
    }

    public void setFavorites(@NonNull final ArrayList<FavoriteModel> favorites) throws JSONException {
        final JSONArray jsonArray = new JSONArray();

        for (final FavoriteModel child : favorites) {
            final JSONObject jsonObject = new JSONObject();
            jsonObject.put(KEY_WORD_ID, child.getWordId());
            jsonObject.put(KEY_NAME, child.getName());
            jsonObject.put(KEY_DIRECTION_ID, child.getDirectionId());
            jsonObject.put(KEY_FROM_CULTURE, child.getFromCulture());
            jsonObject.put(KEY_TO_CULTURE, child.getToCulture());
            jsonArray.put(jsonObject);
        }

        PreferencesHelper.getInstance().setFavorites(mContext, jsonArray.toString());
    }

    public void appendFavorite(@NonNull final FavoriteModel favorite) throws JSONException {
        final ArrayList<FavoriteModel> dataSet = getFavorites();

        for (int i = dataSet.size() - 1; i >= 0; i--) {
            if (dataSet.get(i).getWordId() == favorite.getWordId()) {
                dataSet.remove(i);
            }
        }

        dataSet.add(favorite);
        setFavorites(dataSet);
    }

    @Nullable
    public String prepareTextForSearch(final Editable text) {
        if (TextUtils.isEmpty(text)) {
            return null;
        }

        return text.toString().toLowerCase().trim().replaceAll("\\_|\\?|\\%/g", "");
    }

    public String decompress(final byte[] data) throws IOException {
        final ByteArrayInputStream bais = new ByteArrayInputStream(data);
        final GZIPInputStream zip = new GZIPInputStream(bais);
        final InputStreamReader isr = new InputStreamReader(zip, Charset.forName("UTF-8"));
        final BufferedReader br = new BufferedReader(isr);
        final StringBuilder sb = new StringBuilder();

        String line;

        while ((line = br.readLine()) != null) {
            sb.append(line);
            sb.append("\n");
        }

        br.close();
        isr.close();
        zip.close();
        bais.close();
        return sb.toString();
    }

    @NonNull
    public DirectionModel[] getDirections() {
        final List<DirectionModel> result = new ArrayList<>();
        final SQLiteDatabase db = getReadableDatabase();
        try {
            final Cursor cursor = db.rawQuery("select d.id, fl.culture, tl.culture, fl.locale from directions d " +
                    "join languages fl on fl.id = d.from_lang " +
                    "join languages tl on tl.id = d.to_lang order by fl.culture, tl.culture", null);
            try {
                if (cursor.moveToFirst()) {
                    do {
                        final DirectionModel model = new DirectionModel();
                        model.setId(cursor.getInt(0));
                        model.setFromCulture(cursor.getString(1));
                        model.setToCulture(cursor.getString(2));
                        model.setFromLocale(cursor.getString(3));

                        result.add(model);
                    } while (cursor.moveToNext());
                }
            } finally {
                cursor.close();
            }
        } finally {
            db.close();
        }

        return result.toArray(new DirectionModel[result.size()]);
    }
}