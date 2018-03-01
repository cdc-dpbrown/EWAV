using System;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Ewav.Converters
{
    public class SideConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double sideLength = 5;

            if(value is double && parameter is double)
            { 
                if (((double)value) != 0)
                {
                    sideLength = 149.0 * ((double)parameter) / ((double)value);
                }
                else
                {
                    sideLength = 1;
                }
            }

            return sideLength;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
