using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MSBuildRunnerGUI.Converters
{
    public class ToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
           switch(value)
           {
                case null: return "null";
                case Dictionary<string, string> dictionary: return string.Join(", ",dictionary.Select(kv => $"{kv.Key}\t{kv.Value}"));
           }


           return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
