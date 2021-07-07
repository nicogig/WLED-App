using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using WLED;
using WLED.Models;
using ColorPicker;
using ColorPicker.BaseClasses;
using ColorPicker.BaseClasses.ColorPickerEventArgs;
using Newtonsoft.Json;

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
            colourWheel.SelectedColorChanged += colourWheel_SelectedColorChanged;
            
            
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
                // Device is on; tint navbar maybe?
                bgLeft.BackgroundColor = Color.FromHex("#585858");
                labelLeft.BackgroundColor = Color.FromHex("#585858");
                labelLeft.Text = "Off";
            }
        }

        private async void TogglePower (object sender, EventArgs e)
        {
            JSONStateModel model = wledDevice.LastJSONStateModel;
            model.on = !wledDevice.StateCurrent;
            bool callResult = await wledDevice.SendStateUpdate(model);
            if (callResult)
            {
                wledDevice.StateCurrent = !wledDevice.StateCurrent;
                wledDevice.LastJSONStateModel.on = wledDevice.StateCurrent;
                if (wledDevice.StateCurrent)
                {
                    // Device is on
                    bgLeft.BackgroundColor = Color.FromHex("#585858");
                    labelLeft.BackgroundColor = Color.FromHex("#585858");
                    labelLeft.Text = "Off";
                }
                else
                {
                    bgLeft.BackgroundColor = Color.FromHex("#333");
                    labelLeft.BackgroundColor = Color.FromHex("#333");
                    labelLeft.Text = "On";
                }
            }
        }

        private async void colourWheel_SelectedColorChanged(object sender, ColorChangedEventArgs e)
        {
            
            // Colour is changed, proceed to obtain and update
            Color newColor = e.NewColor;
            JSONStateModel model = wledDevice.LastJSONStateModel;
            int newRed = Convert.ToInt32(newColor.R * 255);
            int newGreen = Convert.ToInt32(newColor.G * 255);
            int newBlue = Convert.ToInt32(newColor.B * 255);
            model.seg[model.mainseg].col[0] = new List<int>() { newRed, newGreen, newBlue };
            bool callResult = await wledDevice.SendStateUpdate(model);
            if (callResult)
            {
                // Call was successful, so update last call to the one we just sent
                wledDevice.LastJSONStateModel = model;
                wledDevice.ColorCurrent = newColor;
                brightnessSlider.MinimumTrackColor = wledDevice.ColorCurrent;
            }

        }
    }
}