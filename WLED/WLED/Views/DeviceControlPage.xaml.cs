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
        public DeviceControlPage(string pageURL, WLEDDevice device)
        {
            InitializeComponent();
            this.Title = device.Name;
            this.Children.Add(new DevicePage(pageURL, device));
            this.Children.Add(new DeviceEffectsPage());
            this.Children.Add(new DeviceSettingsPage());
        }
    }
}