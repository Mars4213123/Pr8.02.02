using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Weather_Кантуганов.Models;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace Weather_Кантуганов.Classes
{
    public class GetWeather
    {
        public static string WeatherUrl = "https://api.weather.yandex.ru/v2/forecast";
        public static string GeocoderUrl = "https://geocode-maps.yandex.ru/1.x/";
        public static string WeatherApiKey = "demo_yandex_weather_api_key_ca6d09349ba0";
        public static string GeocoderApiKey = "4b25a02b-707f-4273-b46b-a160ce89f30d";

        private static int dailyRequestLimit = 50;
        private static string connectionString;

        static GetWeather()
        {
            try
            {
                connectionString = "server=localhost;port=3306;database=weather_db;uid=root;password=;";
                InitializeDatabase();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка инициализации БД: {ex.Message}");
            }
        }

        private static void InitializeDatabase()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string createWeatherCacheTable = @"
                        CREATE TABLE IF NOT EXISTS WeatherCache (
                            Id INT PRIMARY KEY AUTO_INCREMENT,
                            City VARCHAR(100) NOT NULL,
                            Latitude DECIMAL(10, 6) NOT NULL,
                            Longitude DECIMAL(10, 6) NOT NULL,
                            WeatherData TEXT NOT NULL,
                            Timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                            UNIQUE KEY unique_city (City)
                        )";

                    string createDailyRequestsTable = @"
                        CREATE TABLE IF NOT EXISTS DailyRequests (
                            Id INT PRIMARY KEY AUTO_INCREMENT,
                            RequestDate DATE NOT NULL,
                            RequestCount INT DEFAULT 0,
                            UNIQUE KEY unique_date (RequestDate)
                        )";

                    using (MySqlCommand command = new MySqlCommand(createWeatherCacheTable, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    using (MySqlCommand command = new MySqlCommand(createDailyRequestsTable, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка создания таблиц: {ex.Message}");
            }
        }

        private static bool CanMakeRequest()
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT RequestCount FROM DailyRequests WHERE RequestDate = @date";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@date", today);
                        var result = command.ExecuteScalar();

                        if (result == null)
                        {
                            string insertQuery = "INSERT INTO DailyRequests (RequestDate, RequestCount) VALUES (@date, 0)";
                            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@date", today);
                                insertCommand.ExecuteNonQuery();
                            }
                            return true;
                        }

                        int count = Convert.ToInt32(result);
                        return count < dailyRequestLimit;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка проверки лимита: {ex.Message}");
                return true;
            }
        }

        private static void IncrementRequestCount()
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE DailyRequests SET RequestCount = RequestCount + 1 WHERE RequestDate = @date";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@date", today);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка увеличения счетчика: {ex.Message}");
            }
        }

        public static async Task<(float lat, float lon)> GetCoordinates(string city)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string cacheQuery = "SELECT Latitude, Longitude FROM WeatherCache WHERE City = @city";

                    using (MySqlCommand command = new MySqlCommand(cacheQuery, connection))
                    {
                        command.Parameters.AddWithValue("@city", city);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return ((float)reader.GetDecimal(0), (float)reader.GetDecimal(1));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка получения координат из кэша: {ex.Message}");
            }

            if (!CanMakeRequest())
            {
                throw new Exception("Достигнут дневной лимит запросов (50 запросов в день)");
            }

            try
            {
                var coordinates = await GeocodeCity(city);
                IncrementRequestCount();
                return coordinates;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка геокодирования: {ex.Message}");
            }
        }

        private static async Task<(float lat, float lon)> GeocodeCity(string city)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = $"{GeocoderUrl}?apikey={GeocoderApiKey}&geocode={Uri.EscapeDataString(city)}&format=json";

                var response = await client.GetAsync(url);
                var responseString = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<YandexGeocoderResponse>(responseString);

                if (result?.response?.GeoObjectCollection?.featureMember != null &&
                    result.response.GeoObjectCollection.featureMember.Count > 0)
                {
                    var geoObject = result.response.GeoObjectCollection.featureMember[0].GeoObject;
                    var pos = geoObject.Point.pos;

                    var coords = pos.Split(' ');
                    float lon = float.Parse(coords[0].Replace('.', ','));
                    float lat = float.Parse(coords[1].Replace('.', ','));

                    CacheCoordinates(city, lat, lon);
                    return (lat, lon);
                }

                throw new Exception("Город не найден");
            }
        }

        private static void CacheCoordinates(string city, float lat, float lon)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        INSERT INTO WeatherCache (City, Latitude, Longitude, WeatherData) 
                        VALUES (@city, @lat, @lon, '') 
                        ON DUPLICATE KEY UPDATE 
                        Latitude = @lat, 
                        Longitude = @lon, 
                        Timestamp = CURRENT_TIMESTAMP";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@city", city);
                        command.Parameters.AddWithValue("@lat", lat);
                        command.Parameters.AddWithValue("@lon", lon);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка кэширования координат: {ex.Message}");
            }
        }

        public static async Task<DataResponse> GetWeatherData(string city)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string cacheQuery = "SELECT WeatherData FROM WeatherCache WHERE City = @city AND WeatherData != ''";

                    using (MySqlCommand command = new MySqlCommand(cacheQuery, connection))
                    {
                        command.Parameters.AddWithValue("@city", city);
                        var cachedData = command.ExecuteScalar();

                        if (cachedData != null)
                        {
                            var weatherData = cachedData.ToString();
                            if (!string.IsNullOrEmpty(weatherData))
                            {
                                var data = JsonConvert.DeserializeObject<DataResponse>(weatherData);
                                if (data != null && data.forecasts != null && data.forecasts.Count > 0)
                                {
                                    return data;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка получения данных из кэша: {ex.Message}");
            }

            try
            {
                var coordinates = await GetCoordinates(city);
                var weather = await GetWeatherFromApi(coordinates.lat, coordinates.lon);
                CacheWeatherData(city, weather);
                return weather;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения погоды: {ex.Message}");
            }
        }

        public static async Task<DataResponse> GetWeatherFromApi(float lat, float lon)
        {
            if (!CanMakeRequest())
            {
                throw new Exception("Достигнут дневной лимит запросов (50 запросов в день)");
            }

            try
            {
                DataResponse DataResponse = null;
                string url = $"{WeatherUrl}?lat={lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}&lon={lon.ToString(System.Globalization.CultureInfo.InvariantCulture)}";

                using (HttpClient client = new HttpClient())
                {
                    using (HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, url))
                    {
                        message.Headers.Add("X-Yandex-Weather-Key", WeatherApiKey);

                        using (var Response = await client.SendAsync(message))
                        {
                            string ContentResponse = await Response.Content.ReadAsStringAsync();
                            DataResponse = JsonConvert.DeserializeObject<DataResponse>(ContentResponse);
                        }
                    }
                }

                IncrementRequestCount();
                return DataResponse;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка API погоды: {ex.Message}");
            }
        }

        private static void CacheWeatherData(string city, DataResponse weatherData)
        {
            try
            {
                string serializedData = JsonConvert.SerializeObject(weatherData);

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        INSERT INTO WeatherCache (City, Latitude, Longitude, WeatherData) 
                        VALUES (@city, 0, 0, @weatherData) 
                        ON DUPLICATE KEY UPDATE 
                        WeatherData = @weatherData, 
                        Timestamp = CURRENT_TIMESTAMP";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@city", city);
                        command.Parameters.AddWithValue("@weatherData", serializedData);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка кэширования погоды: {ex.Message}");
            }
        }

        public static int GetTodayRequestCount()
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT RequestCount FROM DailyRequests WHERE RequestDate = @date";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@date", today);
                        var result = command.ExecuteScalar();

                        return result == null ? 0 : Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка получения счетчика: {ex.Message}");
                return 0;
            }
        }
    }

    public class YandexGeocoderResponse
    {
        public Response response { get; set; }
    }

    public class Response
    {
        public GeoObjectCollection GeoObjectCollection { get; set; }
    }

    public class GeoObjectCollection
    {
        public List<FeatureMember> featureMember { get; set; }
    }

    public class FeatureMember
    {
        public GeoObject GeoObject { get; set; }
    }

    public class GeoObject
    {
        public Point Point { get; set; }
    }

    public class Point
    {
        public string pos { get; set; }
    }
}