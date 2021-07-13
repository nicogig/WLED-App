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
            if (device.CurrentStatus == DeviceStatus.Default)
            {
                Children.Add(new DevicePage(pageURL, device));
                Children.Add(new DeviceEffectsPage(pageURL, device));
                Children.Add(new DeviceSettingsPage(device));
            }
            else
            {
                Children.Add(new DeviceErrorPage());
            }
            
        }
    }
}