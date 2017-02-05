package xyz.iridiumion.datakitdroid;

import android.content.Context;
import android.hardware.*;
import android.os.*;
import android.support.v7.app.*;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;

import java.io.IOException;
import java.io.OutputStreamWriter;
import java.io.PrintWriter;
import java.net.InetAddress;
import java.net.Socket;
import java.util.ArrayList;
import java.util.Scanner;

public class MainActivity extends AppCompatActivity {

    private EditText ipText;
    private Button connectBtn;

    private ArrayList<DatakitSensor> sensors = new ArrayList<>();

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        ipText = (EditText) findViewById(R.id.ipText);
        connectBtn = (Button) findViewById(R.id.connectBtn);

        connectBtn.setOnClickListener(new View.OnClickListener() {
            public void onClick(View view) {
                // init Sensor(s)
                registerSensor(Sensor.TYPE_LIGHT);
                registerSensor(Sensor.TYPE_ACCELEROMETER);
                registerSensor(Sensor.TYPE_GYROSCOPE);
                registerSensor(Sensor.TYPE_AMBIENT_TEMPERATURE);
                connectBtn.post(new Runnable() {
                    public void run() {
                        connectBtn.setText("Connecting");
                        connectBtn.setEnabled(false);
                    }
                });
            }
        });

        // init SensorManager
        DatakitSensor.sensorManager = (SensorManager) getSystemService(Context.SENSOR_SERVICE);
    }

    protected void setCompText(final TextView view, final String text) {
        view.post(new Runnable() {
            public void run() {
                view.setText(text);
            }
        });
    }

    protected void registerSensor(int type) {
        registerSensor(type, SensorManager.SENSOR_DELAY_GAME);
    }

    protected void registerSensor(int type, int delay) {
        Sensor sensor = DatakitSensor.sensorManager.getDefaultSensor(type);
        if (sensor == null) {
            Log.d("datakit", "Sensor for type " + type + " is null");
        } else {
            sensors.add(new DatakitSensor(sensor, ipText.getText().toString()));
        }
    }


}
