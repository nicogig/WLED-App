using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using WLED.Views;

namespace WLED
{
    //Viewmodel: Open a web view that loads the mobile UI natively hosted on WLED device
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DeviceControlPage : TabbedPage
	{
        private WLEDDevice currentDevice;

        public DeviceControlPage(string pageURL, WLEDDevice device)
        {
            InitializeComponent();
            this.Title = device.Name;
            this.Children.Add(new DevicePage());
        }
            /*currentDevice = device;
            if (currentDevice == null) loadingLabel.Text = "Loading... (WLED-AP)"; //If the device is null, we are connected to the WLED light's access point
            UIBrowser.Source = pageURL;
            UIBrowser.Navigated += OnNavigationCompleted;
            topMenuBar.LeftButtonTapped += OnBackButtonTapped;
        }

        private void OnNavigationCompleted(object sender, WebNavigatedEventArgs e)
        {
            if (e.Result == WebNavigationResult.Success)
            {
                loadingLabel.IsVisible = false;
                if (currentDevice != null) currentDevice.CurrentStatus = DeviceStatus.Default;
            } else
            {
                if (currentDevice != null) currentDevice.CurrentStatus = DeviceStatus.Unreachable;
                loadingLabel.IsVisible = true;
                loadingLabel.Text = "Device Unreachable";
            }
        }

        private async void OnBackButtonTapped(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(false);
            currentDevice?.Refresh(); //refresh device list item to apply changes made in the control page
        }*/
    }
}