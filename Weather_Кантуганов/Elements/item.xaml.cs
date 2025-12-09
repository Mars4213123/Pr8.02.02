using System.Windows.Controls;
using Weather_Кантуганов.Models;

namespace Weather_Кантуганов.Elements
{
    public partial class item : UserControl
    {
        public item(Hour hour)
        {
            InitializeComponent();

            lHour.Content = hour.hour + ":00";
            lCondition.Content = hour.ToCondition();
            lHumandity.Content = hour.humidity + "%";
            lPrecType.Content = hour.toPrecType();
            lTemp.Content = hour.temp + "°C";
        }
    }
}