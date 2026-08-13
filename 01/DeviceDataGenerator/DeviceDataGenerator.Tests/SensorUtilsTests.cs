using DeviceDataGenerator.Utils;
using Xunit;

namespace DeviceDataGenerator.Tests;

/// <summary>
/// SensorUtils 单元测试
/// </summary>
public class SensorUtilsTests
{
    // ========== 倍率转换测试 ==========

    [Fact]
    public void ApplyMultiplier_WithMultiplierOnly_ReturnsCorrectResult()
    {
        // Arrange
        double rawValue = 100;
        double multiplier = 2.5;

        // Act
        double result = SensorUtils.ApplyMultiplier(rawValue, multiplier);

        // Assert
        Assert.Equal(250, result);
    }

    [Fact]
    public void ApplyMultiplier_WithMultiplierAndOffset_ReturnsCorrectResult()
    {
        // Arrange
        double rawValue = 1024;
        double multiplier = 0.1;
        double offset = -50;

        // Act
        double result = SensorUtils.ApplyMultiplier(rawValue, multiplier, offset);

        // Assert
        Assert.Equal(52.4, result, 1); // 1024 * 0.1 - 50 = 52.4
    }

    [Theory]
    [InlineData(0, 32)]        // 0°C = 32°F
    [InlineData(100, 212)]     // 100°C = 212°F
    [InlineData(-40, -40)]     // -40°C = -40°F
    [InlineData(25, 77)]       // 25°C = 77°F
    public void CelsiusToFahrenheit_ReturnsCorrectValues(double celsius, double expectedFahrenheit)
    {
        // Act
        double result = SensorUtils.CelsiusToFahrenheit(celsius);

        // Assert
        Assert.Equal(expectedFahrenheit, result, 1);
    }

    [Theory]
    [InlineData(32, 0)]        // 32°F = 0°C
    [InlineData(212, 100)]     // 212°F = 100°C
    [InlineData(77, 25)]       // 77°F = 25°C
    public void FahrenheitToCelsius_ReturnsCorrectValues(double fahrenheit, double expectedCelsius)
    {
        // Act
        double result = SensorUtils.FahrenheitToCelsius(fahrenheit);

        // Assert
        Assert.Equal(expectedCelsius, result, 1);
    }

    [Fact]
    public void TemperatureConversion_RoundTrip_ReturnsOriginalValue()
    {
        // Arrange
        double original = 25.0;

        // Act
        double converted = SensorUtils.CelsiusToFahrenheit(original);
        double roundTrip = SensorUtils.FahrenheitToCelsius(converted);

        // Assert
        Assert.Equal(original, roundTrip, 10);
    }

    [Theory]
    [InlineData(0.1, 14.504)]
    [InlineData(0.5, 72.519)]
    [InlineData(1.0, 145.038)]
    public void MpaToPsi_ReturnsCorrectValues(double mpa, double expectedPsi)
    {
        // Act
        double result = SensorUtils.MpaToPsi(mpa);

        // Assert
        Assert.Equal(expectedPsi, result, 2);
    }

    [Fact]
    public void PressureConversion_RoundTrip_ReturnsOriginalValue()
    {
        // Arrange
        double original = 0.3;

        // Act
        double converted = SensorUtils.MpaToPsi(original);
        double roundTrip = SensorUtils.PsiToMpa(converted);

        // Assert
        Assert.Equal(original, roundTrip, 10);
    }

    // ========== 范围判断测试 ==========

    [Theory]
    [InlineData(10, 0, 20, true)]    // 在范围内
    [InlineData(0, 0, 20, true)]     // 等于最小值
    [InlineData(20, 0, 20, true)]    // 等于最大值
    [InlineData(-1, 0, 20, false)]   // 小于最小值
    [InlineData(21, 0, 20, false)]   // 大于最大值
    public void IsInRange_ReturnsCorrectResults(double value, double min, double max, bool expected)
    {
        // Act
        bool result = SensorUtils.IsInRange(value, min, max);

        // Assert
        Assert.Equal(expected, result);
    }

    // ========== 温度范围判断测试 ==========

    [Theory]
    [InlineData(20, true)]   // 正常温度
    [InlineData(25, true)]   // 正常温度
    [InlineData(30, true)]   // 正常温度
    [InlineData(10, false)]  // 过低
    [InlineData(40, false)]  // 过高
    public void IsTemperatureNormal_ReturnsCorrectResults(double temperature, bool expected)
    {
        // Act
        bool result = SensorUtils.IsTemperatureNormal(temperature);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(5, "过低")]
    [InlineData(12, "偏低")]
    [InlineData(25, "正常")]
    [InlineData(38, "偏高")]
    [InlineData(45, "过高")]
    public void GetTemperatureStatus_ReturnsCorrectStatus(double temperature, string expectedStatus)
    {
        // Act
        string result = SensorUtils.GetTemperatureStatus(temperature);

        // Assert
        Assert.Equal(expectedStatus, result);
    }

    // ========== 压力范围判断测试 ==========

    [Theory]
    [InlineData(0.2, true)]   // 正常压力
    [InlineData(0.3, true)]   // 正常压力
    [InlineData(0.4, true)]   // 正常压力
    [InlineData(0.05, false)] // 过低
    [InlineData(0.6, false)]  // 过高
    public void IsPressureNormal_ReturnsCorrectResults(double pressure, bool expected)
    {
        // Act
        bool result = SensorUtils.IsPressureNormal(pressure);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0.02, "过低")]
    [InlineData(0.08, "偏低")]
    [InlineData(0.3, "正常")]
    [InlineData(0.55, "偏高")]
    [InlineData(0.7, "过高")]
    public void GetPressureStatus_ReturnsCorrectStatus(double pressure, string expectedStatus)
    {
        // Act
        string result = SensorUtils.GetPressureStatus(pressure);

        // Assert
        Assert.Equal(expectedStatus, result);
    }

    // ========== 转速范围判断测试 ==========

    [Theory]
    [InlineData(1200, true)]  // 正常转速
    [InlineData(1500, true)]  // 正常转速
    [InlineData(1800, true)]  // 正常转速
    [InlineData(500, false)]  // 过低
    [InlineData(2500, false)] // 过高
    public void IsSpeedNormal_ReturnsCorrectResults(double speed, bool expected)
    {
        // Act
        bool result = SensorUtils.IsSpeedNormal(speed);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(200, "过低")]
    [InlineData(800, "偏低")]
    [InlineData(1500, "正常")]
    [InlineData(2200, "偏高")]
    [InlineData(3000, "过高")]
    public void GetSpeedStatus_ReturnsCorrectStatus(double speed, string expectedStatus)
    {
        // Act
        string result = SensorUtils.GetSpeedStatus(speed);

        // Assert
        Assert.Equal(expectedStatus, result);
    }

    // ========== 数据验证测试 ==========

    [Theory]
    [InlineData(25, 0.3, 1500, true)]      // 有效数据
    [InlineData(double.NaN, 0.3, 1500, false)]  // NaN 温度
    [InlineData(25, double.NaN, 1500, false)]   // NaN 压力
    [InlineData(25, 0.3, double.NaN, false)]    // NaN 转速
    [InlineData(25, 0.3, double.PositiveInfinity, false)]  // Infinity
    [InlineData(200, 0.3, 1500, false)]    // 温度过高
    [InlineData(25, -1, 1500, false)]      // 负压力
    [InlineData(25, 0.3, 15000, false)]    // 转速过高
    public void IsValidReading_ReturnsCorrectResults(double temp, double pressure, double speed, bool expected)
    {
        // Act
        bool result = SensorUtils.IsValidReading(temp, pressure, speed);

        // Assert
        Assert.Equal(expected, result);
    }
}
