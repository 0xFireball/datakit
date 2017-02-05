package xyz.iridiumion.datakitdroid;

import android.content.Context;
import android.hardware.*;
import android.os.*;
import android.support.v7.app.*;
import android.util.Log;

public class MainActivity extends AppCompatActivity implements SensorEventListener {

    private SensorManager sensorManager;
//    private Sensor senAmbientLight, senAccelerometer;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        // init SensorManager
        sensorManager = (SensorManager)getSystemService(Context.SENSOR_SERVICE);
        // init Sensor(s)
        registerSensor(Sensor.TYPE_LIGHT);
        registerSensor(Sensor.TYPE_ACCELEROMETER);
        registerSensor(Sensor.TYPE_GYROSCOPE);
        registerSensor(Sensor.TYPE_AMBIENT_TEMPERATURE);
    }

    protected void registerSensor(int type) {
        registerSensor(type, SensorManager.SENSOR_DELAY_GAME);
    }
    protected void registerSensor(int type, int delay) {
        Sensor sensor = sensorManager.getDefaultSensor(type);
        if (sensor == null) {
            Log.d("datakit", "Sensor for type "+type+" is null");
        } else {
            sensorManager.registerListener(this, sensor, delay);
        }
    }

    private long lastSensorUpdate = System.currentTimeMillis();
    private void updateTimestamp() {
        long time = System.currentTimeMillis();
        Log.d("datakit", "DELAY WAS "+(time-lastSensorUpdate)+" ms");
        lastSensorUpdate = time;
    }

    @Override
    public void onSensorChanged(SensorEvent event) {

        float[] values = event.values;
        switch (event.sensor.getType()) {
            case Sensor.TYPE_LIGHT:
                float light = values[0];
                Log.d("datakit", "light = "+light);
                break;
            case Sensor.TYPE_ACCELEROMETER:
                float x = values[0];
                float y = values[1];
                float z = values[2];
//                Log.d("datakit", "acceleration = ["+x+", "+y+", "+z+"]");
                break;
            case Sensor.TYPE_GYROSCOPE:
                float a = values[0];
                float b = values[1];
                float c = values[2];
//                Log.d("datakit", "gyro = ["+a+", "+b+", "+c+"]");
                break;
            case Sensor.TYPE_AMBIENT_TEMPERATURE:
                float temp = values[0];
//                Log.d("datakit", "temperature = "+temp);
                break;
            default:
                Log.d("datakit", "Got data from unregistered sensor");
        }
    }

    @Override
    public void onAccuracyChanged(Sensor sensor, int accuracy) {

    }
}
