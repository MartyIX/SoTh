using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Diagnostics;
using System.Windows;

namespace Sokoban.View.GameDocsComponents
{
    [ValueConversion(/* sourceType */ typeof(int), /* targetType */ typeof(Rect))]
    public class FieldSizeToRectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Debug.Assert(targetType == typeof(Rect));

            double fieldSize = double.Parse(value.ToString());
            return new Rect(0, 0, 2 * fieldSize, 2 * fieldSize);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // should not be called in our example
            throw new NotImplementedException();
        }
    }

}
