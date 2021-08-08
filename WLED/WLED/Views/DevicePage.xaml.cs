using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using WLED.Models;
using WLED.Resources;
using ColorPicker.BaseClasses.ColorPickerEventArgs;
using System.Collections.ObjectModel;
using Syncfusion.XForms.Buttons;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

namespace WLED.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DevicePage : ContentPage
    {
        string DeviceURI;
        WLEDDevice wledDevice;
        ObservableCollection<SfSegmentItem> chipItemSource;
        string SegoeFontFamily = "SegoeMDL2";

        public DevicePage(string pageURL, WLEDDevice device)
        {
            InitializeComponent();
// SegoeFontFamily = Device.RuntimePlatform == Device.iOS ? "Segoe MDL2 Assets" : "SegMDL2.ttf";
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
            chipItemSource = new ObservableCollection<SfSegmentItem>();
            foreach (Color item in wledDevice.ColorCombos)
            {
                chipItemSource.Add(new SfSegmentItem() { IconFont = "\uE91F", FontIconFontColor = item, Text = "Square", FontIconFontSize = 32, FontIconFontFamily = SegoeFontFamily, SelectionBackgroundColor = Color.Transparent });
            }
            chipGroup.ItemsSource = chipItemSource;
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
                labelLeft.Text = AppResources.Off;
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
                labelLeft.Text = AppResources.On;
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
            int selectedIndex = chipGroup.SelectedIndex;
            JSONStateModel model = wledDevice.LastJSONStateModel;
            int newRed = Convert.ToInt32(newColor.R * 255);
            int newGreen = Convert.ToInt32(newColor.G * 255);
            int newBlue = Convert.ToInt32(newColor.B * 255);
            model.seg[model.mainseg].col[selectedIndex] = new List<int>() { newRed, newGreen, newBlue };
            RateLimitedSender.SendAPICall(wledDevice, model);
            wledDevice.LastJSONStateModel = model;
            wledDevice.ColorCurrent = newColor;
            brightnessSlider.MinimumTrackColor = wledDevice.ColorCurrent;
            chipItemSource[selectedIndex].FontIconFontColor = newColor;

        }

        private void changeColour(Color newColor)
        {
            int selectedIndex = chipGroup.SelectedIndex;
            JSONStateModel model = wledDevice.LastJSONStateModel;
            int newRed = Convert.ToInt32(newColor.R * 255);
            int newGreen = Convert.ToInt32(newColor.G * 255);
            int newBlue = Convert.ToInt32(newColor.B * 255);
            model.seg[model.mainseg].col[selectedIndex] = new List<int>() { newRed, newGreen, newBlue };
            RateLimitedSender.SendAPICall(wledDevice, model);
            wledDevice.LastJSONStateModel = model;
            wledDevice.ColorCurrent = newColor;
            brightnessSlider.MinimumTrackColor = wledDevice.ColorCurrent;
            chipItemSource[selectedIndex].FontIconFontColor = newColor;
            colourWheel.SelectedColor = newColor;
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

        private void chipGroup_SelectionChanged(object sender, Syncfusion.XForms.Buttons.SelectionChangedEventArgs e)
        {
            int newSelection = e.Index;
            Color currentColor = chipItemSource[newSelection].FontIconFontColor;
            brightnessSlider.MinimumTrackColor = currentColor;
            colourWheel.SelectedColor = currentColor;
        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                var photo = await MediaPicker.CapturePhotoAsync();
                Stream stream = await photo.OpenReadAsync();
                Color dominantColor = Utilities.ImageUtils.GetAverageColor(stream);
                changeColour(dominantColor);
            }
            catch (FeatureNotSupportedException)
            {
                await DisplayAlert(AppResources.FeatureNotSupportedTitle,
                                   AppResources.FeatureNotSupportedDesc,
                                   AppResources.OK);
            }
            catch (PermissionException)
            {
                await DisplayAlert(AppResources.PermissionNotGrantedTitle,
                                   AppResources.PermissionNotGrantedDesc,
                                   AppResources.OK);
            }
            catch (Exception ex)
            {
                await DisplayAlert(AppResources.CameraExceptionTitle,
                    AppResources.CameraExceptionDesc,
                    AppResources.OK);
                Console.WriteLine($"CapturePhotoAsync THREW: {ex.Message}");
            }
        }
    }
}