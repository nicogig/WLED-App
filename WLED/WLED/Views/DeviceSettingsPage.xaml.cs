using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using WLED.Models;

namespace WLED.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeviceSettingsPage : ContentPage
    {
        WLEDDevice wledDevice;
        List<string> settingsNames = new List<string>() { "wifi", "leds", "sync", "time", "sec" };
        public DeviceSettingsPage(WLEDDevice device)
        {
            InitializeComponent();
            wledDevice = device;
            deviceBrand.Text = device.LastJSONInfoModel.brand + " " + device.LastJSONInfoModel.product;
            deviceVersion.Text = "v. " + device.LastJSONInfoModel.ver;
            deviceAddress.Text = device.LastJSONInfoModel.mac.ToUpper();
            listView.ItemsSource = new[] { "WiFi Setup", "LED Preferences", "Sync Interfaces", "Time & Macros", "Security & Updates" };
        }

        private async void listView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            int itemIndex = e.ItemIndex;
            listView.SelectedItem = null;
            string urlToSend = "http://" + wledDevice.NetworkAddress + "/settings/" + settingsNames[itemIndex];
            await Navigation.PushAsync(new DeviceWebView(urlToSend, wledDevice));

        }
    }
}