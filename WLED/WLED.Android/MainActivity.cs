using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Android.Net.Wifi;
using Xamarin.Forms.Platform.Android;

namespace WLED.Droid
{
    [Activity(Label = "WLED", Icon = "@drawable/LogoACh", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        WifiManager wifi;
        WifiManager.MulticastLock multicastLock;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            wifi = (WifiManager)ApplicationContext.GetSystemService(Context.WifiService);
            multicastLock = wifi.CreateMulticastLock("WLED Zeroconf Lock");
            multicastLock.Acquire();

            LoadApplication(new App());
            
        }

        protected override void OnDestroy()
        {
            if (multicastLock != null) 
            {
                multicastLock.Release();
                multicastLock = null;
            }
            base.OnDestroy();
        
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}