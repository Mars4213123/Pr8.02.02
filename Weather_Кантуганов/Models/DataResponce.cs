using System;
using System.Collections.Generic;
using System.Text;

namespace Weather_Кантуганов.Models
{
    public class DataResponce
    {
        public List<Forecast> forecasts { get; set; }
        public class Forecast
        {
            public string date { get; set; }
            public List<Hour> hours { get; set; }
        }
        public class Hour
        {
            public string hour { get; set; }
            public string condition { get; set; }
            public int humidity { get; set; }
            public int prec_type { get; set; }
            public int temp { get; set; }
        }
    }
}
