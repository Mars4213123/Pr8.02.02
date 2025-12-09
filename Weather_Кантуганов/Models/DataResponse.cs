using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Weather_Кантуганов.Models
{
    public class DataResponse
    {
        public List<Forecast> forecasts { get; set; }
    }
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
        public string ToCondition()
        {
            string result = "";
            switch (this.condition)
            {
                case "clear":
                    result = "ясно";
                    break;
                case "partly-cloudy":
                    result = "малооблочно";
                    break;
                case "cloudy":
                    result = "облочно с прояснениями";
                    break;
                case "overcast":
                    result = "gfcvehyj";
                    break;
                case "light-rain":
                    result = "небольшой дождь";
                    break;
                case "rain":
                    result = "дождь";
                    break;
                case "heavy-rain":
                    result = "сильный дождь";
                    break;
                case "showers":
                    result = "ливень";
                    break;
                case "met-anom":
                    result = "дождь со снегом";
                    break;
                case "light-snow":
                    result = "небольшой снег";
                    break;
                case "snow":
                    result = "снег";
                    break;
                case "snow-showers":
                    result = "снегопад";
                    break;
                case "hail":
                    result = "град";
                    break;
                case "thunderstorm":
                    result = "гроза";
                    break;
                case "thunderstorm-with-rain":
                    result = "дождь с грозой";
                    break;
                case "thunderstorm-with-hail":
                    result = "гроза с градом";
                    break;
                default:
                    result = this.condition;
                    break;
            }
            return result;
        }

        public string toPrecType()
        {
            string result = "";
            switch (this.prec_type)
            {
                case 0:
                    result = "без осадков";
                    break;
                case 1:
                    result = "дождь";
                    break;
                case 2:
                    result = "дождь со снегом";
                    break;
                case 3:
                    result = "снег";
                    break;
                default:
                    result = prec_type.ToString();
                    break;

            }
            return result;
        }
    }
}
