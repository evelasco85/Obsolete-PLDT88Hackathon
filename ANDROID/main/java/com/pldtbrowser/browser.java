package com.pldtbrowser;

import android.content.Context;
import android.content.Intent;
import android.os.AsyncTask;
import android.support.design.widget.NavigationView;
import android.support.design.widget.Snackbar;
import android.support.v4.view.GravityCompat;
import android.support.v4.widget.DrawerLayout;
import android.support.v7.app.ActionBarDrawerToggle;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.support.v7.widget.Toolbar;
import android.util.Log;
import android.view.KeyEvent;
import android.view.MenuItem;
import android.view.View;
import android.view.inputmethod.InputMethodManager;
import android.webkit.WebChromeClient;
import android.webkit.WebSettings;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ProgressBar;
import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;

public class browser extends AppCompatActivity {

    private DrawerLayout mDrawer;
    private ActionBarDrawerToggle mToggle;
    private Toolbar mToolbar;
    private NavigationView navView;
    private Button btnMenu;

    private String defaultURL;

    private WebView wv;
    private EditText urlBar;
    private ProgressBar webProgressBar;

    @Override
    protected void onCreate(Bundle savedInstanceState) {

        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_browser);

        navView = (NavigationView) findViewById(R.id.navigationView);

        mToolbar = (Toolbar) findViewById(R.id.nav_action);
        setSupportActionBar(mToolbar);

        mDrawer = (DrawerLayout) findViewById(R.id.drawerLayout);
        mToggle = new ActionBarDrawerToggle(this, mDrawer, R.string.open, R.string.close);

        mDrawer.addDrawerListener(mToggle);
        mToggle.syncState();

        defaultURL = "https://www.google.com/";
        webProgressBar = (ProgressBar) findViewById(R.id.webProgress);

        wv = (WebView) findViewById(R.id.webView);
        wv.setWebViewClient(new WebViewClient());
        WebSettings ws = wv.getSettings();
        ws.setJavaScriptEnabled(true);

        btnMenu = (Button) findViewById(R.id.btnMenu);
        btnMenu.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                mDrawer.openDrawer(GravityCompat.START);
            }
        });

        navView.setNavigationItemSelectedListener(new NavigationView.OnNavigationItemSelectedListener() {
            @Override
            public boolean onNavigationItemSelected( MenuItem item) {

                switch(item.getItemId()) {
                    case R.id.FreeSMS:

                        break;
                    case R.id.Payment:
                        Intent intent = new Intent(browser.this, CreateAccount.class);
                        mDrawer.closeDrawers();
                        startActivity(intent);

                    break;
                }

                return false;
            }
        });

        wv.setWebChromeClient(new WebChromeClient(){

            @Override
            public void onProgressChanged(WebView view, int newProgress) {

                if(newProgress < 100 && webProgressBar.getVisibility() == ProgressBar.GONE){
                    webProgressBar.setVisibility(ProgressBar.VISIBLE);
                }

                webProgressBar.setProgress(newProgress);
                if(newProgress == 100) {
                    webProgressBar.setVisibility(ProgressBar.GONE);
                }

            }
        });

        wv.loadUrl(defaultURL);
        urlBar = (EditText) findViewById(R.id.urlBar);

        urlBar.setOnKeyListener(new View.OnKeyListener() {
            @Override
            public boolean onKey(View view, int keyCode, KeyEvent event) {

                if ((event.getAction() == KeyEvent.ACTION_DOWN) &&
                        (keyCode == KeyEvent.KEYCODE_ENTER)) {

                    InputMethodManager imm = (InputMethodManager)getSystemService(Context.INPUT_METHOD_SERVICE);
                    imm.hideSoftInputFromWindow(view.getWindowToken(), 0);

                    String url = urlBar.getText().toString();

                    if (!url.startsWith("www.") && !url.startsWith("http://")) {
                        url = "www." + url;
                    }

                    HTTPRequest req = new HTTPRequest();

                    req.setHTTPRequestListener(new HTTPRequestListener() {
                        @Override
                        public void HTTPRequestResult(String s) {
                            showGeneratedTest(s);
                        }
                    });

                    req.execute("http://e8bff582.ngrok.io/PLDT88Hackathon/api/Latency/Get?urlOrIp=" + url);

                    if (!url.startsWith("http://")) {
                        url = "http://" + url;
                    }

                    urlBar.setText(url);
                    wv.loadUrl(url);

                    return true;

                }

                return false;
            }
        });

    }

    public void showGeneratedTest(String s){

        int res = R.string.bad;

        if(s.contains("Good"))
            res = R.string.good;

        else if(s.contains("Average")) {
            res = R.string.average;
        }

        Snackbar.make(findViewById(android.R.id.content), res, Snackbar.LENGTH_SHORT).show();

    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {

        if(mToggle.onOptionsItemSelected(item)) {
            return true;
        }

        return super.onOptionsItemSelected(item);

    }

    public static String readStream(InputStream in) {
        BufferedReader reader = null;
        StringBuffer response = new StringBuffer();
        try {
            reader = new BufferedReader(new InputStreamReader(in));
            String line = "";
            while ((line = reader.readLine()) != null) {
                response.append(line);
            }
        } catch (IOException e) {
            e.printStackTrace();
        } finally {
            if (reader != null) {
                try {
                    reader.close();
                } catch (IOException e) {
                    e.printStackTrace();
                }
            }
        }
        return response.toString();
    }

    public class GetJSON extends AsyncTask<String, Void, String> {

        String server_response;

        @Override
        protected String doInBackground(String... strings) {
            URL url;

            HttpURLConnection urlConnection = null;

            try {
                url = new URL(strings[0]);
                urlConnection = (HttpURLConnection) url.openConnection();

                int responseCode = urlConnection.getResponseCode();

                if(responseCode == HttpURLConnection.HTTP_OK){
                    server_response = readStream(urlConnection.getInputStream());
                    Log.v("Try", server_response);
                }

            } catch (MalformedURLException e) {
                e.printStackTrace();
            } catch (IOException e) {
                e.printStackTrace();
            }

            return null;


        }

        public String getServerResponse() {
            return server_response;
        }

        @Override
        protected void onPostExecute(String s) {
            super.onPostExecute(s);
        }

    }


}

