using System;
using System.Collections.Generic;
using System.Text;

namespace WLED.Models
{
    class JSONStateModel
    {
        public bool on { get; set; }
        public int bri { get; set; }
        public int transition { get; set; }
        public int ps { get; set; }
        public int pl { get; set; }
        public NightLight nl { get; set; }
        public UDPSync udpn { get; set; }
        public int lor { get; set; }
        public int mainseg { get; set; }
        public IList<SegInfo> seg { get; set; }
    }

    public class NightLight
    {
        public bool on { get; set; }
        public int dur { get; set; }
        public int mode { get; set; }
        public int tbri { get; set; }
        public int rem { get; set; }
    }

    public class UDPSync
    {
        public bool send { get; set; }
        public bool recv { get; set; }
    }

    public class SegInfo
    {
        public int id { get; set; }
        public int start { get; set; }
        public int stop { get; set; }
        public int len { get; set; }
        public int grp { get; set; }
        public int of { get; set; }
        public bool on { get; set; }
        public int bri { get; set; }
        public IList<IList<int>> col { get; set; }
        public int fx { get; set; }
        public int sx { get; set; }
        public int ix { get; set; }
        public int pal { get; set; }
        public bool sel { get; set; }
        public bool rev { get; set; }
        public bool mi { get; set; }
    }
}

