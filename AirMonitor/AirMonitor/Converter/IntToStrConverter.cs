using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace AirMonitor.Converter
{
    class IntToStrConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int t = (int)Math.Round((double)value * 100);
            return t.ToString() + "%";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int t = Int32.Parse(value.ToString());
            return (double)t / 100;
        }
    }
}
