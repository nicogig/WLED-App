using System;
using System.Collections.Generic;
using System.Text;

namespace WLED.Models
{
    public class JSONMainModel
    {
        public JSONStateModel state { get; set;}
        public JSONInfoModel info { get; set; }
        public List<string> effects { get; set; }
        public List<string> palettes { get; set; }
    }
}
