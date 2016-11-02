package kz.yzkansarco.offlinedictionary;

import android.app.Activity;
import android.content.ActivityNotFoundException;
import android.content.Context;
import android.content.Intent;
import android.content.pm.ResolveInfo;
import android.net.Uri;
import android.os.AsyncTask;
import android.os.Build;
import android.os.Bundle;
import android.speech.RecognizerIntent;
import android.support.design.widget.NavigationView;
import android.support.v4.content.ContextCompat;
import android.support.v4.view.GravityCompat;
import android.support.v4.widget.DrawerLayout;
import android.support.v7.app.ActionBarDrawerToggle;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.text.Editable;
import android.text.TextUtils;
import android.text.TextWatcher;
import android.view.LayoutInflater;
import android.view.MenuItem;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup;
import android.view.inputmethod.InputMethodManager;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.BaseExpandableListAdapter;
import android.widget.EditText;
import android.widget.ExpandableListView;
import android.widget.ImageButton;
import android.widget.Spinner;
import android.widget.TextView;

import com.google.android.gms.ads.AdView;

import java.util.ArrayList;
import java.util.List;

import kz.yzkansarco.offlinedictionary.helpers.AdsHelper;
import kz.yzkansarco.offlinedictionary.helpers.DatabaseHelper;
import kz.yzkansarco.offlinedictionary.helpers.DatabaseManager;
import kz.yzkansarco.offlinedictionary.helpers.ExceptionHelper;
import kz.yzkansarco.offlinedictionary.helpers.PreferencesHelper;
import kz.yzkansarco.offlinedictionary.models.ArticleModel;
import kz.yzkansarco.offlinedictionary.models.DirectionModel;
import kz.yzkansarco.offlinedictionary.models.WordModel;

public final class HomeActivity extends AppCompatActivity
        implements NavigationView.OnNavigationItemSelectedListener {
    private final static int REQUEST_SPEECH_TO_TEXT = 1;
    private final static String APP_STORE_URL = "http://play.google.com/store/apps/details?id=";

    private Spinner mDirections;
    private DrawerLayout mDrawerLayout;
    private ActionBarDrawerToggle mActionBarDrawerToggle;
    private EditText mSearch;
    private WordAdapter mWordAdapter;
    private ImageButton mMicAndClear;
    private InputMethodManager mIMM;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_home);

        final Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);

        mIMM = (InputMethodManager) getSystemService(Context.INPUT_METHOD_SERVICE);

        setupDrawerLayout(toolbar);
        setupNavigationView();
        setupExpandableListView();
        setupSearch();
        setupMicAndClear();
        updateButtonState();
        setupAds();

        setupDatabase();
    }

    @Override
    public void onBackPressed() {
        if (mDrawerLayout.isDrawerOpen(GravityCompat.START)) {
            mDrawerLayout.closeDrawer(GravityCompat.START);
        } else {
            super.onBackPressed();
        }
    }

    @Override
    public boolean onNavigationItemSelected(final MenuItem item) {
        try {
            switch (item.getItemId()) {
                case R.id.navHistory:
                    HistoryActivity.show(this);
                    break;
                case R.id.navFavorites:
                    FavoritesActivity.show(this);
                    break;
                case R.id.navRate:
                    rate();
                    break;
                case R.id.navShare:
                    share();
                    break;
            }

            mDrawerLayout.closeDrawer(GravityCompat.START);
        } catch (Exception e) {
            ExceptionHelper.getInstance().alertException(this, e);
        }

        return true;
    }

    @Override
    public void onDestroy() {
        try {
            if (mDrawerLayout != null && mActionBarDrawerToggle != null) {
                mDrawerLayout.removeDrawerListener(mActionBarDrawerToggle);
            }
        } catch (Exception e) {
            e.printStackTrace();
        }

        super.onDestroy();
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        try {
            if (resultCode == Activity.RESULT_OK) {
                switch (requestCode) {
                    case REQUEST_SPEECH_TO_TEXT: {
                        if (data != null) {
                            final ArrayList<String> words = data.getStringArrayListExtra(
                                    RecognizerIntent.EXTRA_RESULTS);

                            if (words != null && words.size() > 0) {
                                mSearch.setText(words.get(0));
                                mSearch.setSelection(mSearch.length());
                            }
                        }

                        break;
                    }
                }
            }
        } catch (Exception e) {
            ExceptionHelper.getInstance().alertException(this, e);
        }
    }

    private void setupDrawerLayout(final Toolbar toolbar) {
        mDrawerLayout = (DrawerLayout) findViewById(R.id.drawer_layout);
        mActionBarDrawerToggle = new ActionBarDrawerToggle(this, mDrawerLayout, toolbar,
                R.string.navigation_drawer_open, R.string.navigation_drawer_close);
        mDrawerLayout.addDrawerListener(mActionBarDrawerToggle);
        mActionBarDrawerToggle.syncState();
    }

    private void setupDirections() {
        mDirections = (Spinner) findViewById(R.id.directions);

        final ArrayAdapter<DirectionModel> adapter = new ArrayAdapter<>(this,
                R.layout.directions_spinner_item, DatabaseHelper.getInstance(this).getDirections());
        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);

        mDirections.setAdapter(adapter);
        mDirections.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
                try {
                    final DirectionModel model = (DirectionModel) parent.getItemAtPosition(position);

                    if (model != null) {
                        PreferencesHelper.getInstance().setDirectionId(HomeActivity.this,
                                model.getId());
                    }

                    mWordAdapter.refresh();
                } catch (Exception e) {
                    ExceptionHelper.getInstance().alertException(HomeActivity.this, e);
                }
            }

            @Override
            public void onNothingSelected(AdapterView<?> parent) {
            }
        });

        int id = PreferencesHelper.getInstance().getDirectionId(this);
        int count = adapter.getCount();

        for (int i = 0; i < count; i++) {
            final DirectionModel model = adapter.getItem(i);

            if (model != null && model.getId() == id) {
                mDirections.setSelection(i);
                break;
            }
        }
    }

    private void setupNavigationView() {
        final NavigationView navigationView = (NavigationView) findViewById(R.id.nav_view);

        if (navigationView != null) {
            navigationView.setNavigationItemSelectedListener(this);
        }
    }

    private void setupSearch() {
        mSearch = (EditText) findViewById(R.id.search);
        mSearch.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {
            }

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {
            }

            @Override
            public void afterTextChanged(Editable s) {
                try {
                    mWordAdapter.refresh();
                    updateButtonState();
                } catch (Exception e) {
                    ExceptionHelper.getInstance().alertException(HomeActivity.this, e);
                }
            }
        });
    }

    private void setupMicAndClear() {
        mMicAndClear = (ImageButton) findViewById(R.id.micAndClear);
        mMicAndClear.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(final View v) {
                try {
                    if (TextUtils.isEmpty(mSearch.getText()) && mDirections.getSelectedItem() != null) {
                        final Intent intent = new Intent(RecognizerIntent.ACTION_RECOGNIZE_SPEECH);
                        final List<ResolveInfo> activities =
                                getPackageManager().queryIntentActivities(intent, 0);

                        if (activities.size() > 0) {
                            intent.putExtra(RecognizerIntent.EXTRA_LANGUAGE_MODEL,
                                    RecognizerIntent.LANGUAGE_MODEL_WEB_SEARCH);
                            intent.putExtra(RecognizerIntent.EXTRA_LANGUAGE,
                                    ((DirectionModel) mDirections.getSelectedItem()).getFromLocale());
                            startActivityForResult(intent, REQUEST_SPEECH_TO_TEXT);
                        }
                    } else {
                        mSearch.setText(null);
                    }
                } catch (Exception e) {
                    ExceptionHelper.getInstance().alertException(HomeActivity.this, e);
                }
            }
        });
    }

    private void updateButtonState() {
        final int resId = TextUtils.isEmpty(mSearch.getText()) ? R.drawable.ic_mic : R.drawable.ic_clear;
        mMicAndClear.setImageDrawable(ContextCompat.getDrawable(HomeActivity.this, resId));
    }

    private void setupExpandableListView() {
        mWordAdapter = new WordAdapter();

        final ExpandableListView listView = (ExpandableListView) findViewById(R.id.expandableListView);

        if (listView != null) {
            listView.setOnGroupClickListener(new ExpandableListView.OnGroupClickListener() {
                @Override
                public boolean onGroupClick(ExpandableListView parent, View v,
                                            int groupPosition, long id) {
                    try {
                        final DirectionModel direction = (DirectionModel) mDirections.getSelectedItem();
                        final WordModel item = (WordModel) mWordAdapter.getGroup(groupPosition);

                        if (direction != null && item.getChildrens() == null) {
                            final ArrayList<ArticleModel> articles = DatabaseHelper.getInstance(
                                    HomeActivity.this).getArticles(direction.getId(), item.getId());

                            if (articles != null && !articles.isEmpty()) {
                                item.setChildrens(articles.toArray(new ArticleModel[articles.size()]));
                            }
                        }
                    } catch (Exception e) {
                        ExceptionHelper.getInstance().alertException(HomeActivity.this, e);
                    }

                    return false;
                }
            });
            listView.setAdapter(mWordAdapter);
        }
    }

    @SuppressWarnings("deprecation")
    private void rate() {
        int flags = Intent.FLAG_ACTIVITY_NO_HISTORY | Intent.FLAG_ACTIVITY_MULTIPLE_TASK;

        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP) {
            flags |= Intent.FLAG_ACTIVITY_NEW_DOCUMENT;
        } else {
            flags |= Intent.FLAG_ACTIVITY_CLEAR_WHEN_TASK_RESET;
        }

        final Intent intent = new Intent(Intent.ACTION_VIEW, Uri.parse("market://details?id="
                + getPackageName()));
        intent.addFlags(flags);
        try {
            startActivity(intent);
        } catch (ActivityNotFoundException e) {
            startActivity(new Intent(Intent.ACTION_VIEW,
                    Uri.parse(APP_STORE_URL.concat(getPackageName()))));
        }
    }

    @Override
    public boolean dispatchTouchEvent(MotionEvent ev) {
        final View focusedView = getCurrentFocus();

        if (focusedView != null && focusedView instanceof EditText) {
            final int scrCoords[] = new int[2];
            focusedView.getLocationOnScreen(scrCoords);
            final float x = ev.getRawX() + focusedView.getLeft() - scrCoords[0];
            final float y = ev.getRawY() + focusedView.getTop() - scrCoords[1];

            if (x < focusedView.getLeft() || x > focusedView.getRight()
                    || y < focusedView.getTop() || y > focusedView.getBottom()) {
                mIMM.hideSoftInputFromWindow(getCurrentFocus().getWindowToken(),
                        InputMethodManager.HIDE_NOT_ALWAYS);
            }
        }

        return super.dispatchTouchEvent(ev);
    }

    private void setupAds() {
        final AdView adView = (AdView) findViewById(R.id.adView);

        if (adView != null) {
            AdsHelper.getInstance().setupAds(adView);
        }
    }

    private void share() {
        final Intent intent = new Intent(Intent.ACTION_SEND);
        intent.setType("text/plain");
        intent.putExtra(Intent.EXTRA_TEXT, getString(R.string.app_name)
                .concat(": ")
                .concat(APP_STORE_URL)
                .concat(getPackageName()));
        startActivity(Intent.createChooser(intent, getString(R.string.share)));
    }

    private void setupDatabase() {
        if (TextUtils.isEmpty(new DatabaseManager(this).needCopyDatabase())) {
            DatabaseHelper.getInstance(this);
            afterDatabaseInit();
        } else {
            final AsyncTask<Void, Void, Boolean> task = new AsyncTask<Void, Void, Boolean>() {
                private String error = null;

                @Override
                protected Boolean doInBackground(Void... params) {
                    try {
                        WaitActivity.show(HomeActivity.this, getString(R.string.database_init));
                        try {
                            DatabaseHelper.getInstance(HomeActivity.this);
                        } finally {
                            WaitActivity.close();
                        }

                        return true;
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
                            afterDatabaseInit();
                        }
                    } catch (Exception e) {
                        ExceptionHelper.getInstance().alertException(HomeActivity.this, e);
                    }
                }
            };
            task.execute();
        }
    }

    private void afterDatabaseInit() {
        setupDirections();
    }

    private final class WordAdapter extends BaseExpandableListAdapter {
        private WordModel[] mDataset;

        public WordAdapter() {
            this.mDataset = null;
        }

        @Override
        public Object getChild(int groupPosition, int childPosition) {
            return mDataset[groupPosition].getChildrens()[childPosition];
        }

        @Override
        public long getChildId(int groupPosition, int childPosition) {
            return mDataset[groupPosition].getChildrens()[childPosition].getId();
        }

        @Override
        public View getChildView(int groupPosition, final int childPosition, boolean isLastChild,
                                 View convertView, ViewGroup parent) {
            if (convertView == null) {
                convertView = LayoutInflater.from(HomeActivity.this).inflate(
                        R.layout.word_list_article_item, parent, false);
            }

            final ArticleModel item = (ArticleModel) getChild(groupPosition, childPosition);
            final TextView label = (TextView) convertView.findViewById(R.id.article);
            label.setText(item.getName());

            return convertView;
        }

        @Override
        public int getChildrenCount(int groupPosition) {
            final WordModel item = mDataset[groupPosition];
            return item.getChildrens() != null ? item.getChildrens().length : 0;
        }

        @Override
        public Object getGroup(int groupPosition) {
            return mDataset[groupPosition];
        }

        @Override
        public int getGroupCount() {
            return mDataset != null ? mDataset.length : 0;
        }

        @Override
        public long getGroupId(int groupPosition) {
            return mDataset[groupPosition].getId();
        }

        @Override
        public View getGroupView(int groupPosition, boolean isExpanded, View convertView, ViewGroup parent) {
            if (convertView == null) {
                convertView = LayoutInflater.from(HomeActivity.this).inflate(
                        R.layout.word_list_item, parent, false);
            }

            final WordModel item = (WordModel) getGroup(groupPosition);
            final TextView label = (TextView) convertView.findViewById(R.id.name);
            label.setText(item.getName());

            final View button = convertView.findViewById(R.id.more);
            button.setFocusable(false);
            button.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    try {
                        final DirectionModel model = (DirectionModel) mDirections.getSelectedItem();

                        if (model != null) {
                            ArticleActivity.show(HomeActivity.this, model.getId(),
                                    model.getFromCulture(), model.getToCulture(), item.getId(),
                                    item.getName(), true);
                        }
                    } catch (Exception e) {
                        ExceptionHelper.getInstance().alertException(HomeActivity.this, e);
                    }
                }
            });

            return convertView;
        }

        @Override
        public boolean hasStableIds() {
            return false;
        }

        @Override
        public boolean isChildSelectable(int groupPosition, int childPosition) {
            return true;
        }

        public void refresh() {
            final DirectionModel direction = (DirectionModel) mDirections.getSelectedItem();
            final String text = DatabaseHelper.getInstance(HomeActivity.this)
                    .prepareTextForSearch(mSearch.getText());

            if (direction != null && !TextUtils.isEmpty(text)) {
                mDataset = DatabaseHelper.getInstance(HomeActivity.this)
                        .getWords(direction.getId(), text);
            } else {
                mDataset = null;
            }

            notifyDataSetChanged();
        }
    }
}