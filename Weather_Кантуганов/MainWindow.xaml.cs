using System;
using System.Windows;
using Weather_Кантуганов.Classes;
using Weather_Кантуганов.Models;

namespace Weather_Кантуганов
{
    public partial class MainWindow : Window
    {
        DataResponse response;

        public MainWindow()
        {
            InitializeComponent();
        }

        public async void SearchWeather()
        {
            string city = txtCity.Text.Trim();

            if (string.IsNullOrEmpty(city))
            {
                MessageBox.Show("Введите название города");
                return;
            }

            try
            {
                btnSearch.IsEnabled = false;

                response = await GetWeather.GetWeatherData(city);

                Days.Items.Clear();
                foreach (Forecast forecast in response.forecasts)
                {
                    Days.Items.Add(forecast.date);
                }

                if (Days.Items.Count > 0)
                {
                    Days.SelectedIndex = 0;
                    Create(0);
                }

                int requestCount = GetWeather.GetTodayRequestCount();
                lblRequestCount.Content = $"Запросов: {requestCount}/50";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
            finally
            {
                btnSearch.IsEnabled = true;
            }
        }

        public void Create(int idForecast)
        {
            parent.Children.Clear();
            if (response?.forecasts != null && idForecast < response.forecasts.Count)
            {
                foreach (Hour forecast in response.forecasts[idForecast].hours)
                {
                    parent.Children.Add(new Elements.item(forecast));
                }
            }
        }

        private void SelectDays(object sender, RoutedEventArgs e)
        {
            if (Days.SelectedIndex >= 0)
            {
                Create(Days.SelectedIndex);
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchWeather();
        }
    }
}