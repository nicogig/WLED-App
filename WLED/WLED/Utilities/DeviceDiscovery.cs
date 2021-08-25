using System;
using System.Threading.Tasks;
using Zeroconf;
using System.Collections.Generic;


namespace WLED
{
    //Discover _http._tcp services via Zeroconf and verify they are WLED devices by sending an API call
    class DeviceDiscovery
    {
        private static DeviceDiscovery Instance;
        public event EventHandler<DeviceCreatedEventArgs> ValidDeviceFound;

        public async Task<bool> StartDiscovery()
        {
            IReadOnlyList<IZeroconfHost> responses = null;
            IReadOnlyList<string> domains;
            if (ZeroconfResolver.IsiOSWorkaroundEnabled)
            {
                var iosDomains = await ZeroconfResolver.GetiOSDomains();
                string selectedDomain = (iosDomains.Count > 0) ? iosDomains[0] : null;
                domains = ZeroconfResolver.GetiOSInfoPlistServices(selectedDomain);
            }
            else
            {
                domains = new List<string>() { "_http._tcp.local." };
            }
            responses = await ZeroconfResolver.ResolveAsync(domains);
            foreach (var resp in responses)
            {
                WLEDDevice toAdd = new WLEDDevice();
                toAdd.NetworkAddress = resp.IPAddress;
                toAdd.Name = resp.DisplayName;
                toAdd.NameIsCustom = false;
                if (await toAdd.Refresh()) //check if the service is a valid WLED light
                {
                    OnValidDeviceFound(new DeviceCreatedEventArgs(toAdd, true));
                }
            }
            return true;
        }



       

        public static DeviceDiscovery GetInstance()
        {
            if (Instance == null) Instance = new DeviceDiscovery();
            return Instance;
        }

        protected virtual void OnValidDeviceFound(DeviceCreatedEventArgs e)
        {
            ValidDeviceFound?.Invoke(this, e);
        }
    }
}
