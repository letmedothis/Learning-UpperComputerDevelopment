using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using ProductionLineMonitor.Core.Models;

namespace ProductionLineMonitor.App.Converters;

/// <summary>
/// 将 OperatingState 转换为对应的 Brush。
/// </summary>
public sealed class OperatingStateToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is OperatingState state ? state switch
        {
            OperatingState.Stopped => new SolidColorBrush(Color.FromRgb(0x9E, 0x9E, 0x9E)),  // 灰色
            OperatingState.Running => new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50)),  // 绿色
            OperatingState.Stopping => new SolidColorBrush(Color.FromRgb(0xFF, 0x98, 0x00)), // 橙色
            _ => new SolidColorBrush(Color.FromRgb(0x9E, 0x9E, 0x9E))
        } : new SolidColorBrush(Color.FromRgb(0x9E, 0x9E, 0x9E));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
