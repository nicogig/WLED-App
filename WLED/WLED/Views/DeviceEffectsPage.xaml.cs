using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using WLED.Models;
using Newtonsoft.Json;

namespace WLED.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeviceEffectsPage : ContentPage
    {
        WLEDDevice wledDevice;
        public DeviceEffectsPage(string pageURL, WLEDDevice device)
        {
            InitializeComponent();
            wledDevice = device;
            livePreview.Source = new UrlWebViewSource { Url = pageURL + "/liveview", };
            effectsList.ItemsSource = wledDevice.SupportedEffects;
            effectsList.SelectedItem = wledDevice.SupportedEffects[wledDevice.LastJSONStateModel.seg[wledDevice.LastJSONStateModel.mainseg].fx];
            
        }

        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            JSONStateModel model = wledDevice.LastJSONStateModel;
            model.seg[model.mainseg].fx = e.ItemIndex;
            bool callResult = await wledDevice.SendStateUpdate(model);
            if (callResult)
            {
                wledDevice.LastJSONStateModel = model;
            }
        }
    }
}