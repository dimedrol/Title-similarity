using System;
using System.Windows;
using System.Windows.Data;

namespace TitleSimilarity
{
    public class InvertBoolConverter : IValueConverter
    {
        public object Convert( object value , Type targetType , object parameter ,
            System.Globalization.CultureInfo culture )
        {
            if( value is bool )
            {
                return !(bool) value;
            }
            return value;
        }

        public object ConvertBack( object value , Type targetType , object parameter ,
            System.Globalization.CultureInfo culture )
        {
            if( value is bool )
            {
                return !(bool) value;
            }
            return value;
        }
    }
}
