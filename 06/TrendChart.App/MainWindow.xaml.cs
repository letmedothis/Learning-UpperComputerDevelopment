using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using TrendChart.Core.Models;
using TrendChart.Core.Services;

namespace TrendChart.App;

public partial class MainWindow : Window
{
    private readonly TrendDataGenerator _generator = new();
    private List<TrendSeries> _series = new();

    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // 生成最近 1 小时的数据，每 10 秒一个点
        _series = _generator.GenerateHistoricalData(TimeSpan.FromHours(1), TimeSpan.FromSeconds(10));
        DrawChart();
        DrawLegend();
        DrawStatistics();
    }

    private void ChartCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_series.Count > 0)
            DrawChart();
    }

    private void DrawChart()
    {
        ChartCanvas.Children.Clear();

        var width = ChartCanvas.ActualWidth;
        var height = ChartCanvas.ActualHeight;

        if (width <= 0 || height <= 0 || _series.Count == 0) return;

        var padding = 50;
        var chartWidth = width - padding * 2;
        var chartHeight = height - padding * 2;

        // 绘制网格线
        DrawGrid(padding, chartWidth, chartHeight);

        // 绘制每个系列
        foreach (var series in _series)
        {
            DrawSeries(series, padding, chartWidth, chartHeight);
        }
    }

    private void DrawGrid(double padding, double chartWidth, double chartHeight)
    {
        var gridBrush = new SolidColorBrush(Color.FromRgb(0x45, 0x45, 0x5A));

        // 水平网格线
        for (int i = 0; i <= 5; i++)
        {
            var y = padding + chartHeight * i / 5;
            var line = new Line
            {
                X1 = padding, Y1 = y,
                X2 = padding + chartWidth, Y2 = y,
                Stroke = gridBrush, StrokeThickness = 0.5
            };
            ChartCanvas.Children.Add(line);
        }

        // 垂直网格线
        for (int i = 0; i <= 6; i++)
        {
            var x = padding + chartWidth * i / 6;
            var line = new Line
            {
                X1 = x, Y1 = padding,
                X2 = x, Y2 = padding + chartHeight,
                Stroke = gridBrush, StrokeThickness = 0.5
            };
            ChartCanvas.Children.Add(line);
        }
    }

    private void DrawSeries(TrendSeries series, double padding, double chartWidth, double chartHeight)
    {
        if (series.Points.Count < 2) return;

        var color = (Color)ColorConverter.ConvertFromString(series.Color);
        var brush = new SolidColorBrush(color);

        var minTime = series.Points.First().Timestamp;
        var maxTime = series.Points.Last().Timestamp;
        var timeRange = (maxTime - minTime).TotalSeconds;
        if (timeRange == 0) timeRange = 1;

        var valueRange = series.MaxValue - series.MinValue;
        if (valueRange == 0) valueRange = 1;

        var geometry = new StreamGeometry();
        using (var ctx = geometry.Open())
        {
            var first = series.Points[0];
            var startX = padding + (first.Timestamp - minTime).TotalSeconds / timeRange * chartWidth;
            var startY = padding + chartHeight - (first.Value - series.MinValue) / valueRange * chartHeight;
            ctx.BeginFigure(new Point(startX, startY), false, false);

            for (int i = 1; i < series.Points.Count; i++)
            {
                var point = series.Points[i];
                var x = padding + (point.Timestamp - minTime).TotalSeconds / timeRange * chartWidth;
                var y = padding + chartHeight - (point.Value - series.MinValue) / valueRange * chartHeight;
                ctx.LineTo(new Point(x, y), true, false);
            }
        }

        var path = new Path { Data = geometry, Stroke = brush, StrokeThickness = 2 };
        ChartCanvas.Children.Add(path);
    }

    private void DrawLegend()
    {
        LegendPanel.Children.Clear();

        foreach (var series in _series)
        {
            var stack = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 5) };

            var colorRect = new Rectangle
            {
                Width = 16, Height = 16,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(series.Color)),
                Margin = new Thickness(0, 0, 8, 0)
            };

            var label = new TextBlock
            {
                Text = $"{series.Name} ({series.Unit})",
                Foreground = new SolidColorBrush(Color.FromRgb(0xE0, 0xE0, 0xE0)),
                FontSize = 13
            };

            stack.Children.Add(colorRect);
            stack.Children.Add(label);
            LegendPanel.Children.Add(stack);
        }
    }

    private void DrawStatistics()
    {
        StatsPanel.Children.Clear();

        var header = new TextBlock
        {
            Text = "统计信息",
            FontSize = 14,
            FontWeight = FontWeights.SemiBold,
            Foreground = new SolidColorBrush(Color.FromRgb(0xE0, 0xE0, 0xE0)),
            Margin = new Thickness(0, 0, 0, 10)
        };
        StatsPanel.Children.Add(header);

        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        grid.ColumnDefinitions.Add(new ColumnDefinition());

        foreach (var (series, index) in _series.Select((s, i) => (s, i)))
        {
            var stats = _generator.CalculateStatistics(series.Points);

            var stack = new StackPanel { Margin = new Thickness(10, 0, 10, 0) };
            stack.Children.Add(new TextBlock
            {
                Text = series.Name,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(series.Color)),
                FontWeight = FontWeights.SemiBold,
                FontSize = 14
            });
            stack.Children.Add(new TextBlock { Text = $"最小: {stats.Min:F2} {series.Unit}", Foreground = new SolidColorBrush(Color.FromRgb(0x9E, 0x9E, 0x9E)), FontSize = 12 });
            stack.Children.Add(new TextBlock { Text = $"最大: {stats.Max:F2} {series.Unit}", Foreground = new SolidColorBrush(Color.FromRgb(0x9E, 0x9E, 0x9E)), FontSize = 12 });
            stack.Children.Add(new TextBlock { Text = $"平均: {stats.Average:F2} {series.Unit}", Foreground = new SolidColorBrush(Color.FromRgb(0x9E, 0x9E, 0x9E)), FontSize = 12 });
            stack.Children.Add(new TextBlock { Text = $"标准差: {stats.StdDev:F2}", Foreground = new SolidColorBrush(Color.FromRgb(0x9E, 0x9E, 0x9E)), FontSize = 12 });

            Grid.SetColumn(stack, index);
            grid.Children.Add(stack);
        }

        StatsPanel.Children.Add(grid);
    }
}
