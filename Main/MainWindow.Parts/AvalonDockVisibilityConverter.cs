using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Diagnostics;
using System.Windows;

namespace Sokoban
{
    [ValueConversion(/* sourceType */ typeof(System.Windows.Visibility), /* targetType */ typeof(bool))]
    public class AvalonDockVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Debug.Assert(targetType == typeof(bool));

            Visibility visibility = (Visibility)value;
            return visibility != Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool val = (bool)value;
            return val ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
