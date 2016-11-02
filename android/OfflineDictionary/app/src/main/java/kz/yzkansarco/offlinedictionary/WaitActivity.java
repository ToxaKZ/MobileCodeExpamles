package kz.yzkansarco.offlinedictionary;

import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.widget.TextView;

public final class WaitActivity extends AppCompatActivity {
    private static final String EXTRA_NAME_TEXT = "text";

    private static WaitActivity mInstance = null;
    private static boolean mFinishing = false;

    public static void show(final Context ctx) {
        show(ctx, ctx.getString(R.string.please_wait));
    }

    public static void show(final Context ctx, final String msg) {
        mFinishing = false;
        final Intent intent = new Intent(ctx, WaitActivity.class);
        intent.putExtra(EXTRA_NAME_TEXT, msg);
        ctx.startActivity(intent);
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_wait);

        if (mFinishing) {
            finish();
            mFinishing = false;
            mInstance = null;
            return;
        }

        setFinishOnTouchOutside(false);

        final TextView textView = (TextView) findViewById(R.id.textView);

        if (textView != null) {
            textView.setText(getIntent().getStringExtra(EXTRA_NAME_TEXT));
        }

        mInstance = this;
    }

    @Override
    public void onBackPressed() {
        // Отключаем закрытие окна по нажатию кнопки назад.
    }

    @Override
    public void onDestroy() {
        mInstance = null;
        super.onDestroy();
    }

    public static void close() {
        if (mInstance != null) {
            mInstance.finish();
        } else {
            mFinishing = true;
        }

        mInstance = null;
    }
}