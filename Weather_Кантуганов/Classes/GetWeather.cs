using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Weather_Кантуганов.Models;
using MySql.Data.MySqlClient;

namespace Weather_Кантуганов.Classes
{
    public class GetWeather
    {
        public static string WeatherUrl = "https://api.weather.yandex.ru/v2/forecast";
        public static string WeatherApiKey = "demo_yandex_weather_api_key_ca6d09349ba0";

        private static int dailyRequestLimit = 50;
        private static string connectionString = "server=MySQL-8.2;port=3306;database=weather_db;uid=root;password=;";

        static GetWeather()
        {
            InitializeDatabase();
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
                            Latitude DECIMAL(10, 6) NOT NULL DEFAULT 0,
                            Longitude DECIMAL(10, 6) NOT NULL DEFAULT 0,
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

        public static async Task<DataResponse> GetWeatherData(string city)
        {
            Console.WriteLine($"Попытка получить данные для города: {city}");

            // Шаг 1: Пытаемся получить данные из БД
            var dbData = GetWeatherFromDatabase(city);
            if (dbData != null)
            {
                // Проверяем возраст данных
                int minutesAgo = GetDataAgeMinutes(city);
                Console.WriteLine($"Данные из БД найдены, возраст: {minutesAgo} минут");

                if (minutesAgo < 30 && minutesAgo >= 0)
                {
                    Console.WriteLine($"Данные свежие (< 30 минут), используем из БД");
                    return dbData;
                }
                else
                {
                    Console.WriteLine($"Данные устарели ({minutesAgo} минут) или отсутствуют, проверяем API");
                }
            }
            else
            {
                Console.WriteLine($"Данных в БД нет для города: {city}");
            }

            // Шаг 2: Если данных нет или они устарели, запрашиваем из API
            if (!CanMakeRequest())
            {
                Console.WriteLine("Достигнут дневной лимит запросов (50)");
                if (dbData != null)
                {
                    Console.WriteLine("Используем устаревшие данные из БД");
                    return dbData;
                }
                throw new Exception("Достигнут дневной лимит запросов и данных в БД нет");
            }

            Console.WriteLine($"Запрашиваем новые данные из API для города: {city}");
            try
            {
                var coordinates = await GetCoordinates(city);
                var apiData = await GetWeatherFromApi(coordinates.lat, coordinates.lon);

                // Сохраняем новые данные в БД
                CacheWeatherData(city, apiData);
                Console.WriteLine($"Новые данные сохранены в БД для города: {city}");

                return apiData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка API: {ex.Message}");
                if (dbData != null)
                {
                    Console.WriteLine("Используем устаревшие данные из БД");
                    return dbData;
                }
                throw new Exception($"Не удалось получить данные: {ex.Message}");
            }
        }

        private static DataResponse GetWeatherFromDatabase(string city)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT WeatherData FROM WeatherCache WHERE City = @city AND WeatherData != ''";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@city", city);
                        var result = command.ExecuteScalar();

                        if (result != null)
                        {
                            var weatherData = result.ToString();
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
                Console.WriteLine($"Ошибка получения данных из БД: {ex.Message}");
            }

            return null;
        }

        private static int GetDataAgeMinutes(string city)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT TIMESTAMPDIFF(MINUTE, Timestamp, NOW()) 
                        FROM WeatherCache 
                        WHERE City = @city 
                        AND WeatherData != ''";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@city", city);
                        var result = command.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка проверки возраста данных: {ex.Message}");
            }

            return -1; // Данных нет
        }

        private static async Task<(float lat, float lon)> GetCoordinates(string city)
        {
            // Словарь городов с координатами
            var cities = new Dictionary<string, (float, float)>
            {
                {"Москва", (55.7558f, 37.6173f)},
                {"Санкт-Петербург", (59.9343f, 30.3351f)},
                {"Пермь", (58.0105f, 56.2292f)},
                {"Екатеринбург", (56.8389f, 60.6057f)},
                {"Новосибирск", (55.0084f, 82.9357f)},
                {"Казань", (55.8304f, 49.0661f)},
                {"Нижний Новгород", (56.3269f, 44.0255f)},
                {"Челябинск", (55.1644f, 61.4368f)},
                {"Самара", (53.2415f, 50.2212f)},
                {"Омск", (54.9885f, 73.3242f)},
                {"Ростов-на-Дону", (47.2357f, 39.7015f)},
                {"Уфа", (54.7388f, 55.9721f)},
                {"Красноярск", (56.0184f, 92.8672f)},
                {"Воронеж", (51.6606f, 39.2003f)},
                {"Волгоград", (48.708f, 44.5133f)}
            };

            // Проверяем точное совпадение
            if (cities.ContainsKey(city))
            {
                return cities[city];
            }

            // Проверяем частичное совпадение
            foreach (var kvp in cities)
            {
                if (city.ToLower().Contains(kvp.Key.ToLower()) || kvp.Key.ToLower().Contains(city.ToLower()))
                {
                    return kvp.Value;
                }
            }

            throw new Exception($"Не удалось найти координаты для города: {city}");
        }

        private static async Task<DataResponse> GetWeatherFromApi(float lat, float lon)
        {
            try
            {
                DataResponse DataResponse = null;
                string url = $"{WeatherUrl}?lat={lat.ToString().Replace(',', '.')}&lon={lon.ToString().Replace(',', '.')}";

                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);

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
                Console.WriteLine($"Ошибка сохранения в БД: {ex.Message}");
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

        public static async Task ForceRefresh(string city)
        {
            Console.WriteLine($"Принудительное обновление для города: {city}");

            if (!CanMakeRequest())
            {
                throw new Exception("Достигнут дневной лимит запросов (50)");
            }

            var coordinates = await GetCoordinates(city);
            var apiData = await GetWeatherFromApi(coordinates.lat, coordinates.lon);
            CacheWeatherData(city, apiData);
        }
    }
}