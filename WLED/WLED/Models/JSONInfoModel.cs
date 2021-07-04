using System;
using System.Collections.Generic;
using System.Text;

namespace WLED.Models
{
    class JSONInfoModel
    {
        public string ver { get; set; }
        public int vid { get; set; }
        public LEDSetup leds { get; set; }
        public bool str { get; set; }
        public string name { get; set; }
        public int udpport { get; set; }
        public bool live { get; set; }
        public string lm { get; set; }
        public string lip { get; set; }
        public int ws { get; set; }
        public int fxcount { get; set; }
        public int palcount { get; set; }
        public WiFiInfo wifi { get; set; }
        public FilesystemInfo fs { get; set; }
        public int ndc { get; set; }
        public string arch { get; set; }
        public string core { get; set; }
        public int lwip { get; set; }
        public int freeheap { get; set; }
        public int uptime { get; set; }
        public int opt { get; set; }
        public string brand { get; set; }
        public string product { get; set; }
        public string mac { get; set; }
        public string ip { get; set; }
    }

    public class LEDSetup
    {
        public int count { get; set; }
        public bool rgbw { get; set; }
        public bool wv { get; set; }
        public int pwr { get; set; }
        public int fps { get; set; }
        public int maxpwr { get; set; }
        public int maxseg { get; set; }
        public bool seglock { get; set; }
    }

    public class WiFiInfo
    {
        public string bssid { get; set; }
        public int rssi { get; set; }
        public int signal { get; set; }
        public int channel { get; set; }
    }

    public class FilesystemInfo
    {
        public int u { get; set; }
        public int t { get; set; }
        public int pmt { get; set; }
    }
}
