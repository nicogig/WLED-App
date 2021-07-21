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
    public partial class DeviceWebView : ContentPage
    {
        public DeviceWebView(string deviceUrl, WLEDDevice device)
        {
            InitializeComponent();
            this.Title = device.Name;
            NavigationPage.SetHasNavigationBar(this, true);
            webView.Source = new UrlWebViewSource { Url=deviceUrl };
        }
    }
}