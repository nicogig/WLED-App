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

using System.Collections.ObjectModel;
using System.ComponentModel;
using Syncfusion.XForms.Buttons;

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
            OSAppTheme currentTheme = Application.Current.RequestedTheme;
            colourWheel.SelectedColor = wledDevice.ColorCurrent;
            brightnessSlider.Value = wledDevice.BrightnessCurrent;
            brightnessSlider.MinimumTrackColor = wledDevice.ColorCurrent;
            palettesPicker.ItemsSource = wledDevice.SupportedPalettes;
            palettesPicker.SelectedIndex = wledDevice.CurrentPalette;
            chipGroup.ItemsSource = new ObservableCollection<SfSegmentItem>
            {
                new SfSegmentItem() { BackgroundColor = wledDevice.ColorCurrent }

            };
            if (wledDevice.StateCurrent)
            {
                // Device is on; tint navbar maybe?
                if (currentTheme == OSAppTheme.Dark)
                {
                    bgLeft.BackgroundColor = (Color)Application.Current.Resources["DarkButtonPressed"];
                    labelLeft.BackgroundColor = (Color)Application.Current.Resources["DarkButtonPressed"];
                }
                else
                {
                    bgLeft.BackgroundColor = (Color)Application.Current.Resources["LightButtonPressed"];
                    labelLeft.BackgroundColor = (Color)Application.Current.Resources["LightButtonPressed"];
                }
                labelLeft.Text = "Off";
                nightLight.IsEnabled = true;
                brightnessSlider.IsEnabled = true;
                palettesPicker.IsEnabled = true;
                colourWheel.IsEnabled = true;
                if (wledDevice.LastJSONStateModel.nl.on)
                {
                    if (currentTheme == OSAppTheme.Dark)
                    {
                        nightLight.BackgroundColor = (Color)Application.Current.Resources["DarkButtonPressed"];
                        labelCenter.BackgroundColor = (Color)Application.Current.Resources["DarkButtonPressed"];
                    }
                    else
                    {
                        nightLight.BackgroundColor = (Color)Application.Current.Resources["LightButtonPressed"];
                        labelCenter.BackgroundColor = (Color)Application.Current.Resources["LightButtonPressed"];

                    }
                }
                else
                {
                    if (currentTheme == OSAppTheme.Dark)
                    {
                        nightLight.BackgroundColor = (Color)Application.Current.Resources["DarkNavigationBarColor"];
                        labelCenter.BackgroundColor = (Color)Application.Current.Resources["DarkNavigationBarColor"];
                    }
                    else
                    {
                        nightLight.BackgroundColor = (Color)Application.Current.Resources["LightNavigationBarColor"];
                        labelCenter.BackgroundColor = (Color)Application.Current.Resources["LightNavigationBarColor"];

                    }
                }
            }
            else
            {
                // Device is not on
                if (currentTheme == OSAppTheme.Dark)
                {
                    bgLeft.BackgroundColor = (Color)Application.Current.Resources["DarkNavigationBarColor"];
                    labelLeft.BackgroundColor = (Color)Application.Current.Resources["DarkNavigationBarColor"];
                    nightLight.BackgroundColor = (Color)Application.Current.Resources["DarkNavigationBarColor"];
                    labelCenter.BackgroundColor = (Color)Application.Current.Resources["DarkNavigationBarColor"];
                }
                else
                {
                    bgLeft.BackgroundColor = (Color)Application.Current.Resources["LightNavigationBarColor"];
                    labelLeft.BackgroundColor = (Color)Application.Current.Resources["LightNavigationBarColor"];
                    nightLight.BackgroundColor = (Color)Application.Current.Resources["LightNavigationBarColor"];
                    labelCenter.BackgroundColor = (Color)Application.Current.Resources["LightNavigationBarColor"];
                }
                labelLeft.Text = "On";
                // Disable all controls, as device isn't on
                nightLight.IsEnabled = false;
                brightnessSlider.IsEnabled = false;
                palettesPicker.IsEnabled = false;
                colourWheel.IsEnabled = false;
            }
        }

        private async void TogglePower (object sender, EventArgs e)
        {
            JSONStateModel model = wledDevice.LastJSONStateModel;
            bool previousState = model.on;
            model.on = !previousState;
            bool callResult = await wledDevice.SendStateUpdate(model);
            if (callResult)
            {
                wledDevice.StateCurrent = !wledDevice.StateCurrent;
                bool updatedJsonResult = await wledDevice.GetStatus();
                if (updatedJsonResult)
                {
                    InitPage(wledDevice);
                }
            }
        }

        private void colourWheel_SelectedColorChanged(object sender, ColorChangedEventArgs e)
        {
            
            // Colour is changed, proceed to obtain and update
            Color newColor = e.NewColor;
            JSONStateModel model = wledDevice.LastJSONStateModel;
            int newRed = Convert.ToInt32(newColor.R * 255);
            int newGreen = Convert.ToInt32(newColor.G * 255);
            int newBlue = Convert.ToInt32(newColor.B * 255);
            model.seg[model.mainseg].col[0] = new List<int>() { newRed, newGreen, newBlue };
            //bool callResult = await wledDevice.SendStateUpdate(model);
            RateLimitedSender.SendAPICall(wledDevice, model);
            wledDevice.LastJSONStateModel = model;
            wledDevice.ColorCurrent = newColor;
            brightnessSlider.MinimumTrackColor = wledDevice.ColorCurrent;

        }

        private async void brightnessSlider_DragCompleted(object sender, EventArgs e)
        {
            if ((int)brightnessSlider.Value != wledDevice.BrightnessCurrent)
            {
                JSONStateModel model = wledDevice.LastJSONStateModel;
                model.bri = (int)brightnessSlider.Value;
                bool callResult = await wledDevice.SendStateUpdate(model);
                if (callResult)
                {
                    // Request updated JSON model
                    bool updatedJsonResult = await wledDevice.GetStatus();
                    if (updatedJsonResult)
                    {
                        InitPage(wledDevice);
                    }
                }
            }
        }

        private async void nightLight_Clicked(object sender, EventArgs e)
        {
            JSONStateModel model = wledDevice.LastJSONStateModel;
            model.nl.on = !model.nl.on;
            bool callResult = await wledDevice.SendStateUpdate(model);
            if (callResult)
            {
                bool updatedJsonResult = await wledDevice.GetStatus();
                if (updatedJsonResult)
                {
                    InitPage(wledDevice);
                }
            }

        }

        private async void palettesPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            JSONStateModel model = wledDevice.LastJSONStateModel;
            model.seg[model.mainseg].pal = palettesPicker.SelectedIndex;
            bool callResult = await wledDevice.SendStateUpdate(model);
            if (callResult)
            {
                bool updatedJsonResult = await wledDevice.GetStatus();
                if (updatedJsonResult)
                {
                    InitPage(wledDevice);
                }
            }

        }
    }
}