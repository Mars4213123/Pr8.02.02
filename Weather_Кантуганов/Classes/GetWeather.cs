using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Weather_Кантуганов.Models;

namespace Weather_Кантуганов.Classes
{
    public class GetWeather
    {
        public static string Url = "https://api.weather.yandex.ru/v2/forecast";
        public static string Api_Key = "demo_yandex_weather_api_key_ca6d09349ba0";
        public static async Task<DataResponse> Get(float lat, float lon) {
            DataResponse DataResponse = null;
            string url = $"{Url}?lat={lat}&lon={lon}".Replace(',', '.');
            using (HttpClient client = new HttpClient()) {
                using (HttpRequestMessage message = new HttpRequestMessage(
                        HttpMethod.Get,
                        url)) {
                    message.Headers.Add("X-Yandex-Weather-Key", Api_Key);

                    using (var Responce = await client.SendAsync(message)) {
                        string ContentResponse = await Responce.Content.ReadAsStringAsync();

                        DataResponse = JsonConvert.DeserializeObject<DataResponse>(ContentResponse);
                    }
                }
            }
            return DataResponse;
        }
    }
}

