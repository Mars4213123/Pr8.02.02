using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Weather_Кантуганов.Models;

namespace Weather_Кантуганов.Elements
{
    /// <summary>
    /// Логика взаимодействия для item.xaml
    /// </summary>
    public partial class item : UserControl
    {
        public item(Hour hour)
        {
            InitializeComponent();
            lHour.Content = hour.hour;
            lCondition.Content = hour.ToCondition();
            lHumandity.Content = hour.humidity + "%";
            lPrecType.Content = hour.toPrecType();
            lTemp.Content = hour.temp + "°C";
        }
    }
}
