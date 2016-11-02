package kz.yzkansarco.offlinedictionary;

import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.v7.app.ActionBar;
import android.support.v7.app.AlertDialog;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.support.v7.widget.Toolbar;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import com.google.android.gms.ads.AdView;

import org.json.JSONException;

import java.util.ArrayList;

import kz.yzkansarco.offlinedictionary.helpers.AdsHelper;
import kz.yzkansarco.offlinedictionary.helpers.DatabaseHelper;
import kz.yzkansarco.offlinedictionary.helpers.ExceptionHelper;
import kz.yzkansarco.offlinedictionary.models.DirectionModel;
import kz.yzkansarco.offlinedictionary.models.HistoryModel;

public final class HistoryActivity extends AppCompatActivity {
    private HistoryAdapter mAdapter;

    public static void show(@NonNull final Context ctx) {
        ctx.startActivity(new Intent(ctx, HistoryActivity.class));
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_history);

        final Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);

        final ActionBar ab = getSupportActionBar();

        if (ab != null) {
            ab.setDisplayHomeAsUpEnabled(true);
        }

        setupRecyclerView();
        setupAds();
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.menu_history, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        try {
            switch (item.getItemId()) {
                case android.R.id.home:
                    finish();
                    return true;
                case R.id.clear:
                    clear();
                    return true;
            }
        } catch (Exception e) {
            ExceptionHelper.getInstance().alertException(HistoryActivity.this, e);
        }

        return super.onOptionsItemSelected(item);
    }

    @Override
    public boolean onPrepareOptionsMenu(Menu menu) {
        final int count = menu.size();

        for (int i = 0; i < count; i++) {
            final MenuItem item = menu.getItem(i);

            if (item.getItemId() == R.id.clear) {
                item.setEnabled(mAdapter.getItemCount() > 0);
            }
        }

        return super.onPrepareOptionsMenu(menu);
    }

    private void setupRecyclerView() {
        mAdapter = new HistoryAdapter();

        final RecyclerView recyclerView = (RecyclerView) findViewById(R.id.recyclerView);

        if (recyclerView != null) {
            recyclerView.setLayoutManager(new LinearLayoutManager(this));
            recyclerView.setHasFixedSize(true);
            recyclerView.setAdapter(mAdapter);
            recyclerView.addItemDecoration(new SimpleDividerItemDecoration(this));
        }
    }

    private void clear() {
        new AlertDialog.Builder(this)
                .setMessage(getString(R.string.question_clear_history))
                .setCancelable(true)
                .setNegativeButton(R.string.no, new DialogInterface.OnClickListener() {
                    public void onClick(DialogInterface dialog, int id) {
                        dialog.cancel();
                    }
                })
                .setPositiveButton(R.string.yes, new DialogInterface.OnClickListener() {
                    public void onClick(DialogInterface dialog, int id) {
                        try {
                            mAdapter.clear();
                            dialog.cancel();
                        } catch (Exception e) {
                            ExceptionHelper.getInstance().alertException(
                                    HistoryActivity.this, e);
                        }
                    }
                }).create().show();
    }

    private final static class ViewHolder extends RecyclerView.ViewHolder {
        public TextView mName;
        public TextView mDirection;
        public View mDelete;

        public ViewHolder(final View itemView) {
            super(itemView);
            mName = (TextView) itemView.findViewById(R.id.name);
            mDirection = (TextView) itemView.findViewById(R.id.direction);
            mDelete = itemView.findViewById(R.id.delete);
        }
    }

    private final class HistoryAdapter extends RecyclerView.Adapter<ViewHolder> {
        private ArrayList<HistoryModel> mDataset;

        public HistoryAdapter() {
            try {
                mDataset = DatabaseHelper.getInstance(HistoryActivity.this).getHistory();
            } catch (JSONException e) {
                ExceptionHelper.getInstance().alertException(HistoryActivity.this, e);
            }
        }

        @Override
        public ViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
            final View v = LayoutInflater.from(parent.getContext()).inflate(
                    R.layout.list_item, parent, false);
            return new ViewHolder(v);
        }

        @Override
        public void onBindViewHolder(final ViewHolder holder, final int position) {
            final HistoryModel item = mDataset.get(position);

            holder.mName.setText(item.getName());
            holder.mDirection.setText(DirectionModel.getDisplayDirection(item.getFromCulture(),
                    item.getToCulture()));
            holder.mDelete.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    try {
                        mDataset.remove(item);
                        DatabaseHelper.getInstance(HistoryActivity.this).setHistory(mDataset);
                        refresh();
                    } catch (Exception e) {
                        ExceptionHelper.getInstance().alertException(HistoryActivity.this, e);
                    }
                }
            });
            holder.itemView.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    try {
                        ArticleActivity.show(HistoryActivity.this, item.getDirectionId(),
                                item.getFromCulture(), item.getToCulture(), item.getWordId(),
                                item.getName(), false);
                    } catch (Exception e) {
                        ExceptionHelper.getInstance().alertException(HistoryActivity.this, e);
                    }
                }
            });
        }

        @Override
        public int getItemCount() {
            return mDataset == null ? 0 : mDataset.size();
        }

        private void refresh() {
            notifyDataSetChanged();
            invalidateOptionsMenu();
        }

        public void clear() throws JSONException {
            if (mDataset != null) {
                mDataset.clear();
                DatabaseHelper.getInstance(HistoryActivity.this).setHistory(mDataset);
            }

            refresh();
        }
    }

    private void setupAds() {
        final AdView adView = (AdView) findViewById(R.id.adView);

        if (adView != null) {
            AdsHelper.getInstance().setupAds(adView);
        }
    }
}