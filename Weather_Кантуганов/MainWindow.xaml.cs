using System;
using System.Windows;
using Org.BouncyCastle.Asn1.Cmp;
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
                lblStatus.Content = "Загрузка...";

                response = await GetWeather.GetWeatherData(city);

                Days.Items.Clear();
                foreach (Forecast forecast in response.forecasts)
                {
                    Days.Items.Add(forecast.date.ToString());
                }

                Create(0);

                int requestCount = GetWeather.GetTodayRequestCount();
                lblRequestCount.Content = $"Запросов сегодня: {requestCount}/50";
                lblStatus.Content = "Готово";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
                lblStatus.Content = "Ошибка";
            }
            finally
            {
                btnSearch.IsEnabled = true;
            }
        }

        public void Create(int idForecast)
        {
            parent.Children.Clear();
            if (response.forecasts != null && idForecast < response.forecasts.Count)
            {
                foreach (Hour forecast in response.forecasts[idForecast].hours)
                {
                    parent.Children.Add(new Elements.item(forecast));
                }
            }
        }

        private void SelectDays(object sender, RoutedEventArgs e) => Create(Days.SelectedIndex);

        private void UpdateWeather(object sender, RoutedEventArgs e) => SearchWeather();

        private void SearchButton_Click(object sender, RoutedEventArgs e) => SearchWeather();
    }
}