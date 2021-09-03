using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WLED.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace WLED.Views.Settings
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Settings : ContentPage
    {
        private ObservableCollection<WLEDDevice> DeviceList;

        public Settings(ObservableCollection<WLEDDevice> deviceList)
        {
            InitializeComponent();
            DeviceList = deviceList;
            bool advancedPreference = Preferences.Get("AdvancedMode", false);
            AdvancedModeSwitch.IsToggled = advancedPreference;
            AdvancedModeSwitch.Toggled += AdvancedModeSwitch_Toggled;
        }

        private async void DevicesManager_Tapped(object sender, EventArgs e)
        {
            if (DeviceList == null)
            {
                await DisplayAlert(AppResources.FeatureNotSupportedTitle,
                                   AppResources.FeatureNotSupportedDesc,
                                   AppResources.OK);
            }
            else
            {
                var page = new DeviceModificationListViewPage(DeviceList);
                await Navigation.PushAsync(page);
            }

        }

        private void AdvancedModeSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            bool currentValue = Preferences.Get("AdvancedMode", false);
            Preferences.Set("AdvancedMode", !currentValue);
// AdvancedModeSwitch.IsToggled = !currentValue;
        }
    }
}