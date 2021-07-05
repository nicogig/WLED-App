using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using WLED;

namespace WLED.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DevicePage : ContentPage
    {
        string DeviceURI;
        WLEDDevice wledDevice;

        public DevicePage(string pageURL, WLEDDevice device)
        {
            InitializeComponent();
            DeviceURI = pageURL;
            wledDevice = device;
            InitPage(device);
            
        }

        private void InitPage (WLEDDevice device)
        {
            colourWheel.SelectedColor = wledDevice.ColorCurrent;
            brightnessSlider.Value = wledDevice.BrightnessCurrent;
            brightnessSlider.MinimumTrackColor = wledDevice.ColorCurrent;
            palettesPicker.ItemsSource = wledDevice.SupportedPalettes;
            palettesPicker.SelectedIndex = wledDevice.CurrentPalette;
            if (wledDevice.StateCurrent)
            {
                // Device is on
                bgLeft.BackgroundColor = Color.FromHex("#585858");
                labelLeft.Text = "Off";

            }
            

        }

        
    }
}