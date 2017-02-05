package xyz.iridiumion.datakitdroid;

import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.os.Handler;
import android.os.HandlerThread;
import android.util.Log;

import java.io.IOException;
import java.io.OutputStreamWriter;
import java.io.PrintWriter;
import java.net.InetAddress;
import java.net.Socket;
import java.util.Calendar;
import java.util.Scanner;
import java.util.TimeZone;

/**
 *
 */

public class DatakitSensor implements SensorEventListener {

    static SensorManager sensorManager;
    private Socket socket;
    private PrintWriter socketOut;
    private Scanner socketIn;
    private String guid;
    private boolean connected = false;

    public DatakitSensor(final Sensor sensor, final String ip) {
        HandlerThread hThread = new HandlerThread("Sensor thread: " + sensor.getType());
        hThread.start();
        sensorManager.registerListener(DatakitSensor.this, sensor, SensorManager.SENSOR_DELAY_GAME, new Handler(hThread.getLooper()));

        new Thread() {
            public void run() {
                try {
                    socket = new Socket(InetAddress.getByName(ip), 5503);
                    socketOut = new PrintWriter(new OutputStreamWriter(socket.getOutputStream()));
                    socketIn = new Scanner(socket.getInputStream());
                    socketOut.println("$H|Android Phone|some_unit|lol|stream");
                    socketOut.flush();
                    Log.d("datakit", "Waiting for GUID, connected=" + socket.isConnected());
                    guid = socketIn.nextLine().split("\\|")[1];
                    Log.d("datakit", "GUID = " + guid);
                    connected = true;

                    while (true) {
                        try {
                            socketOut.println("$P");
                            socketOut.flush();
                            Thread.sleep(2000);
                        } catch (InterruptedException e) {
                            e.printStackTrace();
                        }
                    }
                } catch (IOException e) {
                    e.printStackTrace();
                }
            }
        }.start();
    }

    private long lastSensorUpdate = System.currentTimeMillis();

    private void updateTimestamp() {
        long time = System.currentTimeMillis();
        Log.d("datakit", "DELAY WAS " + (time - lastSensorUpdate) + " ms");
        lastSensorUpdate = time;
    }

    private String createPacket(String tag, long timestamp, float data) {
        return ">|" + tag + "|" + timestamp + "|" + data;
    }

    @Override
    public void onSensorChanged(SensorEvent event) {
        if (!connected) return; // ignore data if the socket isn't connected

        long timestamp = Calendar.getInstance(TimeZone.getTimeZone("UTC")).getTime().getTime();

        float[] values = event.values;
        switch (event.sensor.getType()) {
            case Sensor.TYPE_LIGHT:
                float light = values[0];
                Log.d("datakit", "light = " + light);

                socketOut.println(createPacket("light", timestamp, light));
                break;
            case Sensor.TYPE_ACCELEROMETER: {
                float x = values[0];
                float y = values[1];
                float z = values[2];

                socketOut.println(createPacket("x", timestamp, x));
                socketOut.println(createPacket("y", timestamp, y));
                socketOut.println(createPacket("z", timestamp, z));
//                Log.d("datakit", "acceleration = ["+x+", "+y+", "+z+"]");
            }
            break;
            case Sensor.TYPE_GYROSCOPE: {
                float x = values[0];
                float y = values[1];
                float z = values[2];
                socketOut.println(createPacket("x", timestamp, x));
                socketOut.println(createPacket("y", timestamp, y));
                socketOut.println(createPacket("z", timestamp, z));
//                Log.d("datakit", "gyro = ["+a+", "+b+", "+c+"]");
            }
            break;
            case Sensor.TYPE_AMBIENT_TEMPERATURE: {
                float temp = values[0];
                socketOut.println(createPacket("temp", timestamp, temp);
//                Log.d("datakit", "temperature = "+temp);
            }
            break;
            default:
                Log.d("datakit", "wtf Got data from unregistered sensor");
        }

//        socketOut.println(data);
        socketOut.flush();
    }

    @Override
    public void onAccuracyChanged(Sensor sensor, int accuracy) {

    }

}
