using Xamarin.Forms;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;
using System.Linq;
using WLED.Views;
using System.Resources;

/*
 * WLED App
 * (c) 2021 Nicola Gigante
 * (c) 2019 Christian Schwinne
 * Licensed under the MIT license
 * 
 * This project was build for and tested with Android and iOS.
 */
[assembly: ExportFont("SegMDL2.ttf", Alias = "SegoeMDL2")]
[assembly: NeutralResourcesLanguage("en-US")]
[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace WLED
{
    public partial class App : Application
    {
        private DevicesListView listview;

        private bool connectedToLocalLast = false;

        public App()
        {

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(Secrets.LicenseKey);


            InitializeComponent();

            listview = new DevicesListView();
            MainPage = new NavigationPage(listview);

            Connectivity.ConnectivityChanged += OnConnectivityChanged;
        }

        protected override void OnStart()
        {
            //Directly open the device web page if connected to WLED Access Point
            if (NetUtility.IsConnectedToWledAP()) listview.OpenAPDeviceControlPage();

            // Load device list from Preferences
            if (Preferences.ContainsKey("wleddevices"))
            {
                string devices = Preferences.Get("wleddevices", "");
                if (!devices.Equals(""))
                {
                    ObservableCollection<WLEDDevice> fromPreferences = Serialization.Deserialize(devices);
                    if (fromPreferences != null) listview.DeviceList = fromPreferences;
                }
                listview.RefreshAll();
            }
        }

        protected override void OnSleep()
        {
            //Handle when app sleeps, save device list to Preferences
            string devices = Serialization.SerializeObject(listview.DeviceList);
            Preferences.Set("wleddevices", devices);
        }

        protected override void OnResume()
        {
            //Handle when app resumes, directly open the device web page if connected to WLED Access Point
            if (NetUtility.IsConnectedToWledAP()) listview.OpenAPDeviceControlPage();

            //Refresh light states
            listview.RefreshAll();
        }

        private void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            //Detect if currently connected to local (WiFi) or mobile network
            var profiles = Connectivity.ConnectionProfiles;
            bool connectedToLocal = (profiles.Contains(ConnectionProfile.WiFi) || profiles.Contains(ConnectionProfile.Ethernet));

            //Directly open the device web page if connected to WLED Access Point
            if (connectedToLocal && NetUtility.IsConnectedToWledAP()) listview.OpenAPDeviceControlPage();

            //Refresh all devices on connection change
            if (connectedToLocal && !connectedToLocalLast) listview.RefreshAll();
            connectedToLocalLast = connectedToLocal;
        }
    }
}