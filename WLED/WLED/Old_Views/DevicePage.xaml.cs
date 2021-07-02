using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace WLED.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DevicePage : ContentPage
    {
        public DevicePage()
        {
            InitializeComponent();
        }

        void OnLogoTapped(object sender, EventArgs e)
        {
            Launcher.OpenAsync(new Uri("https://github.com/Aircoookie/WLED"));
        }
    }
}