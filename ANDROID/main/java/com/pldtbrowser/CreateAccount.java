package com.pldtbrowser;

import android.app.DatePickerDialog;
import android.app.Dialog;
import android.app.ProgressDialog;
import android.content.Context;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.support.design.widget.TextInputEditText;
import android.util.Log;
import android.view.View;
import android.view.inputmethod.InputMethodManager;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.DatePicker;
import android.widget.Spinner;

import java.util.Calendar;

public class CreateAccount extends AppCompatActivity {

    private TextInputEditText firstname, middlename, lastname, line1, line2, city, state, zipcode, countrycode;
    private Button btnNext;
    private Spinner sex;
    private static  TextInputEditText birthday;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.register);

        SharedPreferences myPrefs = getSharedPreferences("myPrefs", MODE_PRIVATE);

        sex = (Spinner) findViewById(R.id.reg_sex);
        firstname = (TextInputEditText) findViewById(R.id.reg_firstname);
        middlename = (TextInputEditText) findViewById(R.id.reg_middlename);
        lastname = (TextInputEditText) findViewById(R.id.reg_lastname);
        birthday = (TextInputEditText) findViewById(R.id.reg_birthday);
        line1 = (TextInputEditText) findViewById(R.id.reg_line1);
        line2 = (TextInputEditText) findViewById(R.id.reg_line2);
        city = (TextInputEditText) findViewById(R.id.reg_city);
        state = (TextInputEditText) findViewById(R.id.reg_state);
        zipcode = (TextInputEditText) findViewById(R.id.reg_zipcode);
        countrycode = (TextInputEditText) findViewById(R.id.reg_countrycode);
        btnNext = (Button) findViewById(R.id.reg_next);

        final android.support.v4.app.FragmentManager fm = getSupportFragmentManager();
        birthday.setOnFocusChangeListener(new View.OnFocusChangeListener() {
            @Override
            public void onFocusChange(View view, boolean b) {
                InputMethodManager imm = (InputMethodManager)getSystemService(Context.INPUT_METHOD_SERVICE);
                imm.hideSoftInputFromWindow(view.getWindowToken(), 0);
                android.support.v4.app.DialogFragment newFragment = new DatePickerFragment();
                newFragment.show(fm, "datePicker");
            }
        });

        ArrayAdapter<CharSequence> adapter = ArrayAdapter.createFromResource(this,
                R.array.gender_array, android.R.layout.simple_spinner_item);

        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);

        sex.setAdapter(adapter);

        btnNext.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {

                final ProgressDialog dialog = ProgressDialog.show(CreateAccount.this, "",
                        "Loading. Please wait...", true);

                HTTPRequest req = new HTTPRequest();

                req.setHTTPRequestListener(new HTTPRequestListener() {
                    @Override
                    public void HTTPRequestResult(String s) {
                        Log.v("asdas", s);
                        dialog.cancel();
                    }
                });

                req.execute("http://e8bff582.ngrok.io/PLDT88Hackathon/api/PayMaya/CreateCustomer?firstName="+firstname.getText().toString()+"&middleName="+middlename.getText().toString()+"&lastName="+lastname.getText().toString()+"&birthday=asd&sex=wqe&line1=wqewq&line2=ewq&city=ewq&state=ewqew&zipCode=e&countryCode=wqewq&phone=ewqe&email=wqewqewq");

            }
        });

    }

    public static class DatePickerFragment extends android.support.v4.app.DialogFragment
            implements DatePickerDialog.OnDateSetListener {

        @Override
        public Dialog onCreateDialog(Bundle savedInstanceState) {
            // Use the current date as the default date in the picker
            final Calendar c = Calendar.getInstance();
            int year = c.get(Calendar.YEAR);
            int month = c.get(Calendar.MONTH);
            int day = c.get(Calendar.DAY_OF_MONTH);

            // Create a new instance of DatePickerDialog and return it
            return new DatePickerDialog(getActivity(), this, year, month, day);
        }

        public void onDateSet(DatePicker view, int year, int month, int day) {
            birthday.setText(year + " - " + month + " - " + day);
        }
    }

}
