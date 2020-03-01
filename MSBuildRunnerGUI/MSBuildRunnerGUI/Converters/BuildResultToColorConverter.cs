using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using MSBuildRunnerGUI.Data;

namespace MSBuildRunnerGUI.Converters
{
    public class BuildResultToColorConverter : IValueConverter
    {
        private readonly Brush Failed = Brushes.DarkRed;
        private readonly Brush Success = Brushes.GreenYellow;
        private readonly Brush Unknown = Brushes.LightGray;


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Project.BuildResultEnum buildResult)
            {
                switch (buildResult)
                {
                    case Project.BuildResultEnum.Successful: return Success;
                    case Project.BuildResultEnum.Failed: return Failed;
                }
            }

            return Unknown;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}