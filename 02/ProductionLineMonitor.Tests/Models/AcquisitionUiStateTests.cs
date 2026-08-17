using ProductionLineMonitor.Core.Models;

namespace ProductionLineMonitor.Tests.Models;

public sealed class AcquisitionUiStateTests
{
    [Theory]
    [InlineData(OperatingState.Stopped, "■ 已停止")]
    [InlineData(OperatingState.Running, "▶ 运行中")]
    [InlineData(OperatingState.Stopping, "⏸ 正在停止")]
    public void GetOperatingStatus_ReturnsTextForCurrentState(
        OperatingState state,
        string expected)
    {
        Assert.Equal(expected, AcquisitionUiState.GetOperatingStatus(state));
    }

    [Fact]
    public void GetCompletionMessage_AfterNormalCancellation_ReturnsStopped()
    {
        Assert.Equal("已停止", AcquisitionUiState.GetCompletionMessage(null));
    }

    [Fact]
    public void GetCompletionMessage_AfterFailure_PreservesError()
    {
        var error = new InvalidOperationException("读取失败");

        Assert.Equal("采集异常: 读取失败", AcquisitionUiState.GetCompletionMessage(error));
    }
}
