using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using ProductionLineMonitor.Core.Models;

namespace ProductionLineMonitor.App.Converters;

/// <summary>
/// 将 MetricLevel 枚举转换为对应的 Brush。
/// </summary>
public sealed class MetricLevelToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is MetricLevel level ? level switch
        {
            MetricLevel.Normal => new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50)),  // 绿色
            MetricLevel.Warning => new SolidColorBrush(Color.FromRgb(0xFF, 0x98, 0x00)), // 橙色
            MetricLevel.Alarm => new SolidColorBrush(Color.FromRgb(0xF4, 0x43, 0x36)),   // 红色
            _ => new SolidColorBrush(Color.FromRgb(0x9E, 0x9E, 0x9E))                   // 灰色
        } : new SolidColorBrush(Color.FromRgb(0x9E, 0x9E, 0x9E));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
