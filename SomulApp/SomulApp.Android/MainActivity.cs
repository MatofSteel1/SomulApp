using System;
using System.IO;
using Android.Bluetooth;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using Java.Util;
using System.Threading.Tasks;
using SomulApp.Droid;
using Android.Content;
using Android.Runtime;

namespace SomulApp.Droid
{
    [Activity(Label = "Somul Remote", Icon = "@drawable/icon", Theme = "@style/MainTheme",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IBluetoothConnect, IToastMessageDisplay
    {
        private static string address = "00:21:13:01:B3:56";
        private static UUID MY_UUID = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");
        public BluetoothAdapter mSomulBluetoothAdapter = null;
        private BluetoothSocket BTSocket = null;
        private String result;
        private Stream outStream = null;
        private Stream inStream = null;

        Context context;

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }

        private void CheckBT()
        {
            mSomulBluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            if (!mSomulBluetoothAdapter.Enable())
            {
                System.Console.WriteLine("Bluetooth Disactivated");
                ShowMessage("Bluetooth Disactivated");
            }

            if (mSomulBluetoothAdapter == null)
            {
                System.Console.WriteLine("Bluetooth Does Not Exist or is Occupied");
                ShowMessage("Bluetooth Does Not Exist or is Occupied");
            }
        }

        public bool Connect()
        {
            ShowMessage("Connection in progress...");
            CheckBT();

            BluetoothDevice device = mSomulBluetoothAdapter.GetRemoteDevice(address);
            System.Console.WriteLine("Connection in progress... " + device);

            mSomulBluetoothAdapter.CancelDiscovery();
            try
            {
                BTSocket = device.CreateInsecureRfcommSocketToServiceRecord(MY_UUID);
                BTSocket.Connect();

                ShowMessage("Connected");
                System.Console.WriteLine("Connection Complete");
                return true;
            }
            catch (System.Exception e)
            {
                ShowMessage("Could Not Connect");
                System.Console.WriteLine("Could Not Connect " + e.Message);
                return false;
            }
            finally
            {
                System.Console.WriteLine("Socket Created");
            }
        }

        public bool Disconnect()
        {
            try
            {
                BTSocket.Close();
                return true;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public void BeginListenForData()
        {
            try
            {
                inStream = BTSocket.InputStream;
            }
            catch (System.IO.IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
            Task.Factory.StartNew(() => {
                byte[] buffer = new byte[1024];
                int bytes;
                while (true)
                {
                    try
                    {
                        bytes = inStream.Read(buffer, 0, buffer.Length);
                        if (bytes > 0)
                        {
                            RunOnUiThread(() => {
                                string value = System.Text.Encoding.ASCII.GetString(buffer);
                                result = result + "\n" + value;
                            });
                        }
                    }
                    catch (Java.IO.IOException)
                    {
                        RunOnUiThread(() => {
                            result = string.Empty;
                        });
                        break;
                    }
                }
            });
        }

        public void WriteData(string data)
        {
            try
            {
                outStream = BTSocket.OutputStream;
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error Sending " + e.Message);
            }

            Java.Lang.String message = new Java.Lang.String(data);

            byte[] msgBuffer = message.GetBytes();

            try
            {
                outStream.Write(msgBuffer, 0, msgBuffer.Length);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error Sending " + e.Message);
            }
        }

        public void ShowMessage(string msg)
        {
            context = Application.Context;
            Toast.MakeText(context, msg, ToastLength.Short).Show();
        }
    }
}