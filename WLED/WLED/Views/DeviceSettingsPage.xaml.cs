﻿using System;
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
    public partial class DeviceSettingsPage : ContentPage
    {
        WLEDDevice wledDevice;
        public DeviceSettingsPage(WLEDDevice device)
        {
            InitializeComponent();
            wledDevice = device;
        }
    }
}