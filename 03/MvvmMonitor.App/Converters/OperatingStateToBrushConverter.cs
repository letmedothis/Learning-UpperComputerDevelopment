using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using MvvmMonitor.Core.Models;

namespace MvvmMonitor.App.Converters;

public sealed class OperatingStateToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is OperatingState state ? state switch
        {
            OperatingState.Stopped => new SolidColorBrush(Color.FromRgb(0x9E, 0x9E, 0x9E)),
            OperatingState.Running => new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50)),
            OperatingState.Stopping => new SolidColorBrush(Color.FromRgb(0xFF, 0x98, 0x00)),
            _ => new SolidColorBrush(Color.FromRgb(0x9E, 0x9E, 0x9E))
        } : new SolidColorBrush(Color.FromRgb(0x9E, 0x9E, 0x9E));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
