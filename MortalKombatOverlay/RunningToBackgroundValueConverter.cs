using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MortalKombatOverlay;

public class RunningToBackgroundColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isRunning && isRunning) return Brushes.Transparent;
        return Brushes.White;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}