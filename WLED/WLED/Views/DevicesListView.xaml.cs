using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WLED.Views
{
    [Serializable()]
    [XmlRootAttribute(Namespace = "", IsNullable = false)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DevicesListView : ContentPage
    {
        private ObservableCollection<WLEDDevice> deviceList;
        public ObservableCollection<WLEDDevice> DeviceList
        {
            get { return deviceList; }
            set
            {
                deviceList = value;
                DeviceListView.ItemsSource = deviceList;
                RefreshAll();
                UpdateElementsVisibility();
            }

        }

        public DevicesListView()
        {
            InitializeComponent();

            DeviceList = new ObservableCollection<WLEDDevice>();
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetBackButtonTitle(this, "");

        }

        private void OnRefresh(object sender, EventArgs e)
        {
            RefreshAll();
            DeviceListView.EndRefresh();
        }

        protected override void OnAppearing()
        {
            UpdateElementsVisibility();
        }

        private async void OnDeleteButtonTapped (object sender, EventArgs e)
        {
            if (deviceList != null)
            {
                var page = new DeviceModificationListViewPage(deviceList);
                await Navigation.PushAsync(page);
            }
        }

        private async void OnAddButtonTapped (object sender, EventArgs e)
        {
            var page = new DeviceAddPage(this);
            page.DeviceCreated += OnDeviceCreated;
            await Navigation.PushAsync(page);
        }

        private void OnDeviceCreated (object sender, DeviceCreatedEventArgs e)
        {
            WLEDDevice toAdd = e.CreatedDevice;

            if (toAdd != null)
            {
                foreach (WLEDDevice device in deviceList)
                {
                    if (toAdd.NetworkAddress.Equals(device.NetworkAddress))
                    {
                        if (toAdd.NameIsCustom)
                        {
                            device.Name = toAdd.Name;
                            device.NameIsCustom = true;
                            ReinsertDeviceSorted(device);
                        }
                        return;
                    }
                }
                InsertDeviceSorted(toAdd);
                toAdd.PropertyChanged += DevicePropertyChanged;
                if (e.RefreshRequired) _ = toAdd.InitDevice();

                UpdateElementsVisibility();
            }

        }

        private void DevicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                ReinsertDeviceSorted(sender as WLEDDevice);
            }
        }

        private void InsertDeviceSorted (WLEDDevice device)
        {
            int index = 0;
            while (index < deviceList.Count && device.CompareTo(deviceList.ElementAt(index)) > 0) index++;
            deviceList.Insert(index, device);

        }

        private void ReinsertDeviceSorted(WLEDDevice device)
        {
            if (device == null) return;
            if (deviceList.Remove(device)) InsertDeviceSorted(device);
        }

        private void OnPowerButtonTapped (object sender, EventArgs e)
        {
            Button s = sender as Button;
            if (s.Parent.BindingContext is WLEDDevice targetDevice)
                _ = targetDevice.SendAPICall("T=2");
        }

        private async void OnDeviceTapped (object sender, ItemTappedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;

            if (!(e.Item is WLEDDevice targetDevice)) return;

            var page = new DeviceControlPage("http://" + targetDevice.NetworkAddress, targetDevice);
            await Navigation.PushAsync(page);
        }

        internal async void OpenAPDeviceControlPage()
        {
            var page = new DeviceControlPage("http://192.168.4.1", null);
            await Navigation.PushAsync(page);
        }

        private void UpdateElementsVisibility()
        {
            bool isListEmpty = (deviceList.Count == 0);

            welcomeLabel.IsVisible = isListEmpty;
            instructionLabel.IsVisible = isListEmpty;
            imageLeft.IsVisible = !isListEmpty;

            if (Device.RuntimePlatform == Device.iOS)
            {
                WLEDDevice dummy = new WLEDDevice();
                deviceList.Add(dummy);
                deviceList.Remove(dummy);
            }
        }

        internal void RefreshAll()
        {
            foreach (WLEDDevice device in deviceList) _ = device.Refresh();
        }
    }
}
