﻿using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using WLED.Views;
using WLED.Resources;
using Zeroconf;
using System.Collections.Generic;

namespace WLED
{
    //Viewmodel: Page for adding new lights
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DeviceAddPage : ContentPage
	{
        public event EventHandler<DeviceCreatedEventArgs> DeviceCreated;
        private bool discoveryMode = false;
        private int devicesFoundCount = 0;

		public DeviceAddPage(DevicesListView list)
		{
			InitializeComponent ();

            networkAddressEntry.Focus();
        }

        private async void OnDismissTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        //If done, create device and close page
        private async void OnEntryCompleted(object sender, EventArgs e)
        {
            if (sender is Entry currentEntry) currentEntry.Unfocus();

            var device = new WLEDDevice();

            string address = networkAddressEntry.Text;
            string name = nameEntry.Text;

            if (address == null || address.Length == 0) address = "192.168.4.1";
            if (address.StartsWith("http://")) address = address.Substring(7);
            if (address.EndsWith("/")) address = address.Substring(0, address.Length -1);
            if (name == null || name.Length == 0)
            {
                name = AppResources.DefaultName;
                device.NameIsCustom = false;
            }

            device.Name = name;
            device.NetworkAddress = address;

            await Navigation.PopAsync();

            //Add device, but not if the user clicked checkmark after doing auto-discovery only
            if (devicesFoundCount == 0 || !address.Equals("192.168.4.1")) OnDeviceCreated(new DeviceCreatedEventArgs(device));
        }

        private async void OnDiscoveryButtonClicked(object sender, EventArgs e)
        {
            discoveryMode = !discoveryMode;
            Button b = sender as Button;
            if (b == null) return;
            var discovery = DeviceDiscovery.GetInstance();
            if (discoveryMode)
            {
                b.Text = AppResources.StopDiscovery;
                devicesFoundCount = 0;
                discovery.ValidDeviceFound += OnDeviceCreated;
                discoveryResultLabel.IsVisible = true;
                activityIndicator.IsVisible = true;
                discoveryResultLabel.Text = AppResources.NoLightsFound;
                bool isFinished = await discovery.StartDiscovery();
                if (isFinished)
                {
                    activityIndicator.IsVisible = false;
                    b.Text = AppResources.DiscoverLights;
                    discovery.ValidDeviceFound -= OnDeviceCreated;
                }
            } 
            else
            {
                discovery.ValidDeviceFound -= OnDeviceCreated;
                b.Text = AppResources.DiscoverLights;
            }    
        }

        protected virtual void OnDeviceCreated(DeviceCreatedEventArgs e)
        {
            DeviceCreated?.Invoke(this, e);
        }

        private void OnDeviceCreated(object sender, DeviceCreatedEventArgs e)
        {
            //this method only gets called by mDNS search, display found devices
            devicesFoundCount++;
            if (devicesFoundCount == 1)
            {
                discoveryResultLabel.Text = $"{AppResources.Found} {e.CreatedDevice.Name}!";
            } else
            {
                discoveryResultLabel.Text = $"{AppResources.Found} {e.CreatedDevice.Name} {AppResources.And} {(devicesFoundCount - 1)} {AppResources.OtherLights}";
            }

            OnDeviceCreated(e);
        }

        protected override void OnDisappearing()
        {
            //stop discovery if running
            if (discoveryMode)
            {
                var discovery = DeviceDiscovery.GetInstance();
                discovery.ValidDeviceFound -= OnDeviceCreated;
            }
        }
    }

    public class DeviceCreatedEventArgs
    {
        public WLEDDevice CreatedDevice { get; }
        public bool RefreshRequired { get; } = true;

        public DeviceCreatedEventArgs(WLEDDevice created, bool refresh = true)
        {
            CreatedDevice = created;
            RefreshRequired = refresh;
        }
    }
}