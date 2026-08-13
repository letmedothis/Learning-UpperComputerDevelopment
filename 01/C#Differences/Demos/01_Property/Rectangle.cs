namespace C_Differences.Demos._01_Property;

/// <summary>
/// Rectangle 类 - 展示计算属性
/// </summary>
public class Rectangle
{
    public double Width { get; }
    public double Height { get; }

    // 计算属性 - 不存储值，每次计算
    public double Area => Width * Height;

    public Rectangle(double width, double height)
    {
        Width = width;
        Height = height;
    }
}
