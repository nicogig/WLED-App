using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Xamarin.Forms;
using Newtonsoft.Json;
using WLED.Models;

namespace WLED
{
    enum DeviceStatus { Default, Unreachable, Error };

    //Data Model. Represents a WLED light with a network address, name, and some current light values.
    [XmlType("dev")]
    public class WLEDDevice : INotifyPropertyChanged, IComparable
    {
        private string networkAddress = "192.168.4.1";                          //device IP (can also be hostname if applicable)
        private string name = "";                                               //device display name ("Server Description")
        private DeviceStatus status = DeviceStatus.Default;                     //Current connection status
        private bool stateCurrent = false;                                      //Is the light currently on?
        private bool isEnabled = true;                                          //Disabled devices don't get polled or show up in the list
        private double brightnessReceived = 0.9, brightnessCurrent = 0.9;       //There are two vars for brightness to discern API responses from slider updates
        private List<string> supportedPalettes;
        private JSONStateModel lastJSONStateModel;

        [XmlElement("url")]
        public string NetworkAddress
        {
            set
            {
                if (value == null || value.Length < 3) return; //More elaborate checking for URL syntax could be added here
                networkAddress = value;
            }
            get { return networkAddress; }
        }

        [XmlElement("name")]
        public string Name
        { 
            set
            {
                if (value == null || name.Equals(value)) return; //Make sure name is not set to null
                name = value;
                OnPropertyChanged("Name");
            }
            get { return name; }
        }

        internal DeviceStatus CurrentStatus
        {
            set
            {
                status = value;
                OnPropertyChanged("Status");
            }
            get { return status; }
        }

        [XmlElement("ncustom")]
        public bool NameIsCustom { get; set; } = true; //If the light name is custom, the name returned by the API response will be ignored

        [XmlElement("en")]
        public bool IsEnabled
        {
            set
            {
                isEnabled = value;
                OnPropertyChanged("Status");
                OnPropertyChanged("ListHeight");
                OnPropertyChanged("TextColor");
                OnPropertyChanged("IsEnabled");
            }
            get { return isEnabled; }
        }

        [XmlIgnore]
        public double BrightnessCurrent
        {
            set
            {
                brightnessCurrent = value;
                if (brightnessCurrent != brightnessReceived) //only send if value was changed by slider
                {
                    byte toSend = (byte)Math.Round(brightnessCurrent);
                    RateLimitedSender.SendAPICall(this, "A=" + toSend);
                }
            }
            get { return brightnessCurrent; }
        }

        [XmlIgnore]
        public Color ColorCurrent { get; set; }

        [XmlIgnore]
        public int CurrentPalette { get; set; }

        [XmlIgnore]
        public bool StateCurrent
        {
            get { return stateCurrent; }
            set { OnPropertyChanged("StateColor"); stateCurrent = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        //helper properties for updating view dynamically via data binding
        [XmlIgnore]
        public Color StateColor { get { return StateCurrent ? Color.FromHex("#666") : Color.FromHex("#222"); } } //button background color

        [XmlIgnore]
        public string ListHeight { get { return isEnabled ? "-1" : "0"; } } //height of one view cell (set to 0 to hide device)

        [XmlIgnore]
        public string TextColor { get { return isEnabled ? "#FFF" : "#999"; } } //text color for modification page

        [XmlIgnore]
        public string Status //string containing IP and current status, second label in list viewcell
        {
            get
            {
                string statusText = "";
                if (IsEnabled)
                {
                    switch (status)
                    {
                        case DeviceStatus.Default: statusText = ""; break;
                        case DeviceStatus.Unreachable: statusText = " (Offline)"; break;
                        case DeviceStatus.Error: statusText = " (Error)"; break;
                    }
                }
                else
                {
                    statusText = " (Hidden)";
                }
                return string.Format("{0}{1}", networkAddress, statusText);
            }
        }

        [XmlElement("pal")]
        public List<string> SupportedPalettes 
        { 
            get 
            { 
                return supportedPalettes; 
            }
            set 
            { 
                supportedPalettes = value; 
            } 
        }

        [XmlIgnore]
        public JSONStateModel LastJSONStateModel
        {
            get { return lastJSONStateModel; } set { lastJSONStateModel = value; }
        }


        //constructors
        public WLEDDevice() { }

        public WLEDDevice(string nA, string name)
        {
            NetworkAddress = nA;
            Name = name;
        }

        //member functions

        public async Task<string> MakeConnection(string apiCall)
        {
            string url = "http://" + networkAddress;

            if (networkAddress.StartsWith("https://"))
            {
                url = networkAddress;
            }

            string response = await DeviceHttpConnection.GetInstance().GetWLEDJson(url, apiCall);
            if (response == null)
            {
                CurrentStatus = DeviceStatus.Unreachable;
                return null;
            }
            else if (response == "err")
            {
                CurrentStatus = DeviceStatus.Error;
                return "err";
            }
            CurrentStatus = DeviceStatus.Default;
            return response;
        }

        public async Task<bool> SendStateUpdate (JSONStateModel jsonStateModel)
        {
            string url = "http://" + networkAddress;

            if (networkAddress.StartsWith("https://"))
            {
                url = networkAddress;
            }
            string response = await DeviceHttpConnection.GetInstance().SendStatusUpdate(url, jsonStateModel);
            if (response == null)
            {
                CurrentStatus = DeviceStatus.Unreachable;
                return false;
            }
            else if (response == "err")
            {
                CurrentStatus = DeviceStatus.Error;
                return false;
            }
            try
            {
                JSONResponseModel jsonResponse = JsonConvert.DeserializeObject<JSONResponseModel>(response);
                CurrentStatus = DeviceStatus.Default;
                return jsonResponse.success;
            }
            catch
            {
                CurrentStatus = DeviceStatus.Error;
            }
            return false;
        }

        public async Task<bool> InitDevice() //fetches updated values from WLED device
        {
            if (!IsEnabled) return false;
            return await GetInfo() && await GetStatus() && await GetPalettesInfo();
        }

        public async Task<bool> GetPalettesInfo()
        {
            string response = await MakeConnection("palettes");
            if (response == null || response == "err") return false;

            try
            {
                SupportedPalettes = JsonConvert.DeserializeObject<List<string>>(response);
            }
            catch (Exception)
            {
                CurrentStatus = DeviceStatus.Error;
                return false;
            }
            return true;
        }

        public async Task<bool> GetInfo()
        {
            string response = await MakeConnection("info");
            if (response == null || response == "err") return false;

            JSONInfoModel jsonInfoModel = new JSONInfoModel();
            try
            {
                jsonInfoModel = JsonConvert.DeserializeObject<JSONInfoModel>(response);
            }
            catch (Exception)
            {
                CurrentStatus = DeviceStatus.Error;
                return false;
            }
            CurrentStatus = DeviceStatus.Default;
            if (!NameIsCustom) Name = jsonInfoModel.name;

            return true;
        }

        public async Task<bool> GetStatus()
        {
            string response = await MakeConnection("state");
            if (response == null || response == "err") return false;

            JSONStateModel jsonStateModel = new JSONStateModel();
            try
            {
                jsonStateModel = JsonConvert.DeserializeObject<JSONStateModel>(response);
            }
            catch (Exception)
            {
                CurrentStatus = DeviceStatus.Error;
                return false;
            }
            CurrentStatus = DeviceStatus.Default;
            LastJSONStateModel = jsonStateModel;
            StateCurrent = jsonStateModel.on;
            if (jsonStateModel.bri > 0 && jsonStateModel.bri != BrightnessCurrent)
            {
                brightnessReceived = jsonStateModel.bri;
                BrightnessCurrent = jsonStateModel.bri;
                OnPropertyChanged("BrightnessCurrent");
            }
            int mainseg = jsonStateModel.mainseg;
            SegInfo segInfos = jsonStateModel.seg[mainseg];
            IList<int> colorIndexesPrimary = segInfos.col[0];
            ColorCurrent = Color.FromRgb(colorIndexesPrimary[0], colorIndexesPrimary[1], colorIndexesPrimary[2]);
            CurrentPalette = segInfos.pal;
            return true;
        }

        //send a call to this device's WLED HTTP API
        //API reference: http://apiref.wled.me
        public async Task<bool> SendAPICall(string call)
        {
            string url = "http://" + networkAddress;
            if (networkAddress.StartsWith("https://"))
            {
                url = networkAddress;
            }

            string response = await DeviceHttpConnection.GetInstance().Send_WLED_API_Call(url, call);
            if (response == null)
            {
                CurrentStatus = DeviceStatus.Unreachable;
                return false;
            }

            if (response.Equals("err")) //404 or other non-success http status codes, indicates that target is not a WLED device
            {
                CurrentStatus = DeviceStatus.Error;
                return false;
            }

            XmlApiResponse deviceResponse = XmlApiResponseParser.ParseApiResponse(response);
            if (deviceResponse == null) //could not parse XML API response
            {
                CurrentStatus = DeviceStatus.Error;
                return false;
            }

            CurrentStatus = DeviceStatus.Default; //the received response was valid

            if (!NameIsCustom) Name = deviceResponse.Name;

            //only consider brightness if light is on and if it wasn't modified in the same call (prevents brightness slider "jumps")
            if (deviceResponse.Brightness > 0 && !call.Contains("A="))
            {
                brightnessReceived = deviceResponse.Brightness;
                BrightnessCurrent = brightnessReceived;
                OnPropertyChanged("BrightnessCurrent"); //update slider binding
            }

            ColorCurrent = deviceResponse.LightColor;
            OnPropertyChanged("ColorCurrent");

            StateCurrent = deviceResponse.IsOn;
            return true;
        }

        public async Task<bool> Refresh() //fetches updated values from WLED device
        {
            if (!IsEnabled) return false;
            return await GetInfo() && await GetStatus();
        }

        public int CompareTo(object comp) //compares devices in alphabetic order based on name
        {
            WLEDDevice c = comp as WLEDDevice;
            if (c == null || c.Name == null) return 1;
            int result = (name.CompareTo(c.name));
            if (result != 0) return result;
            return (networkAddress.CompareTo(c.networkAddress));
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
