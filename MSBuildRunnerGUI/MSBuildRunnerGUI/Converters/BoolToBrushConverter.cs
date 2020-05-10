using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using MSBuildRunnerGUI.Data;

namespace MSBuildRunnerGUI.Converters
{
    public class BoolToBrushConverter : IValueConverter
    {

        public Brush TrueBrush { get; set; }
        public Brush FalseBrush { get; set; }

       public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                if (b)
                {
                    return TrueBrush;
                }

                return FalseBrush;

            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}