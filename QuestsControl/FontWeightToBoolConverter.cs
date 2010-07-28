using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows;

namespace Sokoban.View
{
    [ValueConversion(/* sourceType */ typeof(FontWeight), /* targetType */ typeof(bool))]
    public class FontWeightToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Debug.Assert(targetType == typeof(System.Windows.FontWeight));

            //FontWeight fw = (FontWeight)value;


            return (((bool)value) == true)  ? FontWeights.Bold : FontWeights.Normal;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool val = (bool)value;
            return val ? FontWeights.Bold : FontWeights.Normal;
        }
    }

}
