using System.Windows;
using Weather_Кантуганов.Classes;
using Weather_Кантуганов.Models;

namespace Weather_Кантуганов
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataResponse response;
        public MainWindow()
        {
            InitializeComponent();
            Iint();
        }

        public async void Iint() {
            response = await GetWeather.Get(58.010461f, 56.229188f);
            foreach (Forecast forecast in response.forecasts) {
                Days.Items.Add(forecast.date.ToString());
            }
            Create(0);
        }
        public void Create(int idForecast) {
            parent.Children.Clear();
            foreach (Hour forecast in response.forecasts[idForecast].hours)
            {
                parent.Children.Add(new Elements.item(forecast));
            }
        }

        
        private void SelectDays(object sender, RoutedEventArgs e) => Create(Days.SelectedIndex);

        private void UpdateWeather(object sender, RoutedEventArgs e)
        {
            Iint();
        }
    }
}
