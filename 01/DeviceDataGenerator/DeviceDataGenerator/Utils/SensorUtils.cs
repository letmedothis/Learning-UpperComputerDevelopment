namespace DeviceDataGenerator.Utils;

/// <summary>
/// 传感器工具类 - 倍率转换和范围判断
///
/// 用于单元测试验证
/// </summary>
public static class SensorUtils
{
    /// <summary>
    /// 倍率转换 - 将原始值转换为实际值
    /// </summary>
    /// <param name="rawValue">原始值</param>
    /// <param name="multiplier">倍率</param>
    /// <param name="offset">偏移量</param>
    /// <returns>转换后的值</returns>
    public static double ApplyMultiplier(double rawValue, double multiplier, double offset = 0)
    {
        return rawValue * multiplier + offset;
    }

    /// <summary>
    /// 温度转换 - 摄氏度转华氏度
    /// </summary>
    public static double CelsiusToFahrenheit(double celsius)
    {
        return celsius * 9.0 / 5.0 + 32;
    }

    /// <summary>
    /// 温度转换 - 华氏度转摄氏度
    /// </summary>
    public static double FahrenheitToCelsius(double fahrenheit)
    {
        return (fahrenheit - 32) * 5.0 / 9.0;
    }

    /// <summary>
    /// 压力单位转换 - MPa 转 PSI
    /// </summary>
    public static double MpaToPsi(double mpa)
    {
        return mpa * 145.038;
    }

    /// <summary>
    /// 压力单位转换 - PSI 转 MPa
    /// </summary>
    public static double PsiToMpa(double psi)
    {
        return psi / 145.038;
    }

    /// <summary>
    /// 范围判断 - 检查值是否在指定范围内
    /// </summary>
    /// <param name="value">要检查的值</param>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <returns>是否在范围内</returns>
    public static bool IsInRange(double value, double min, double max)
    {
        return value >= min && value <= max;
    }

    /// <summary>
    /// 温度范围判断 - 正常范围检查
    /// </summary>
    public static bool IsTemperatureNormal(double temperature)
    {
        // 正常温度范围: 15°C ~ 35°C
        return IsInRange(temperature, 15.0, 35.0);
    }

    /// <summary>
    /// 温度范围判断 - 返回状态描述
    /// </summary>
    public static string GetTemperatureStatus(double temperature)
    {
        if (temperature < 10)
            return "过低";
        if (temperature < 15)
            return "偏低";
        if (temperature <= 35)
            return "正常";
        if (temperature <= 40)
            return "偏高";
        return "过高";
    }

    /// <summary>
    /// 压力范围判断 - 正常范围检查
    /// </summary>
    public static bool IsPressureNormal(double pressure)
    {
        // 正常压力范围: 0.1 MPa ~ 0.5 MPa
        return IsInRange(pressure, 0.1, 0.5);
    }

    /// <summary>
    /// 压力范围判断 - 返回状态描述
    /// </summary>
    public static string GetPressureStatus(double pressure)
    {
        if (pressure < 0.05)
            return "过低";
        if (pressure < 0.1)
            return "偏低";
        if (pressure <= 0.5)
            return "正常";
        if (pressure <= 0.6)
            return "偏高";
        return "过高";
    }

    /// <summary>
    /// 转速范围判断 - 正常范围检查
    /// </summary>
    public static bool IsSpeedNormal(double speed)
    {
        // 正常转速范围: 1000 rpm ~ 2000 rpm
        return IsInRange(speed, 1000, 2000);
    }

    /// <summary>
    /// 转速范围判断 - 返回状态描述
    /// </summary>
    public static string GetSpeedStatus(double speed)
    {
        if (speed < 500)
            return "过低";
        if (speed < 1000)
            return "偏低";
        if (speed <= 2000)
            return "正常";
        if (speed <= 2500)
            return "偏高";
        return "过高";
    }

    /// <summary>
    /// 数据校验 - 检查读数是否有效
    /// </summary>
    public static bool IsValidReading(double temperature, double pressure, double speed)
    {
        // 基本有效性检查
        return !double.IsNaN(temperature) &&
               !double.IsNaN(pressure) &&
               !double.IsNaN(speed) &&
               !double.IsInfinity(temperature) &&
               !double.IsInfinity(pressure) &&
               !double.IsInfinity(speed) &&
               temperature > -100 && temperature < 200 &&
               pressure >= 0 && pressure < 10 &&
               speed >= 0 && speed < 10000;
    }
}
