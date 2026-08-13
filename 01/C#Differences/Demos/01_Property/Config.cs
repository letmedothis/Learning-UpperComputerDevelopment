namespace C_Differences.Demos._01_Property;

/// <summary>
/// Config 类 - 展示手动实现的属性，可以添加额外逻辑
/// </summary>
public class Config
{
    private int _timeout;

    public int Timeout
    {
        get => _timeout;
        set
        {
            if (value <= 0)
                throw new ArgumentException("超时时间必须大于0");
            _timeout = value;
            Console.WriteLine($"超时时间已设置为: {value}ms");
        }
    }
}
