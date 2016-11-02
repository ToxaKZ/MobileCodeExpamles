package kz.yzkansarco.offlinedictionary;

import android.content.Context;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.graphics.Paint;
import android.net.Uri;
import android.os.AsyncTask;
import android.os.Build;
import android.os.Bundle;
import android.os.Environment;
import android.support.annotation.Nullable;
import android.support.v7.app.ActionBar;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.webkit.WebView;
import android.widget.Toast;

import com.google.android.gms.ads.AdView;

import org.json.JSONException;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.Serializable;
import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

import kz.yzkansarco.offlinedictionary.helpers.AdsHelper;
import kz.yzkansarco.offlinedictionary.helpers.DatabaseHelper;
import kz.yzkansarco.offlinedictionary.helpers.ExceptionHelper;
import kz.yzkansarco.offlinedictionary.models.ArticleModel;
import kz.yzkansarco.offlinedictionary.models.FavoriteModel;
import kz.yzkansarco.offlinedictionary.models.HistoryModel;

public final class ArticleActivity extends AppCompatActivity {
    private final static String EXTRA_DIRECTION_ID = "directionId";
    private final static String EXTRA_ID = "id";
    private final static String EXTRA_NAME = "name";
    private final static String EXTRA_ARTICLES = "articles";
    private final static String EXTRA_FROM_CULTURE = "fromCulture";
    private final static String EXTRA_TO_CULTURE = "toCulture";

    private WebView mWebView;
    private static int adsCount = 0;

    public static void show(final Context ctx, final int directionId, final String fromCulture,
                            final String toCulture, final int id, final String name,
                            final boolean history) throws Exception {
        final ArrayList<ArticleModel> articles = DatabaseHelper.getInstance(ctx).getArticles(directionId, id);

        if (articles == null || articles.isEmpty()) {
            throw new Exception(ctx.getString(R.string.сould_not_find_translation));
        }

        if (history) {
            final HistoryModel model = new HistoryModel();
            model.setWordId(id);
            model.setName(name);
            model.setDirectionId(directionId);
            model.setFromCulture(fromCulture);
            model.setToCulture(toCulture);

            DatabaseHelper.getInstance(ctx).insertHistory(model);
        }

        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP) {
            WebView.enableSlowWholeDocumentDraw();
        }

        final Intent intent = new Intent(ctx, ArticleActivity.class);
        intent.putExtra(EXTRA_DIRECTION_ID, directionId);
        intent.putExtra(EXTRA_FROM_CULTURE, fromCulture);
        intent.putExtra(EXTRA_TO_CULTURE, toCulture);
        intent.putExtra(EXTRA_ID, id);
        intent.putExtra(EXTRA_NAME, name);
        intent.putExtra(EXTRA_ARTICLES, articles);
        ctx.startActivity(intent);

        if (AdsHelper.getInstance().load(ctx)) {
            if (adsCount > 0 && adsCount % 6 == 0) {
                AdsHelper.getInstance().show();
            }

            adsCount++;
        }
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_article);

        final Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);

        final ActionBar ab = getSupportActionBar();

        if (ab != null) {
            ab.setDisplayHomeAsUpEnabled(true);
        }

        setupAds();
        setupWebView();
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.menu_article, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        try {
            switch (item.getItemId()) {
                case android.R.id.home:
                    finish();
                    return true;
                case R.id.addToFavorite:
                    appendArticle();
                    return true;
                case R.id.share:
                    share();
                    return true;
            }
        } catch (Exception e) {
            ExceptionHelper.getInstance().alertException(this, e);
        }

        return super.onOptionsItemSelected(item);
    }

    private void share() throws IOException {
        mWebView.measure(View.MeasureSpec.makeMeasureSpec(View.MeasureSpec.UNSPECIFIED,
                View.MeasureSpec.UNSPECIFIED), View.MeasureSpec.makeMeasureSpec(0,
                View.MeasureSpec.UNSPECIFIED));
        final Bitmap bm = Bitmap.createBitmap(mWebView.getMeasuredWidth(),
                mWebView.getMeasuredHeight(), Bitmap.Config.ARGB_8888);
        final Canvas canvas = new Canvas(bm);
        canvas.drawBitmap(bm, 0, 0, new Paint());
        mWebView.draw(canvas);

        final AsyncTask<Void, Void, Boolean> task = new AsyncTask<Void, Void, Boolean>() {
            private String error = null;
            private File mFileName;

            @Override
            protected Boolean doInBackground(Void... params) {
                try {
                    WaitActivity.show(ArticleActivity.this);
                    try {
                        mFileName = new File(Environment.getExternalStorageDirectory(),
                                UUID.randomUUID().toString().concat(".png"));

                        final FileOutputStream fos = new FileOutputStream(mFileName);
                        try {
                            bm.compress(Bitmap.CompressFormat.PNG, 100, fos);
                            fos.flush();
                        } finally {
                            fos.close();
                        }

                        return true;
                    } finally {
                        WaitActivity.close();
                    }
                } catch (Exception e) {
                    error = e.getMessage();
                }

                return false;
            }

            @Override
            protected void onPostExecute(final Boolean result) {
                try {
                    if (error != null) {
                        throw new Exception(error);
                    }

                    if (result) {
                        final Intent intent = new Intent(Intent.ACTION_SEND);
                        intent.setType("image/*");
                        intent.putExtra(Intent.EXTRA_STREAM, Uri.fromFile(mFileName));
                        startActivity(Intent.createChooser(intent, getString(R.string.share)));
                    }
                } catch (Exception e) {
                    ExceptionHelper.getInstance().alertException(ArticleActivity.this, e);
                }
            }
        };
        task.execute();
    }

    private void appendArticle() throws JSONException {
        final FavoriteModel model = new FavoriteModel();
        model.setDirectionId(getIntent().getIntExtra(EXTRA_DIRECTION_ID, 0));
        model.setFromCulture(getIntent().getStringExtra(EXTRA_FROM_CULTURE));
        model.setToCulture(getIntent().getStringExtra(EXTRA_TO_CULTURE));
        model.setWordId(getIntent().getIntExtra(EXTRA_ID, 0));
        model.setName(getIntent().getStringExtra(EXTRA_NAME));

        DatabaseHelper.getInstance(this).appendFavorite(model);
        Toast.makeText(this, getString(R.string.added_to_favorites), Toast.LENGTH_SHORT).show();
    }

    private void setupWebView() {
        mWebView = (WebView) findViewById(R.id.webView);
        final StringBuilder sb = new StringBuilder();
        final ArticleModel[] articles = getItems(getIntent().getSerializableExtra(EXTRA_ARTICLES));

        if (articles != null) {
            for (ArticleModel item : articles) {
                if (sb.length() > 0) {
                    sb.append("<br>");
                }

                sb.append(item.getName().trim());
            }
        }

        sb.insert(0, "<!DOCTYPE HTML>" +
                "<html>" +
                "<head>" +
                "<meta charset=\"utf-8\">" +
                "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1, minimum-scale=1, maximum-scale=1, user-scalable=0\">" +
                "</head>" +
                "<body>");
        sb.append("</body></html>");

        mWebView.loadDataWithBaseURL(null, sb.toString().replace("\n", "<br>"), "text/html", "UTF-8", null);
    }

    @Nullable
    public ArticleModel[] getItems(final Serializable obj) {
        final ArrayList<ArticleModel> result = new ArrayList<>();

        if (obj != null && obj instanceof List) {
            int count = ((List<?>) obj).size();

            for (int i = 0; i < count; i++) {
                final Object item = ((List<?>) obj).get(i);

                if (item instanceof ArticleModel) {
                    result.add((ArticleModel) item);
                }
            }

            count = result.size();

            if (count > 0) {
                return result.toArray(new ArticleModel[count]);
            }
        }

        return null;
    }

    private void setupAds() {
        final AdView adView = (AdView) findViewById(R.id.adView);

        if (adView != null) {
            AdsHelper.getInstance().setupAds(adView);
        }
    }
}